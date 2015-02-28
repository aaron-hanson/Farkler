using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            Dictionary<string, double> evcache =
                JsonConvert.DeserializeObject<Dictionary<string, double>>(File.ReadAllText("EVCache.json"));
            ExpectedValueCalc.EVCache = evcache;
            
            Dictionary<string, double> evopeningcache =
                JsonConvert.DeserializeObject<Dictionary<string, double>>(File.ReadAllText("EVOpeningCache.json"));
            ExpectedValueCalc.EVOpeningCache = evopeningcache;

            MrSmartyPants.Interactive();

            //var ev = ExpectedValueCalc.EV(6, 0);
            //Console.WriteLine();
            //Console.WriteLine(ev);
            //Console.WriteLine("EV Cache: " + ExpectedValueCalc.EVCache.Count);
            //Console.WriteLine("Action Cache: " + Farkle.GenCache.Count);
            //Console.WriteLine("Score Cache: " + Farkle.ValidScoreCache.Count);
            //Console.WriteLine("Combo Cache: " + Dice.RollComboCache.Count);


            //string serialized = JsonConvert.SerializeObject(
            //    ExpectedValueCalc.EVCache
            //    .OrderBy(x => x.Key)
            //    .ToDictionary(k => k.Key, v => v.Value));
            //File.WriteAllText("EVCache.json", serialized);
        }

    }
}
