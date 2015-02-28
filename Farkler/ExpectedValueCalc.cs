using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class ExpectedValueCalc
    {
        public static int WinScore { get; set; }
        public static int MinBank { get; set; }
        public static int MinOpen { get; set; }
        public static SortedDictionary<string, double> EVCache;
        public static SortedDictionary<string, double> EVOpeningCache;

        static ExpectedValueCalc()
        {
            WinScore = 10000;
            MinBank = 300;
            MinOpen = 500;

            SortedDictionary<string, double> evcache =
                JsonConvert.DeserializeObject<SortedDictionary<string, double>>(File.ReadAllText("EVCache.json"));
            ExpectedValueCalc.EVCache = evcache;

            SortedDictionary<string, double> evopeningcache =
                JsonConvert.DeserializeObject<SortedDictionary<string, double>>(File.ReadAllText("EVOpeningCache.json"));
            ExpectedValueCalc.EVOpeningCache = evopeningcache;
        }

        public static double EV(int d, double p)
        {
            double ev;
            string key = d + ":" + p;
            if (EVCache.TryGetValue(key, out ev)) return ev;

            if (p >= WinScore)
            {
                EVCache.Add(key, p);
                return p;
            }

            ev = Dice.Permute[d]
                .Average(x => Farkle.GenerateActions(x)
                    .Max(y => (double?)EV(y.DiceToRoll, p + y.ScoreToAdd)) ?? 0D);

            ev = p < MinBank ? ev : Math.Max(ev, p);
            EVCache.Add(key, ev);
            Console.Write('.');
            return ev;
        }

        public static double EVOpening(int d, double p)
        {
            double ev;
            string key = d + ":" + p;
            if (EVOpeningCache.TryGetValue(key, out ev)) return ev;

            if (p >= MinOpen || p >= WinScore)
            {
                EVOpeningCache.Add(key, p);
                return p;
            }

            ev = Dice.Permute[d]
                .Average(x => Farkle.GenerateActions(x)
                    .Max(y => (double?)EVOpening(y.DiceToRoll, p + y.ScoreToAdd)) ?? 0D);

            ev = (p < MinBank || p < MinOpen) ? ev : Math.Max(ev, p);
            EVOpeningCache.Add(key, ev);
            Console.Write('.');
            return ev;
        }

        public static double ChanceToWin(int d, double p)
        {
            double ev;
            string key = d + ":" + p;
            if (EVCache.TryGetValue(key, out ev)) return ev;

            if (p >= WinScore && p >= MinBank)
            {
                EVCache.Add(key, 1);
                return 1;
            }

            ev = Dice.Permute[d]
                .Average(x => Farkle.GenerateActions(x)
                    .Max(y => (double?)ChanceToWin(y.DiceToRoll, p + y.ScoreToAdd)) ?? 0D);

            EVCache.Add(key, ev);
            Console.Write('.');
            return ev;
        }

        public static void RewriteEVCache()
        {
            EVCache.Clear();
            var ev = EV(6, 0);
            Console.WriteLine();
            Console.WriteLine(ev);
            Console.WriteLine("EV Cache: " + EVCache.Count);
            Console.WriteLine("Action Cache: " + Farkle.GenCache.Count);
            Console.WriteLine("Score Cache: " + Farkle.ValidScoreCache.Count);
            Console.WriteLine("Combo Cache: " + Dice.RollComboCache.Count);

            string serialized = JsonConvert.SerializeObject(ExpectedValueCalc.EVCache);
            File.WriteAllText("EVCache.json", serialized);
        }

        public static void RewriteEVOpeningCache()
        {
            EVOpeningCache.Clear();
            var ev = EVOpening(6, 0);
            Console.WriteLine();
            Console.WriteLine(ev);
            Console.WriteLine("EV Opening Cache: " + EVOpeningCache.Count);
            Console.WriteLine("Action Cache: " + Farkle.GenCache.Count);
            Console.WriteLine("Score Cache: " + Farkle.ValidScoreCache.Count);
            Console.WriteLine("Combo Cache: " + Dice.RollComboCache.Count);

            string serialized = JsonConvert.SerializeObject(ExpectedValueCalc.EVOpeningCache);
            File.WriteAllText("EVOpeningCache.json", serialized);
        }

    }
}
