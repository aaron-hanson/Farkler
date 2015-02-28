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
        public static Dictionary<Roll, List<FarkleAction>> GenCache = new Dictionary<Roll, List<FarkleAction>>();
        public static List<FarkleAction> Gen(Roll roll)
        {
            List<FarkleAction> actions;
            if (GenCache.TryGetValue(roll, out actions))
            {
                return new List<FarkleAction>(actions);
            }

            actions = new List<FarkleAction>();
            
            var dice = roll.Count();

            for (int subdice = 1, diceToRoll = dice - subdice; subdice <= dice; subdice++, diceToRoll--)
            {
                var combos = Dice.RollCombinations(subdice, roll);
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
                        actions.Add(new FarkleAction(score, diceToRoll == 0 ? 6 : diceToRoll));
                    }
                }
            }

            GenCache.Add(roll, actions);
            return new List<FarkleAction>(actions);
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

    }
}
