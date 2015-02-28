using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static Dictionary<string, double> EVCache;
        public static Dictionary<string, double> EVOpeningCache;

        static ExpectedValueCalc()
        {
            WinScore = 10000;
            MinBank = 300;
            MinOpen = 500;
            EVCache = new Dictionary<string, double>();
            EVOpeningCache = new Dictionary<string, double>();
        }

        public static double EV(int d, double p)
        {
            if (p >= WinScore) return p;

            double ev;
            string key = d + ":" + p;
            if (EVCache.TryGetValue(key, out ev)) return ev;

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
            if (p >= WinScore) return p;

            double ev;
            string key = d + ":" + p;
            if (EVOpeningCache.TryGetValue(key, out ev)) return ev;

            ev = Dice.Permute[d]
                .Average(x => Farkle.GenerateActions(x)
                    .Max(y => (double?)(
                        (p + y.ScoreToAdd) >= MinOpen
                            ? p + y.ScoreToAdd
                            : EVOpening(y.DiceToRoll, p + y.ScoreToAdd))) ?? 0D);

            ev = (p < MinBank || p < MinOpen) ? ev : Math.Max(ev, p);
            EVOpeningCache.Add(key, ev);
            Console.Write('.');
            return ev;
        }

    }
}
