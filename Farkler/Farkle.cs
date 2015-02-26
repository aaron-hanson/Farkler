using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    enum ActionLevel
    {
        None,
        Singles,
        Triples
    }

    class Farkle
    {
        public static List<Action> Gen(Roll roll)
        {
            var actions = new List<Action>();



            return actions;
        }

        public static int? ValidScore(Roll roll)
        {
            if (roll.Count == 6 && roll.Distinct<int>().Count() == 6) return 1500;

            var triples = roll.GroupBy(x => x).Where(x => x.Count() == 3);

            

            return null;
        }

        public static List<Action> GenerateActions(Roll roll, Action baseAction = null, ActionLevel level = ActionLevel.Triples, int numOnes = 0, int numFives = 0)
        {
            var actions = new List<Action>();
            var dice = roll.Count();

            // hot dice!
            if (roll.Count == 6 && roll.Distinct<int>().Count() == 6) actions.Add(new Action(1500, 0));

            //triples
            if (level >= ActionLevel.Triples)
            {
                var triples = roll.GroupBy(x => x).Where(x => x.Count() >= 3);
                foreach (var triple in triples)
                {
                    int score = triple.Key == 1 ? 1000 : triple.Key * 100;
                    var act = new Action(score, dice - 3).Combine(baseAction);
                    actions.Add(act);
                    actions.AddRange(GenerateActions(roll.Narrow(triple.Take(3).ToList()), act, ActionLevel.Triples, numOnes, numFives));
                }
            }

            // singles
            if (level >= ActionLevel.Singles)
            {
                if (numOnes < 2 && roll.Any(x => x == 1))
                {
                    var act = new Action(100, dice - 1).Combine(baseAction);
                    actions.Add(act);
                    actions.AddRange(GenerateActions(roll.Narrow(1), act, ActionLevel.Singles, numOnes + 1, numFives));
                }
                if (numFives < 2 && roll.Any(x => x == 5))
                {
                    var act = new Action(50, dice - 1).Combine(baseAction);
                    actions.Add(act);
                    actions.AddRange(GenerateActions(roll.Narrow(5), act, ActionLevel.Singles, numOnes, numFives + 1));
                }
            }

            return actions;
        }


    }
}
