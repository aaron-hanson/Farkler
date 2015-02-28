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
        public static Dictionary<string, double> EVCache = new Dictionary<string, double>();

        public static double EV(int d, double p)
        {
            if (p >= 10000) return p;

            double ev;
            string key = string.Format("{0}:{1}", d, p);
            if (EVCache.TryGetValue(key, out ev)) return ev;

            ev = Dice.Permute[d]
                .Average(x => Farkle.GenerateActions(x)
                    .Max(y => (double?)EV(y.DiceToRoll, p + y.ScoreToAdd)) ?? 0D);

            ev = p < 300 ? ev : Math.Max(ev, p);
            EVCache.Add(key, ev);
            Console.Write('.');
            return ev;
        }

    }
}
