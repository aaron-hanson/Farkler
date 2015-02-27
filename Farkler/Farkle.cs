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
        public static Dictionary<Roll, List<Action>> GenCache = new Dictionary<Roll, List<Action>>();
        public static List<Action> Gen(Roll roll)
        {
            List<Action> actions;
            if (GenCache.TryGetValue(roll, out actions)) return actions;

            actions = new List<Action>();
            
            var dice = roll.Count();

            for (int subdice = 1, diceToRoll = dice - subdice; subdice <= dice; subdice++, diceToRoll--)
            {
                var comboss = Dice.RollCombinations(subdice, roll);
                var combos = comboss.Distinct();
                foreach (var subroll in combos)
                {
                    int score;
                    if (!ValidScoreCache.TryGetValue(subroll, out score))
                    {
                        Roll subrollcopy = new Roll(subroll);
                        score = ValidScore(subrollcopy);
                        ValidScoreCache.Add(subroll, score);
                    }
                    if (score > 0)
                    {
                        actions.Add(new Action(score, diceToRoll == 0 ? 6 : diceToRoll));
                    }
                }
            }

            GenCache.Add(roll, actions);
            return actions;
        }

        public static Dictionary<Roll, int> ValidScoreCache = new Dictionary<Roll, int>();
        public static int ValidScore(Roll roll)
        {
            if (roll.Count == 6 && roll.Distinct<int>().Count() == 6) return 1500;

            var score = 0;

            if (roll.GroupBy(x => x).Where(x => x.Count() == 6).Any())
            {
                return roll[0] * (roll[0] == 1 ? 2000 : 200);
            }
           
            foreach (var triple in roll.GroupBy(x => x).Where(x => x.Count() >= 3))
            {
                triple.Take(3).ToList().ForEach(x => roll.Remove(x));
                score += triple.Key * (triple.Key == 1 ? 1000 : 100);
            }

            roll.Where(x => x == 1).ToList().ForEach(x => { roll.Remove(x); score += 100; });
            roll.Where(x => x == 5).ToList().ForEach(x => { roll.Remove(x); score += 50; });

            return roll.Any() ? 0 : score;
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
