using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace princess_connect_support
{
    class Need
    {
        public Need(string name, int num, int rank = 0, bool have = false)
        {
            Name = name;
            Num = num;
            Rank = rank;
            Have = have;
        }

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public int Rank
        {
            set { rank = value; }
            get { return rank; }
        }

        public int Num
        {
            set { num = value; }
            get { return num; }
        }

        public bool Have
        {
            set { have = value; }
            get { return have; }
        }

        string name;
        int rank, num;
        bool have;
    }
}