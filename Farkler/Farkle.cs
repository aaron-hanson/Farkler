using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class Farkle
    {
        public static ICollection<Action> GetActions(Roll roll)
        {
            Collection<Action> actions = new Collection<Action>();
            var dice = roll.Count();

            var triples = roll.GroupBy(x => x).Where(x => x.Count() >= 3);
            var numOnes = roll.Count(x => x == 1);
            var numFives = roll.Count(x => x == 5);

            bool hasOnes = numOnes > 0;
            bool hasFives = numFives > 0;
            
            bool hasTriples = triples.Any();

            // farkled?
            if (!hasOnes && !hasFives && !hasTriples) return actions;

            // hot dice?
            if (roll.Count == 6 && roll.Distinct<int>().Count() == 6) actions.Add(new Action(1500, 6));

            //ways to keep one die
            if (hasOnes) actions.Add(new Action(100, dice - 1));
            if (hasFives) actions.Add(new Action(50, dice - 1));

            //ways to keep two dice
            if (numOnes >= 2) actions.Add(new Action(200, dice - 2));
            if (numFives >= 2) actions.Add(new Action(100, dice - 2));
            if (hasOnes && hasFives) actions.Add(new Action(150, dice - 2));

            //ways to keep three dice
            foreach (var triple in triples)
            {
                int score = triple.Key == 1 ? 1000 : triple.Key * 100;
                actions.Add(new Action(score, dice - 3));
                if (triple.Count() == 6) actions.Add(new Action(score * 2, 6));
            }

            return actions;
        }

    }
}
