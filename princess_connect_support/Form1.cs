﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace princess_connect_support
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            TabPage page = tabControl1.TabPages[0];
            page.Text = "角色裝備";

            log = new Label();

            log.Parent = page;
            log.Height = 400;
            log.Width = 200;
            log.Left = 400;

            c_list = new List<Character>();
            equip_dict = new Dictionary<string, List<Need>>();
        }

        private void new_character_but_Click(object sender, EventArgs e)
        {
            //Form2 f2 = new Form2();
            //f2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // read character files
            if (!Directory.Exists(character_dir))
            {
                log.Text = "character dir doesn\'t exist";
                return;
            }

            string[] file_paths = Directory.GetFiles(character_dir);

            foreach(string file_path in file_paths)
                using (StreamReader sr = new StreamReader(file_path))
                    c_list.Add(new Character(sr));

            // write to files
            //foreach(Character c in c_list)
            //{
            //    using (StreamWriter sw = new StreamWriter(character_dir_c + "/" + c.Name + ".txt"))
            //    {
            //        sw.WriteLine(c.Name);
            //        foreach(Need n in c.Needs)
            //        {
            //            sw.WriteLine(n.Name + " " + n.Num.ToString() + " " + n.Rank.ToString() + " " + n.Have);
            //        }
            //    }
            //}

            // read the equipment_list file
            using (StreamReader sr = new StreamReader(equipment_list_path))
                get_equip_dict(sr);

            // summary
            // dict<rank, dict<equip_name, num>>
            Dictionary<int, Dictionary<string, int>> all_need_rank_num = new Dictionary<int, Dictionary<string, int>>();
            // dict<character_name, dict<rank, dict<equip_name, num>>>
            Dictionary<string, Dictionary<int, Dictionary<string, int>>> all_c_need_rank_num = new Dictionary<string, Dictionary<int, Dictionary<string, int>>>();

            foreach (Character c in c_list)
            {
                all_c_need_rank_num[c.Name] = new Dictionary<int, Dictionary<string, int>>();

                foreach (Need c_n in c.Needs)
                    if (!c_n.Have)
                    {
                        // summary all needs of c_n
                        Dictionary<string, int> c_n_need_num= new Dictionary<string, int>();
                        Queue<Need> q = new Queue<Need>();

                        q.Enqueue(c_n);

                        while (q.Count != 0)
                        {
                            Need q_n = q.Dequeue();

                            if (equip_dict.ContainsKey(q_n.Name))
                            {
                                // found
                                foreach (Need n in equip_dict[q_n.Name])
                                    q.Enqueue(new Need(n.Name, n.Num * q_n.Num));
                            }
                            else
                            {
                                // min item
                                if (c_n_need_num.ContainsKey(q_n.Name))
                                    c_n_need_num[q_n.Name] += q_n.Num;
                                else
                                    c_n_need_num[q_n.Name] = q_n.Num;
                            }
                        }

                        // summary those characters' needs, which is classified by rank
                        if (!all_need_rank_num.ContainsKey(c_n.Rank))
                            all_need_rank_num[c_n.Rank] = new Dictionary<string, int>();

                        foreach (KeyValuePair<string, int> pair in c_n_need_num)
                            if (all_need_rank_num[c_n.Rank].ContainsKey(pair.Key))
                                all_need_rank_num[c_n.Rank][pair.Key] += pair.Value;
                            else
                                all_need_rank_num[c_n.Rank][pair.Key] = pair.Value;

                        // summary those characters' needs
                        if (all_c_need_rank_num[c.Name].ContainsKey(c_n.Rank))
                            foreach (KeyValuePair<string, int> pair in c_n_need_num)
                                if (all_c_need_rank_num[c.Name][c_n.Rank].ContainsKey(pair.Key))
                                    all_c_need_rank_num[c.Name][c_n.Rank][pair.Key] += pair.Value;
                                else
                                    all_c_need_rank_num[c.Name][c_n.Rank][pair.Key] = pair.Value;
                        else
                            all_c_need_rank_num[c.Name][c_n.Rank] = c_n_need_num;
                    }
            }

            // output the all_need_rank_num
            using (StreamWriter sw = new StreamWriter(rank_summary_path))
                foreach (KeyValuePair<int, Dictionary<string, int>> rank_pair in all_need_rank_num)
                {
                    sw.WriteLine("R" + rank_pair.Key.ToString());

                    foreach (KeyValuePair<string, int> pair in rank_pair.Value)
                        sw.WriteLine(pair.Key + " " + pair.Value.ToString());

                    sw.WriteLine("");
                }

            // output the summary
            using (StreamWriter sw = new StreamWriter(summary_path))
            {
                Dictionary<string, int> all_need_num = new Dictionary<string, int>();

                foreach (KeyValuePair<int, Dictionary<string, int>> rank_pair in all_need_rank_num)
                    foreach (KeyValuePair<string, int> pair in rank_pair.Value)
                        if (all_need_num.ContainsKey(pair.Key))
                            all_need_num[pair.Key] += pair.Value;
                        else
                            all_need_num[pair.Key] = pair.Value;

                foreach (KeyValuePair<string, int> pair in all_need_num)
                    sw.WriteLine(pair.Key + " " + pair.Value.ToString());
            }

            // output the character_rank_summary
            if (!Directory.Exists(character_rank_summary_dir))
                Directory.CreateDirectory(character_rank_summary_dir);
            
            foreach (KeyValuePair<string, Dictionary<int, Dictionary<string, int>>> c_pair in all_c_need_rank_num)
            {
                string path = character_rank_summary_dir + "/" + c_pair.Key + ".txt";

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(c_pair.Key);

                    foreach (KeyValuePair<int, Dictionary<string, int>> rank_pair in c_pair.Value)
                    {
                        sw.WriteLine("R" + rank_pair.Key.ToString());

                        foreach (KeyValuePair<string, int> pair in rank_pair.Value)
                            sw.WriteLine(pair.Key + " " + pair.Value.ToString());
                    }
                }
            }

            // output the character_summary
            if (!Directory.Exists(character_summary_dir))
                Directory.CreateDirectory(character_summary_dir);

            foreach (KeyValuePair<string, Dictionary<int, Dictionary<string, int>>> c_pair in all_c_need_rank_num)
            {
                string path = character_summary_dir + "/" + c_pair.Key + ".txt";

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(c_pair.Key);

                    Dictionary<string, int> all_c_need_num = new Dictionary<string, int>();

                    foreach (KeyValuePair<int, Dictionary<string, int>> rank_pair in c_pair.Value)
                        foreach (KeyValuePair<string, int> pair in rank_pair.Value)
                            if (all_c_need_num.ContainsKey(pair.Key))
                                all_c_need_num[pair.Key] += pair.Value;
                            else
                                all_c_need_num[pair.Key] = pair.Value;

                    foreach (KeyValuePair<string, int> pair in all_c_need_num)
                        sw.WriteLine(pair.Key + " " + pair.Value.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //using(StreamWriter sw = new StreamWriter(need_file_path))
            //{
            //    for(int i = 0; i < names.Count; ++i)
            //    {
            //        sw.WriteLine(names[i]);

            //        CheckedListBox clb = clbs[i];

            //        for (int j = 0; j < clb.Items.Count; ++j)
            //        {
            //            if (clb.CheckedIndices.IndexOf(j) == -1)
            //            {
            //                sw.WriteLine(clb.Items[j]);
            //            }
            //        }

            //        sw.WriteLine();
            //    }
            //}
        }

        private void get_equip_dict(StreamReader sr)
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                List<Need> needs = new List<Need>();
                string name = line;

                while ((line = sr.ReadLine()) != "")
                {
                    string[] line_split = line.Split(' ');
                    needs.Add(new Need(line_split[0], int.Parse(line_split[1])));
                }

                if (equip_dict.ContainsKey(name))
                {
                    log.Text += name + " is duplicate!!\n";
                }

                equip_dict[name] = needs;
            }
        }

        List<Character> c_list;
        Dictionary<string, List<Need>> equip_dict;

        Label log;

        string character_dir = "character";
        string character_summary_dir = "character_summary";
        string character_rank_summary_dir = "character_rank_summary";

        string equipment_list_path = "equipment_list.txt";
        string summary_path = "summary.txt";
        string rank_summary_path = "rank_summary.txt";
    }
}
