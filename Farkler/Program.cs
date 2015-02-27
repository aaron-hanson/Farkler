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

            MrSmartyPants.Play();

            var ev = ExpectedValueCalc.EV(6, 0);
            Console.WriteLine();
            Console.WriteLine(ev);
            Console.WriteLine("EV Cache: " + ExpectedValueCalc.EVCache.Count);
            Console.WriteLine("Action Cache: " + Farkle.GenCache.Count);
            Console.WriteLine("Score Cache: " + Farkle.ValidScoreCache.Count);
            Console.WriteLine("Combo Cache: " + Dice.RollComboCache.Count);

            //var acts = Farkle.Gen(new Roll { 1 });

            //var actions = Farkle.GenerateActions(new Roll { 1, 1, 1, 1, 2, 3 }).Distinct();

            //Console.WriteLine(ExpectedValueCalc.EV(6, 0));
            //Console.WriteLine(ExpectedValueCalc.EV(2, 50));
            Console.ReadLine();
        }
    }
}
