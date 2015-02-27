using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class MrSmartyPants
    {
        public static void Play()
        {
            int score = 0;
            int dice = 6;
            while(true)
            {
                Console.Write("\nScore = {0}\nRoll : ", score);
                
                //string rstr = Console.ReadLine();
                //Console.WriteLine();
                //Roll roll = new Roll(rstr);

                Roll roll = Dice.RandomRoll(dice);
                Console.WriteLine(roll);
                
                List<Action> actions = Farkle.Gen(roll);

                if (actions.Count == 0)
                {
                    Console.WriteLine("FARKLE!");
                    score = 0;
                    dice = 6;
                    Console.ReadLine();
                    continue;
                }

                double best = score;
                Action pick = null;
                foreach (var act in actions)
                {
                    Console.Write("(" + act + ")");
                    double ev = ExpectedValueCalc.EV(act.DiceToRoll, score + act.ScoreToAdd);
                    if (ev > best)
                    {
                        best = ev;
                        pick = act;
                    }
                    Console.WriteLine(ev);
                }

                score += pick.ScoreToAdd;
                if (best == score)
                {
                    Console.WriteLine(">>> STOP {0} BANK {1}", pick.ScoreToAdd, score);
                    score = 0;
                    dice = 6;
                }
                else
                {
                    dice = pick.DiceToRoll;
                    Console.WriteLine(">>> {0}", pick);
                }
                
                Console.ReadLine();
            }
        }


    }
}
