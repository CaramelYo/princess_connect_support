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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            

            Label name_label = new Label();
            name_label.Parent = this;
            f2_top += f2_step;
            name_label.Top = f2_top;
            name_label.Height = 12;
            name_label.Width = 48;
            f2_top += name_label.Height;

            name_label.Text = "姓名";

            name_tb = new TextBox();
            name_tb.Parent = this;
            name_tb.Top = name_label.Top - 5;
            name_tb.Left = 10 + name_label.Width + 10;
            name_tb.Height = 12;
            name_tb.Width = 100;

            Label rank_label = new Label();
            rank_label.Parent = this;
            f2_top += f2_step;
            rank_label.Top = f2_top;
            rank_label.Height = 12;
            rank_label.Width = 48;
            f2_top += rank_label.Height;

            rank_label.Text = "rank";

            rank_cb = new ComboBox();
            rank_cb.Parent = this;
            rank_cb.Top = rank_label.Top - 5;
            rank_cb.Left = 10 + rank_label.Width + 10;
            rank_cb.Height = 12;
            rank_cb.Width = 40;

            rank_cb.SelectedIndexChanged += rank_cb_selected_index_changed;

            for (int i = 1; i <= 7; ++i)
                rank_cb.Items.Add(i);

            cb_list = new List<ComboBox>();
            checkbox_list = new List<CheckBox>();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            new_equitment_tb.Text = "";

            ComboBox t_cb = new ComboBox();

            // read file
            using (StreamReader sr = new StreamReader(equipment_file))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    t_cb.Items.Add(line);
                }
            }

            for (int i = 0; i < cb_list.Count; ++i)
            {
                ComboBox cb = cb_list[i];

                for (int j = 0; j < t_cb.Items.Count; ++j)
                {
                    cb.Items.Add(t_cb.Items[j]);
                }

                checkbox_list[i].Checked = false;
            }
        }

        void rank_cb_selected_index_changed(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int left = 0;
            int left_step = 10;
            int temp_f2 = 0;
            
            ComboBox t_cb = new ComboBox();

            // read file
            using (StreamReader sr = new StreamReader(equipment_file))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    t_cb.Items.Add(line);
                }
            }

            for (int i = (int)cb.SelectedItem; i <= 7; ++i)
            {
                int temp_top = f2_top;
                int temp_left = 0;

                for (int j = 0; j < 6; ++j)
                {
                    int t_left = left;

                    ComboBox e_cb = new ComboBox();
                    cb_list.Add(e_cb);
                    e_cb.Parent = this;
                    temp_top += f2_step;
                    e_cb.Top = temp_top;
                    t_left += left_step;
                    e_cb.Left = t_left;
                    //e_cb.Left = left + left_step;
                    e_cb.Height = 12;
                    e_cb.Width = 150;
                    t_left += e_cb.Width;

                    for (int k = 0; k < t_cb.Items.Count; ++k)
                    {
                        e_cb.Items.Add(t_cb.Items[k]);
                    }

                    CheckBox e_checkbox = new CheckBox();
                    checkbox_list.Add(e_checkbox);
                    e_checkbox.Parent = this;
                    e_checkbox.Top = temp_top;
                    t_left += left_step;
                    e_checkbox.Left = t_left;
                    e_checkbox.Width = 15;
                    t_left += e_checkbox.Width;

                    temp_top += e_cb.Height;

                    temp_left = t_left;
                }

                left = temp_left;

                temp_f2 = temp_top;
            }

            f2_top = temp_f2;
        }

        private void new_equitment_but_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(equipment_file, true))
            {
                if (cb_list[0].Items.IndexOf(new_equitment_tb.Text) == -1)
                {
                    sw.WriteLine(new_equitment_tb.Text);
                }
            }

            Form2_Load(null, null);
        }

        private void save_but_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(character_dir))
            {
                Directory.CreateDirectory(character_dir);
            }

            string path = character_dir + "/" + name_tb.Text + ".txt";

            if (!File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(name_tb.Text);

                    int rank = (int)rank_cb.SelectedItem;
                    sw.WriteLine(rank);
                    
                    for(int i = 0; i <= 7 - rank; ++i)
                    {
                        sw.WriteLine('r' + (rank + i).ToString());
                        for (int j = 0; j < 6; ++j)
                        {
                            int idx = i * 6 + j;
                            sw.WriteLine(cb_list[idx].SelectedItem + " " + checkbox_list[idx].Checked);
                        }
                    }
                }
            }
        }

        List<ComboBox> cb_list;
        List<CheckBox> checkbox_list;

        TextBox name_tb;
        ComboBox rank_cb;

        string equipment_file = "equipment.txt";
        string character_dir = "character";

        int f2_top = 0;
        int f2_step = 10;
    }
}
