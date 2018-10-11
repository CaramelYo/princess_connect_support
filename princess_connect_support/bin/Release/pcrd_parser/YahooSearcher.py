from urllib import request
import requests
import json
import re
from pymongo import MongoClient, errors
import datetime
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from time import sleep
import sys


# for web driver
board = 'travel'
chrome_driver_path = '../chromedriver.exe'
chrome_options = '--disable-notifications'
yahoo_page_url = 'https://tw.news.yahoo.com/%s' % (board)
scroll_length = 5000
driver = None

# mongo db
mongodb_url = "mongodb://user:cssalab2017@140.116.154.88/?authSource=cssalab"
client = MongoClient(mongodb_url)
db = client['cssalab']

# regexp news title pattern
news_title_pattern = re.compile(
    r'class="C\(#959595\) Fz\(13px\) C\(#979ba7\)! D\(ib\) Mb\(6px\)"[^>]*><span[^>]*>(?P<provider>[^<]*)</span><i[^>]*>[^<]*</i><span[^>]*>(?P<time>[^<]*)</span></div>[^<]*<h3[^>]*><a href="(?P<href>[^"]*)"[^>]*><u[^>]*></u><!-- [^>]*>(?P<title>[^<]*)')

# regexp news article pattern
news_article_pattern = re.compile(r'\"liveCoverageEventId\".*?\"_viewedContentIds\"', re.DOTALL)

# regexp tag parttern
tag_pattern = re.compile(r'</?[^>]*>')

# to get posted time in article page
posted_time_pattern = re.compile(
    r'class="date Fz\(13px\) D\(ib\) Mb\(4px\)  D\(ib\)" datetime="(?P<time>[^"]*)"')
# <time class="date Fz(13px) D(ib) Mb(4px)  D(ib)" datetime="2017-06-23T21:50:04.000Z" itemprop="datePublished" data-reactid="15">

# log file
log_file = None


def print_message(message):
    global log_file

    print(message)
    log_file.write('%s\n' % (message))


def time_string_to_datetime(time_string):
    # publishDate:Sun, 25 Jun 2017 07:24:27 GMT
    return datetime.datetime.strptime(time_string, '%a, %d %b %Y %H:%M:%S GMT')


