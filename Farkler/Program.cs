using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class Program
    {
        static void Main(string[] args)
        {
            Program instance = new Program();
            instance.go();
        }

        public Program()
        {

        }

        public void go()
        {
            //MrSmartyPants.GenerateTurnHistogram();
            MrSmartyPants.Interactive();
        }

    }
}
