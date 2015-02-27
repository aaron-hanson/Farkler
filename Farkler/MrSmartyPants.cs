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
            int bestTurn = 0;
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
                //Console.ReadLine();

                List<Action> actions = Farkle.Gen(roll);

                if (actions.Count == 0)
                {
                    Console.WriteLine("FARKLE!");
                    score = 0;
                    dice = 6;
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
                if (pick == null) Console.WriteLine(">>> BANK --- " + score);
                else
                {
                    if (best == score + pick.ScoreToAdd)
                    {
                        Console.WriteLine(">>> STOP {0} BANK {1}", pick.ScoreToAdd, score + pick.ScoreToAdd);
                        score += pick.ScoreToAdd;
                        if (score > bestTurn) { bestTurn = score; Console.ReadLine(); }
                        score = 0;
                        dice = 6;
                    }
                    else
                    {
                        score += pick.ScoreToAdd;
                        dice = pick.DiceToRoll;
                        Console.WriteLine(">>> {0}", pick);
                    }
                }

            }
        }


    }
}
