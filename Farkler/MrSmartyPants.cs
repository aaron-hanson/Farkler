using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Farkler
{
    sealed class UserCommand
    {
        public const string NewGame = "n";
        public const string Roll = "r";
        public const string RandomRoll = "rr";
        public const string Quit = "q";
        public const string ModeAutomatic = "ma";
        public const string ModeManual = "mm";
        public const string AddPlayer = "p";
    }

    enum GameState
    {
        NoGame,
        InGame
    }

    enum AIMode
    {
        Automatic,
        Manual
    }

    enum PlayerType
    {
        AI,
        Human
    }

    class FarklePlayer
    {
        public PlayerType Type;
        public string Name;
        public int BankedScore;

        public FarklePlayer(string name, PlayerType type = PlayerType.Human)
        {
            Name = name;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return Name.Equals((obj as FarklePlayer).Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    
    class MrSmartyPants
    {
        static GameState State = GameState.NoGame;
        static AIMode Mode = AIMode.Manual;

        static LinkedList<FarklePlayer> Players = new LinkedList<FarklePlayer>();

        static int TurnScore;
        static int DiceToRoll = 6;
        static Roll Roll;
        static LinkedListNode<FarklePlayer> PlayerWithTheDice = null;

        public static void Interactive()
        {
            Players.AddLast(new FarklePlayer("MrSmartyPants", PlayerType.AI));

            string cmd = string.Empty, cmdData = string.Empty;
            while (!UserCommand.Quit.Equals(cmd))
            {
                Console.Write("<farkler> $ ");
                var cmdtext = Console.ReadLine().Split('-');
                cmd = cmdtext[0];
                cmdData = cmdtext.Length > 1 ? cmdtext[1] : null;

                switch (cmd)
                {
                    case UserCommand.AddPlayer:
                        if (cmdData == null) Console.WriteLine("ERR - (p) Usage:  p-PlayerName");
                        else
                        {
                            Players.AddLast(new FarklePlayer(cmdData));
                            Console.WriteLine("Added new player {0}, now there are {1} players.", cmdData, Players.Count);
                        }
                        break;
                    case UserCommand.ModeAutomatic:
                        Mode = AIMode.Automatic;
                        Console.WriteLine("Mode set to Automatic");
                        break;
                    case UserCommand.ModeManual:
                        Mode = AIMode.Manual;
                        Console.WriteLine("Mode set to Manual");
                        break;
                    case UserCommand.RandomRoll:
                        Roll = Dice.RandomRoll(DiceToRoll);
                        Console.WriteLine(Roll);
                        break;
                    case UserCommand.Roll:
                        if (cmdData == null || !Regex.IsMatch(cmdData, @"^\d{1,6}$")) Console.WriteLine("ERR - (r) Usage:  r-122245");
                        else
                        {
                            Roll = new Roll(cmdData);
                            Console.WriteLine(Roll);
                        }
                        break;
                    case UserCommand.NewGame:
                        State = GameState.InGame;
                        TurnScore = 0;
                        DiceToRoll = 6;
                        Roll = null;
                        PlayerWithTheDice = Players.First;
                        break;
                    case UserCommand.Quit:
                        State = GameState.NoGame;
                        break;
                    default:
                        break;
                }
                Console.WriteLine();

                if (GameState.NoGame.Equals(State)) continue;

                if (Roll != null)
                {
                    switch (PlayerWithTheDice.Value.Type)
                    {
                        case PlayerType.AI:
                            if (ChooseAndPerformAction())
                            {
                                DiceToRoll = 6;
                                TurnScore = 0;
                                PlayerWithTheDice = PlayerWithTheDice.Next;
                            }
                            break;
                        case PlayerType.Human:

                            break;
                    }
                    Roll = null;
                }

                Console.WriteLine("\n### PLAYER={0}  Dice={1} Score={2} Turn={3} ###", PlayerWithTheDice.Value.Name, DiceToRoll, PlayerWithTheDice.Value.BankedScore, TurnScore);



            } //while
        }

        static bool ChooseAndPerformAction()
        {
            List<Action> actions = Farkle.Gen(Roll);
            if (!actions.Any())
            {
                Console.WriteLine("%%%% I GOT FARKLED!");
                return true;
            }

            double best = TurnScore;
            Action pick = null;
            foreach (var act in actions)
            {
                double ev = ExpectedValueCalc.EV(act.DiceToRoll, TurnScore + act.ScoreToAdd);
                if (ev > best)
                {
                    best = ev;
                    pick = act;
                }
            }

            TurnScore += pick.ScoreToAdd;
            if (best == TurnScore)
            {
                Console.WriteLine("%%%%% I CHOOSE TO BANK {0}", TurnScore);
                PlayerWithTheDice.Value.BankedScore += TurnScore;
                return true;
            }
            else
            {
                Console.WriteLine("%%%%% I CHOOSE TO SCORE {0} AND ROLL {1} DICE", pick.ScoreToAdd, pick.DiceToRoll);
                DiceToRoll = pick.DiceToRoll;
                return false;
            }
        }

        //public static void Play()
        //{
        //    int score = 0;
        //    int dice = 6;
        //    while(true)
        //    {
        //        Console.Write("\nScore = {0}\nRoll : ", score);
                
        //        //string rstr = Console.ReadLine();
        //        //Console.WriteLine();
        //        //Roll roll = new Roll(rstr);

        //        Roll roll = Dice.RandomRoll(dice);
        //        Console.WriteLine(roll);
                
        //        List<Action> actions = Farkle.Gen(roll);

        //        if (actions.Count == 0)
        //        {
        //            Console.WriteLine("FARKLE!");
        //            score = 0;
        //            dice = 6;
        //            Console.ReadLine();
        //            continue;
        //        }

        //        double best = score;
        //        Action pick = null;
        //        foreach (var act in actions)
        //        {
        //            Console.Write("(" + act + ")");
        //            double ev = ExpectedValueCalc.EV(act.DiceToRoll, score + act.ScoreToAdd);
        //            if (ev > best)
        //            {
        //                best = ev;
        //                pick = act;
        //            }
        //            Console.WriteLine(ev);
        //        }

        //        score += pick.ScoreToAdd;
        //        if (best == score)
        //        {
        //            Console.WriteLine(">>> STOP {0} BANK {1}", pick.ScoreToAdd, score);
        //            score = 0;
        //            dice = 6;
        //        }
        //        else
        //        {
        //            dice = pick.DiceToRoll;
        //            Console.WriteLine(">>> {0}", pick);
        //        }
                
        //        Console.ReadLine();
        //    }
        //}


    }
}
