using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace princess_connect_support
{
    class Item
    {
        public Item(string name)
        {
            Name = name;
        }

        public List<Need> Needs
        {
            set { needs = value; }
            get { return needs; }
        }

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        List<Need> needs;

        string name;
    }
}
