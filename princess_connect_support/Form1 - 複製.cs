using System;
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

            character_list = new List<Character>();

            log = new Label();

            log.Parent = page;
            log.Height = 400;
            log.Width = 100;
            log.Left = 400;
        }

        private void new_character_but_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TabPage page = tabControl1.TabPages[0];
            
            names = new List<string>();
            equipment_list = new List<List<string>>();
            clbs = new List<CheckedListBox>();

            using (StreamReader sr = new StreamReader(need_file_path))
            {
                string line;
                int top = 0, top_step = 10;

                while ((line = sr.ReadLine()) != null)
                {
                    string name = line;
                    names.Add(name);

                    List<string> eqs = new List<string>();
                    while ((line = sr.ReadLine()) != "")
                    {
                        eqs.Add(line);
                    }

                    equipment_list.Add(eqs);

                    GroupBox gb = new GroupBox();
                    gb.Parent = page;
                    top += top_step;
                    gb.Top = top;
                    gb.Height = 150;
                    gb.Width = 150;
                    top += gb.Height;

                    int gb_top = 0, gb_top_step = 10;

                    Label name_label = new Label();
                    name_label.Parent = gb;
                    gb_top += gb_top_step;
                    name_label.Top = gb_top;
                    name_label.Height = 12;
                    name_label.Width = 48;
                    gb_top += name_label.Height;

                    name_label.Text = name;

                    CheckedListBox equipment_clb = new CheckedListBox();
                    equipment_clb.Parent = gb;
                    gb_top += gb_top_step;
                    equipment_clb.Top = gb_top;
                    equipment_clb.Height = 120;
                    equipment_clb.Width = 150;
                    gb_top += equipment_clb.Height;

                    foreach (string equipment in eqs)
                    {
                        equipment_clb.Items.Add(equipment);
                    }

                    clbs.Add(equipment_clb);
                }
            }

            // func
            sum();

            page = tabControl1.TabPages[1];

            if (!Directory.Exists(character_dir))
            {
                return;
            }

            string[] file_paths = Directory.GetFiles(character_dir);

            foreach (string file_path in file_paths)
            {

                using (StreamReader sr = new StreamReader(file_path))
                {
                    string name = sr.ReadLine();
                    int rank = Int32.Parse(sr.ReadLine());

                    Character c = new Character(name, rank);
                    List<string> eqs = new List<string>();
                    List<bool> yns = new List<bool>();

                    GroupBox gb = new GroupBox();
                    gb.Parent = page;
                    gb.Height = 500;
                    gb.Width = 500;

                    int gb_top = 0;
                    int top_step = 10;

                    Label name_label = new Label();
                    name_label.Parent = gb;
                    gb_top += top_step;
                    name_label.Top = gb_top;
                    name_label.Height = 12;
                    name_label.Width = 48;
                    gb_top += name_label.Height;

                    name_label.Text = c.get_name();

                    for (int i = 0; i <= 7 - rank; ++i)
                    {
                        CheckedListBox equipment_clb = new CheckedListBox();
                        equipment_clb.Parent = gb;
                        gb_top += top_step;
                        equipment_clb.Top = gb_top;
                        equipment_clb.Height = 130;
                        equipment_clb.Width = 150;

                        equipment_clb.Items.Add(sr.ReadLine());

                        for (int j = 0; j < 6; ++j)
                        {
                            string line = sr.ReadLine();
                            string[] line_split = line.Split(' ');
                            
                            string eq = line_split[0];
                            bool b = Convert.ToBoolean(line_split[1].ToString());

                            equipment_clb.Items.Add(eq);
                            equipment_clb.SetItemChecked(equipment_clb.Items.Count - 1, b);

                            eqs.Add(eq);
                            yns.Add(b);
                        }

                        gb_top += equipment_clb.Height;
                    }

                    c.set_eqs(eqs);
                    c.set_yns(yns);

                    character_list.Add(c);
                }
            }
        }

        void sum()
        {
            Dictionary<string, int> sum = new Dictionary<string, int>();

            foreach (List<string> eqs in equipment_list)
            {
                foreach (string eq in eqs)
                {
                    if (sum.ContainsKey(eq))
                    {
                        sum[eq] += 1;
                    }
                    else
                    {
                        sum[eq] = 1;
                    }
                }
            }

            foreach (KeyValuePair<string, int> pair in sum)
            {
                log.Text += pair.Key + " " + pair.Value.ToString() + "\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(StreamWriter sw = new StreamWriter(need_file_path))
            {
                for(int i = 0; i < names.Count; ++i)
                {
                    sw.WriteLine(names[i]);

                    CheckedListBox clb = clbs[i];

                    for (int j = 0; j < clb.Items.Count; ++j)
                    {
                        if (clb.CheckedIndices.IndexOf(j) == -1)
                        {
                            sw.WriteLine(clb.Items[j]);
                        }
                    }

                    sw.WriteLine();
                }
            }
        }

        List<Character> character_list;
        List<string> names;
        List<List<string>> equipment_list;
        List<CheckedListBox> clbs;

        Label log;

        string character_dir = "character";
        string need_file_path = "need.txt";
    }
}