def insert_to_mongodb(collection, documents):
    # to ensure that if there has a duplicate key, the remain documents will insert to mongo db correctly
    # temp_time = datetime.datetime.now()
    # temp_order = 1
    while True:
        try:
            collection.insert_many(documents)
            break
        except errors.BulkWriteError as bwe:
            # 'errmsg': 'E11000 duplicate key error collection: cssalab.Original_Post_lioncyber index: _id_ dup key: { : "44925918279_10155212057548280" }'
            if bwe.details['writeErrors'][0]['code'] == 11000:
                error_dict = bwe.details['writeErrors'][0]
                duplicate_key = error_dict['errmsg'].split('\"')[1]
                print_message('duplicate key: %s' % (duplicate_key))

                documents_len = len(documents)
                index = error_dict['index']
                error_document = error_dict['op']
                error_document_time = error_document['postedTime']
                error_document_title = error_document['title']

                duplicate_data = collection.find({'postedTime': error_document_time})
                temp_order = 0
                is_the_same = False
                for data in duplicate_data:
                    if error_document_title != data['title']:
                        # both the news are published in the same time
                        temp_order = data['order'] if temp_order < data['order'] else temp_order
                    else:
                        # both the news are the same
                        next_index = index + 1
                        is_the_same = True
                        if next_index < documents_len:
                            documents = documents[next_index:documents_len]
                        else:
                            # there is no document which needs to insert to mongo db in documents
                            documents = []
                            # break
                            # return

                        break

                if is_the_same:
                    # print('the same')
                    print_message('the same')
                    continue
                else:
                    temp_order += 1
                    error_document['order'] = temp_order
                    error_document['_id'] = '%s_%d' % (
                        error_document['postedTime'], error_document['order'])
                    # print('duplicate data count = %d\nnew error_document id = %s'%(duplicate_data.count(), error_document['_id']))
                    print_message('duplicate data count = %d\nnew error_document id = %s' %
                                  (duplicate_data.count(), error_document['_id']))
                    documents = documents[index:documents_len]
                    print(documents[0]['_id'])

                # db_document = collection.find_one({"_id": duplicate_key})
                # error_document = error_dict['op']
                # document_in_list = documents[index]
                # print('index = %d\ndb_document title = %s\nerror_document title = %s'%(index, db_document['title'], error_document['title']))

                # if error_document['title'] != db_document['title']:
                # 	# Both the news are published in the same time
                # 	# db_document_id = db_document['_id']

                # 	order = int(db_document['_id'].split('_')[1]) + 1
                # 	# error_time =
                # 	error_document['_id'] = '%s_%d'%(error_document['_id'].split('_')[0], order)
                # 	print('error_document id = %s'%(error_document['_id']))
                # 	# temp = index - 1
                # 	# if temp != -1:
                # 	documents = documents[index:len(documents)]
                # else:
                # 	# both the news are the same
                # 	documents = documents[index + 1:len(documents)]

                # documents_len = len(documents)
                # # is_find = False
                # temp = -1

                # # to drop all indexes before the last index of duplicate_key
                # for i in range(documents_len):
                # 	if documents[i]['_id'] == duplicate_key:
                # 		temp = i
                # 		break
                # 		# if is_find:
                # 		# 	temp = i
                # 		# 	break
                # 		# else:
                # 		# 	is_find = True

                # if temp != -1:
                # 	print('temp = %d'%(temp))
                # 	documents = documents[temp+1:documents_len]
                # 	# exit()
                # else:
                # 	print('there is no _id %s in documents'%(duplicate_key))
                # 	exit()
            else:
                # print('unexpected error %s'%(bwe))
                print_message('unexpected error %s insert_to_mongodb' % (bwe))
                with open('yahoo_parser_error.txt', 'w', encoding='utf-8') as error_file:
                    for err in bwe.details.keys():
                        error_file.write('%s: %s \n' % (err, bwe.details[err]))
                # exit()
                sys.exit('unexpected error %s insert_to_mongodb' % (bwe))

                # raise
        except TypeError as te:
            # print('type error: %s'%(te))
            print_message('type error: %s insert_to_mongodb' % (te))
            if str(te) != 'documents must be a non-empty list':
                # print('unexpected type error')
                print_message('unexpected type error insert_to_mongodb')
                # exit()
                sys.exit('unexpected type error insert_to_mongodb')
            break

# def yahoo_parser(from_time, isPos = False):


