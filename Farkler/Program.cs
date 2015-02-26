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
            Console.WriteLine("Farkler!");
            var x = ExpectedValueCalc.RollPermutations(1);

            //Console.WriteLine(ExpectedValueCalc.EV(6, 0));
            //Console.WriteLine(ExpectedValueCalc.EV(2, 50));
            Console.ReadLine();
        }
    }
}
