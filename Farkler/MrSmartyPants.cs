using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Farkler
{
    sealed class UserCommand
    {
        public const string Reset = "$";
        public const string NewGame = "n";
        public const string Roll = "r";
        public const string RandomRoll = "rr";
        public const string Quit = "q";
        public const string ModeAutomatic = "ma";
        public const string ModeManual = "mm";
        public const string AddPlayer = "p";
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
        static GameState State;
        static AIMode Mode;

        static LinkedList<FarklePlayer> Players;

        static int TurnScore;
        static int DiceToRoll;
        static Roll Roll;
        static List<FarkleAction> ActionsPossible;
        static LinkedListNode<FarklePlayer> PlayerWithTheDice;

        static void Reset()
        {
            State = GameState.NoGame;
            Mode = AIMode.Manual;
            TurnScore = 0;
            DiceToRoll = 6;
            Roll = null;
            ActionsPossible = new List<FarkleAction>();
            Players = new LinkedList<FarklePlayer>();
            Players.AddLast(new FarklePlayer("MrSmartyPants", PlayerType.AI));
        }

        public static void Interactive()
        {
            Reset();

            string cmd = string.Empty, cmdData = null, cmdData2 = null;
            while (!UserCommand.Quit.Equals(cmd))
            {
                if (Mode == AIMode.Manual || State == GameState.NoGame || PlayerWithTheDice.Value.Type == PlayerType.Human)
                {
                    Console.Write("<farkler> $ ");
                    var cmdtext = Console.ReadLine().Replace("  ", " ").Trim().Split(' ');
                    cmd = cmdtext[0];
                    cmdData = cmdtext.Length > 1 ? cmdtext[1] : null;
                    cmdData2 = cmdtext.Length > 2 ? cmdtext[2] : null;
                }
                else
                {
                    RandomRoll();
                    Console.WriteLine(Roll);
                    cmd = string.Empty;
                    Thread.Sleep(2000);
                }

                switch (cmd)
                {
                    case UserCommand.Reset:
                        Reset();
                        break;
                    case UserCommand.AddPlayer:
                        if (State == GameState.InGame) Console.WriteLine("ERR - cannot add a player while a game is in progress.");
                        else if (cmdData == null) Console.WriteLine("ERR - (p) Usage:  p-PlayerName");
                        else if (Players.Any(x => x.Name.Equals(cmdData))) Console.WriteLine("ERR - player with that name already exists.");
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
                            RandomRoll();
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
                                Console.WriteLine("Stashed {0} and rolling {1} dice.", points, dice);
                                RandomRoll();
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
                        Players.ToList().ForEach(x => x.BankedScore = 0);
                        PlayerWithTheDice = Players.First;
                        break;
                    case UserCommand.Quit:
                        State = GameState.NoGame;
                        break;
                    default:
                        break;
                }
                
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

                Console.WriteLine();
                foreach (FarklePlayer p in Players)
                {
                    Console.WriteLine("{0}{1}{2}", 
                        p.Name.PadLeft(15).PadRight(16),
                        p.BankedScore.ToString().PadRight(7), 
                        (PlayerWithTheDice.Value.Equals(p) ? TurnScore.ToString() + " [" + DiceToRoll + "d]" : ""));    
                }
                if (Roll != null) Console.WriteLine(Roll);



            } //while
        }

        static void RandomRoll()
        {
            Roll = Dice.RandomRoll(DiceToRoll);
            ActionsPossible = Farkle.Gen(Roll);
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
            FarkleAction pick = null;
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

    }
}
