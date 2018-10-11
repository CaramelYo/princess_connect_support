using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace princess_connect_support
{
    class Character
    {
        public Character(string name)
        {
            this.name = name;
        }

        public Character(StreamReader sr)
        {
            name = sr.ReadLine();

            total_needs = new List<Need>();
            needs = new List<Need>();
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] line_split = line.Split(' ');
                //string n = line_split[0];
                //int num = int.Parse(line_split[1]), rank = int.Parse(line_split[2]);
                //bool is_need = bool.Parse(line_split[3]);
                //Need need = new Need(n, num, rank, is_need);

                //needs.Add(new Need(line_split[0], int.Parse(line_split[1]), int.Parse(line_split[2]), bool.Parse(line_split[3])));

                Need need = new Need(line_split[0], int.Parse(line_split[1]), int.Parse(line_split[2]), bool.Parse(line_split[3]));
                
                total_needs.Add(need);

                if (!need.Have)
                    needs.Add(need);
            }
        }

        public List<Need> Total_needs
        {
            set { total_needs = value; }
            get { return total_needs; }
        }

        public List<Need> Needs
        {
            set { needs = value;}
            get { return needs; }
        }

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        List<Need> total_needs, needs;
        string name;
    }
}