def yahoo_parser(isPos=False):
    # # to get yahoo news website
    # news_response = requests.get('https://tw.news.yahoo.com/%s/'%(board))

    # # to create directory path
    # directory_path = os.path.join(os.getcwd(), 'Yahoo')
    # if not os.path.isdir(directory_path):
    # 	os.makedirs(directory_path)
    directory_path = ''

    # # to record original news website
    # with open('%s/Original_News.html'%(directory_path), 'w', encoding = 'utf-8') as result:
    # 	result.write(news_response.text)

    global driver, db

    # to get the collection of db
    collection = db['Original_News_Yahoo']
    documents = []
    # now = dattetime.datetime.now()
    # print('now = %s'%(now))

    # to extract news
    news_number = 0

    driver.get(yahoo_page_url)

    # print('wait 10 sec')
    #
    # try:
    #     element = WebDriverWait(driver, 10).until(EC.presence_of_element_located(By.ID, ""))

    # while True:
    # link = driver.find_element_by_css_selector('a.C\(\#26282a\).Fw\(b\).Fz\(20px\).Lh\(23px\).LineClamp\(2\,46px\).Fz\(17px\)\-\-sm1024.Lh\(19px\)\-\-sm1024.LineClamp\(2\,38px\)\-\-sm1024.Td\(n\).C\(\#0078ff\)\:h.C\(\#000\)')
    # link = driver.find_element_by_css_selector('h3.Mb\(5px\):nth-of-type(1) a').get_attribute('data-reactid')
    # link = driver.find_element_by_xpath('//h3[1]')
    # link = driver.find_element_by_xpath('//h3[10]')
    # link = driver.find_element_by_xpath('//h3[@class = "Mb(5px)"][1]')
    # link = driver.find_element_by_xpath('//h3[0]')

    # to get the whole news in the yahoo news page
    links = []
    for i in range(10):
        links = driver.find_elements_by_xpath('//div[@class = "C(#959595) Fz(13px) C($c-fuji-grey-f)! D(ib) Mb(6px)"]/../h3/a')
        print_message('links len = %d' % (len(links)))
        driver.execute_script('window.scrollBy(0, %d);'%(scroll_length))
        sleep(2)


    # create a new driver for articles
    article_driver = initialize_web_driver()
    print_message('start to get the articles')
    init_order = 0

    # to get the attributes of the yahoo news
    for link in links:
        url = link.get_attribute('href')

        # static web version before 20171012
        # article_text = requests.get(url).text
        # dynamic web version after 20171012
        # the content of links is directly attached to driver
        article_driver.get(url)

        article_text = article_driver.find_element_by_xpath('//*').get_attribute('outerHTML')
        article_matchs = news_article_pattern.finditer(article_text)

        if article_matchs:
            for article_match in article_matchs:
                json_text = article_match.group(0)
                json_text = json_text[:len(json_text) - 22]
                data = json.loads('{%s}' % (json_text))

                document = {}

                if 'publishDate' in data.keys():
                    time = time_string_to_datetime(data['publishDate'])

                    # if time < from_time:
                    # 	# print('time %s is earlier than from_time'%(time, from_time))
                    # 	print_message('time %s is earlier than from_time'%(time, from_time))
                    # 	# exit()
                    # 	continue

                    document['postedTime'] = time
                    document['order'] = init_order
                    document['_id'] = '%s_%d' % (document['postedTime'], document['order'])
                else:
                    print_message('no publishDate in data\n%s' % (url))

                if 'title' in data.keys():
                    document['title'] = data['title']
                    # print_message('title = %s'%(data['title']))
                else:
                    print_message('no title in data\n%s' % (url))

                if 'attribution' in data.keys():
                    if 'provider' in data['attribution'].keys() and 'name' in data['attribution']['provider']:
                        document['provider'] = data['attribution']['provider']['name']
                    else:
                        print_message('no provider or name in attribution in data\n%s' % (url))
                else:
                    print_message('no attribution in data\n%s' % (url))

                if 'body' in data.keys():
                    document['article'] = ''
                    body_list = data['body']

                    for body in body_list:
                        if 'type' in body and body['type'] == 'text' and 'tagName' in body and body['tagName'] == 'p':
                            document['article'] += body['content'] + '\n'
                else:
                    print_message('no body in data\n%s' % (url))
        else:
            print_message('there is no article_matchs')
            sys.exit('there is no article_matchs')

        documents.append(document)
        news_number += 1

    print_message('start to insert data into mongodb')

    insert_to_mongodb(collection, documents)
    documents.clear()
    end_web_driver(article_driver)
    print_message('news number = %s' % (news_number))

def initialize_web_driver():
    options = Options()
    options.add_argument(chrome_options)
    driver = webdriver.Chrome(chrome_driver_path, chrome_options = options)
    driver.implicitly_wait(10)
    return driver

def end_web_driver(driver):
    if driver != None:
        driver.close()


if __name__ == '__main__':
    log_file = open('yahoo_parser_log.txt', 'a', encoding='utf-8')

    print_message('\n\n\n----------\n\n')
    print_message('start time = %s' % (datetime.datetime.now()))
    print_message('main program')

    driver = initialize_web_driver()

    # from_time = datetime.datetime(2014, 1, 1, 0, 0, 0)

    # yahoo_parser(from_time, False)
    yahoo_parser()

    end_web_driver(driver)
