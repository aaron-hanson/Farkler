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
        public const string Bank = "b";
        public const string Action = "a";
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
        static List<Action> ActionsPossible = new List<Action>();
        static Action ActionPicked;
        static LinkedListNode<FarklePlayer> PlayerWithTheDice;

        public static void Interactive()
        {
            Players.AddLast(new FarklePlayer("MrSmartyPants", PlayerType.AI));

            string cmd = string.Empty, cmdData, cmdData2;
            while (!UserCommand.Quit.Equals(cmd))
            {
                Console.Write("<farkler> $ ");
                var cmdtext = Console.ReadLine().Split(' ');
                cmd = cmdtext[0];
                cmdData = cmdtext.Length > 1 ? cmdtext[1] : null;
                cmdData2 = cmdtext.Length > 2 ? cmdtext[2] : null;

                switch (cmd)
                {
                    case UserCommand.AddPlayer:
                        if (State == GameState.InGame) Console.WriteLine("ERR - cannot add a player while a game is in progress.");
                        else if (cmdData == null) Console.WriteLine("ERR - (p) Usage:  p-PlayerName");
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
                        if (Roll != null) Console.WriteLine("ERR - cannot Roll until current roll is acted upon.");
                        else
                        {
                            Roll = Dice.RandomRoll(DiceToRoll);
                            ActionsPossible = Farkle.Gen(Roll);
                            Console.WriteLine(Roll);
                        }
                        break;
                    case UserCommand.Roll:
                        if (Roll != null) Console.WriteLine("ERR - cannot Roll until current roll is acted upon.");
                        else if (cmdData == null || !Regex.IsMatch(cmdData, @"^\d{" + DiceToRoll + "}$")) Console.WriteLine("ERR - (r) Usage:  r-122245");
                        else
                        {
                            Roll = new Roll(cmdData);
                            ActionsPossible = Farkle.Gen(Roll);
                            Console.WriteLine(Roll);
                        }
                        break;
                    case UserCommand.Action:
                        if (PlayerWithTheDice.Value.Type != PlayerType.Human) Console.WriteLine("ERR - cannot perform Action, player is not human.");
                        else if (Roll == null) Console.WriteLine("ERR - cannot perform Action, there is no roll on the table.");
                        else if (cmdData == null) Console.WriteLine("ERR - (a) Usage:  a-400-3 (a-points-dicetoroll)");
                        else
                        {
                            int points;
                            if (!int.TryParse(cmdData, out points)) { Console.WriteLine("ERR - (a) Usage:  a-400-3 (a-points-dicetoroll)"); break; }
                            if (!ActionsPossible.Any(x => x.ScoreToAdd == points)) { Console.WriteLine("ERR - no matching possible action."); break; }

                            if (cmdData2 == null)
                            {
                                if (TurnScore + points < 300) { Console.WriteLine("ERR - cannot bank less than 300 points."); break; }
                                TurnScore += points;
                                PlayerWithTheDice.Value.BankedScore += TurnScore;
                                Console.WriteLine("Banked {0}, Score for {1} is now {2}", TurnScore, PlayerWithTheDice.Value.Name, PlayerWithTheDice.Value.BankedScore);
                                EndTurn();
                            }
                            else
                            {
                                int dice;
                                if (!int.TryParse(cmdData2, out dice)) { Console.WriteLine("ERR - (a) Usage:  a-400-3 (a-points-dicetoroll)"); break; }
                                if (!ActionsPossible.Any(x => x.ScoreToAdd == points && x.DiceToRoll == dice)) { Console.WriteLine("ERR - no matching possible action."); break; }

                                TurnScore += points;
                                DiceToRoll = dice;
                                Roll = Dice.RandomRoll(DiceToRoll);
                                ActionsPossible = Farkle.Gen(Roll);
                                Console.WriteLine("Stashed {0} and rolling {1} dice.", points, dice);
                                Console.WriteLine(Roll);
                            }
                        }
                        break;
                    case UserCommand.NewGame:
                        State = GameState.InGame;
                        TurnScore = 0;
                        DiceToRoll = 6;
                        Roll = null;
                        ActionsPossible.Clear();
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
                    bool turnIsOver = false;
                    switch (PlayerWithTheDice.Value.Type)
                    {
                        case PlayerType.AI:
                            turnIsOver = ChooseAndPerformAction();
                            Roll = null;
                            ActionsPossible.Clear();
                            break;
                        case PlayerType.Human:
                            turnIsOver = false;
                            if (!ActionsPossible.Any())
                            {
                                Console.WriteLine("{0} GOT FARKLED!", PlayerWithTheDice.Value.Name);
                                turnIsOver = true;
                            }

                            break;
                    }

                    if (turnIsOver) EndTurn();
                }

                Console.WriteLine("\n### PLAYER={0}  Dice={1} Score={2} Turn={3} ###", PlayerWithTheDice.Value.Name, DiceToRoll, PlayerWithTheDice.Value.BankedScore, TurnScore);
                if (Roll != null) Console.WriteLine(Roll);



            } //while
        }

        static void EndTurn()
        {
            DiceToRoll = 6;
            TurnScore = 0;
            Roll = null;
            ActionsPossible.Clear();
            PlayerWithTheDice = PlayerWithTheDice.Next ?? Players.First;
        }

        static bool ChooseAndPerformAction()
        {
            if (!ActionsPossible.Any())
            {
                Console.WriteLine("%%%% I GOT FARKLED!");
                return true;
            }

            double best = TurnScore;
            Action pick = null;
            foreach (var act in ActionsPossible)
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
                Console.WriteLine("%%%%% I CHOOSE TO STASH {0} AND ROLL {1} DICE", pick.ScoreToAdd, pick.DiceToRoll);
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
