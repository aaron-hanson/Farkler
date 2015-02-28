using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public const string AddPlayerAI = "pa";
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
        public int TurnsTaken;

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
        public static bool Quiet = false;

        static GameState State;
        static AIMode Mode;

        static LinkedList<FarklePlayer> Players;

        static int TurnScore;
        static int DiceToRoll;
        static Roll Roll;
        static List<FarkleAction> ActionsPossible;
        static LinkedListNode<FarklePlayer> PlayerWithTheDice;

        public static void Interactive()
        {
            Reset();

            string cmd = string.Empty, cmdData = null, cmdData2 = null;
            while (!UserCommand.Quit.Equals(cmd))
            {
                if (Mode == AIMode.Manual || State == GameState.NoGame || PlayerWithTheDice.Value.Type == PlayerType.Human)
                {
                    Write("<farkler> $ ");
                    var cmdtext = Console.ReadLine().Replace("  ", " ").Trim().Split(' ');
                    cmd = cmdtext[0];
                    cmdData = cmdtext.Length > 1 ? cmdtext[1] : null;
                    cmdData2 = cmdtext.Length > 2 ? cmdtext[2] : null;
                }
                else
                {
                    RandomRoll();
                    WriteLine(Roll);
                    cmd = string.Empty;
                    //Thread.Sleep(2000);
                }

                switch (cmd)
                {
                    case UserCommand.Reset:
                        Reset();
                        break;
                    case UserCommand.AddPlayer:
                        if (State == GameState.InGame) WriteLine("ERR - cannot add a player while a game is in progress.");
                        else if (cmdData == null) WriteLine("ERR - (p) Usage:  p-PlayerName");
                        else if (Players.Any(x => x.Name.Equals(cmdData))) WriteLine("ERR - player with that name already exists.");
                        else
                        {
                            Players.AddLast(new FarklePlayer(cmdData));
                            WriteLine("Added new player {0}, now there are {1} players.", cmdData, Players.Count);
                        }
                        break;
                    case UserCommand.AddPlayerAI:
                        if (State == GameState.InGame) WriteLine("ERR - cannot add a player while a game is in progress.");
                        else if (cmdData == null) WriteLine("ERR - (p) Usage:  p-PlayerName");
                        else if (Players.Any(x => x.Name.Equals(cmdData))) WriteLine("ERR - player with that name already exists.");
                        else
                        {
                            Players.AddLast(new FarklePlayer(cmdData, PlayerType.AI));
                            WriteLine("Added new AI player {0}, now there are {1} players.", cmdData, Players.Count);
                        }
                        break;
                    case UserCommand.ModeAutomatic:
                        Mode = AIMode.Automatic;
                        WriteLine("Mode set to Automatic");
                        break;
                    case UserCommand.ModeManual:
                        Mode = AIMode.Manual;
                        WriteLine("Mode set to Manual");
                        break;
                    case UserCommand.RandomRoll:
                        if (Roll != null) WriteLine("ERR - cannot Roll until current roll is acted upon.");
                        else
                        {
                            RandomRoll();
                            WriteLine(Roll);
                        }
                        break;
                    case UserCommand.Roll:
                        if (Roll != null) WriteLine("ERR - cannot Roll until current roll is acted upon.");
                        else if (cmdData == null || !Regex.IsMatch(cmdData, @"^\d{" + DiceToRoll + "}$")) WriteLine("ERR - (r) Usage:  r-122245");
                        else
                        {
                            Roll = new Roll(cmdData);
                            ActionsPossible = Farkle.GenerateActions(Roll);
                            WriteLine(Roll);
                        }
                        break;
                    case UserCommand.Action:
                        if (PlayerWithTheDice.Value.Type != PlayerType.Human) WriteLine("ERR - cannot perform Action, player is not human.");
                        else if (Roll == null) WriteLine("ERR - cannot perform Action, there is no roll on the table.");
                        else if (cmdData == null) WriteLine("ERR - (a) Usage:  a-400-3 (a-points-dicetoroll)");
                        else
                        {
                            int points;
                            if (!int.TryParse(cmdData, out points)) { WriteLine("ERR - (a) Usage:  a-400-3 (a-points-dicetoroll)"); break; }
                            if (!ActionsPossible.Any(x => x.ScoreToAdd == points)) { WriteLine("ERR - no matching possible action."); break; }

                            if (cmdData2 == null)
                            {
                                if (TurnScore + points < 300) { WriteLine("ERR - cannot bank less than 300 points."); break; }
                                TurnScore += points;
                                PlayerWithTheDice.Value.BankedScore += TurnScore;
                                WriteLine("{0} BANKS {1}", CurrentPlayerInfo(), TurnScore);
                                EndTurn();
                            }
                            else
                            {
                                int dice;
                                if (!int.TryParse(cmdData2, out dice)) { WriteLine("ERR - (a) Usage:  a-400-3 (a-points-dicetoroll)"); break; }
                                if (!ActionsPossible.Any(x => x.ScoreToAdd == points && x.DiceToRoll == dice)) { WriteLine("ERR - no matching possible action."); break; }

                                TurnScore += points;
                                DiceToRoll = dice;
                                WriteLine("{0} STASHES {1} and ROLLS {2}", CurrentPlayerInfo(), points, dice);
                                RandomRoll();
                                WriteLine(Roll);
                            }
                        }
                        break;
                    case UserCommand.NewGame:
                        NewGame();
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
                                TurnScore = 0;
                                WriteLine("{0} BUSTED!", CurrentPlayerInfo());
                                turnIsOver = true;
                            }

                            break;
                    }

                    if (turnIsOver) EndTurn();
                }

                FarklePlayer winner = Players.FirstOrDefault(x => x.BankedScore >= ExpectedValueCalc.WinScore);
                if (winner != null)
                {
                    WriteLine("\n\n---------- {0} WINS! ----------", winner.Name);
                    WriteLine("{0} Turns for an average of {1} points per turn.", winner.TurnsTaken, (double)winner.BankedScore / winner.TurnsTaken);
                    NewGame();
                    State = GameState.NoGame;
                }

            } //while
        }

        static void RandomRoll()
        {
            Roll = Dice.RandomRoll(DiceToRoll);
            ActionsPossible = Farkle.GenerateActions(Roll);
        }

        static void EndTurn()
        {
            DiceToRoll = 6;
            TurnScore = 0;
            Roll = null;
            ActionsPossible.Clear();
            PlayerWithTheDice.Value.TurnsTaken++;
            PlayerWithTheDice = PlayerWithTheDice.Next ?? Players.First;

            WriteLine();
            WriteLine("===============================================================================");
            foreach (FarklePlayer p in Players)
            {
                WriteLine("{0}{1}{2}",
                    p.Name.PadLeft(15).PadRight(16),
                    p.BankedScore.ToString().PadLeft(5).PadRight(7),
                    (PlayerWithTheDice.Value.Equals(p) ? " [" + TurnScore.ToString() + " " + DiceToRoll + "d]" : ""));
            }
            WriteLine("...............................................................................");
            if (Roll != null) WriteLine(Roll);
        }

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

        static void NewGame()
        {
            State = GameState.InGame;
            TurnScore = 0;
            DiceToRoll = 6;
            Roll = null;
            ActionsPossible.Clear();
            Players.ToList().ForEach(x => { x.BankedScore = 0; x.TurnsTaken = 0; });
            PlayerWithTheDice = Players.First;
        }

        static string CurrentPlayerInfo()
        {
            return string.Format("{0} [ {1} ] [{2} {3}d]", PlayerWithTheDice.Value.Name, PlayerWithTheDice.Value.BankedScore, TurnScore, DiceToRoll);
        }

        static bool ChooseAndPerformAction()
        {
            bool tryingToOpen = PlayerWithTheDice.Value.BankedScore == 0;
            if (!ActionsPossible.Any())
            {
                TurnScore = 0;
                WriteLine("{0} BUSTED!", CurrentPlayerInfo());
                return true;
            }

            double best = 0;
            FarkleAction pick = null;
            foreach (var act in ActionsPossible.OrderByDescending(x => x.ScoreToAdd))
            {
                var potentialNewScore = TurnScore + act.ScoreToAdd;
                if (potentialNewScore >= ExpectedValueCalc.MinBank && PlayerWithTheDice.Value.BankedScore + potentialNewScore >= ExpectedValueCalc.WinScore)
                {
                    pick = act;
                    best = potentialNewScore;
                    break;
                }

                double ev = tryingToOpen ?
                    ExpectedValueCalc.EVOpening(act.DiceToRoll, potentialNewScore)
                    : ExpectedValueCalc.EV(act.DiceToRoll, potentialNewScore);
                if (ev >= best)
                {
                    best = ev;
                    pick = act;
                }
            }

            TurnScore += pick.ScoreToAdd;
            if ((tryingToOpen && TurnScore >= ExpectedValueCalc.MinOpen) || (!tryingToOpen && best == TurnScore))
            {
                PlayerWithTheDice.Value.BankedScore += TurnScore;
                WriteLine("{0} BANKS {1}", CurrentPlayerInfo(), TurnScore);
                return true;
            }
            else
            {
                DiceToRoll = pick.DiceToRoll;
                WriteLine("{0} STASHES {1} and ROLLS {2}", CurrentPlayerInfo(), pick.ScoreToAdd, pick.DiceToRoll);
                return false;
            }
        }

        public static void GenerateTurnHistogram()
        {
            int count = 0;
            Quiet = true;
            bool turnIsOver = false;
            Reset();
            NewGame();

            SortedDictionary<int, double> Histogram = new SortedDictionary<int, double>();

            while (count < 100000)
            {
                RandomRoll();
                turnIsOver = ChooseAndPerformAction();
                Roll = null;
                ActionsPossible.Clear();
                if (turnIsOver)
                {
                    if (Histogram.ContainsKey(TurnScore)) Histogram[TurnScore]++;
                    else Histogram[TurnScore] = 1;
                    EndTurn();
                    count++;
                    if (count % 10000 == 0) Console.WriteLine(count);
                }
            }

            Histogram.Keys.ToList().ForEach(x => Histogram[x] /= count);

            string serialized = JsonConvert.SerializeObject(Histogram);
            File.WriteAllText("TurnHistogram.json", serialized);
        }

        public static double ExpectedTurnsLeft(double score)
        {
            return (ExpectedValueCalc.WinScore - score) / 469.88274854547706;
        }

        public static void Write(object s)
        {
            if (!Quiet) Console.Write(s);
        }

        public static void Write(string s, params object[] stuff)
        {
            if (!Quiet) Console.Write(s, stuff);
        }

        public static void WriteLine()
        {
            if (!Quiet) Console.WriteLine();
        }

        public static void WriteLine(object s)
        {
            if (!Quiet) Console.WriteLine(s);
        }

        public static void WriteLine(string s, params object[] stuff)
        {
            if (!Quiet) Console.WriteLine(s, stuff);
        }

    }
}
