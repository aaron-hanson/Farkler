using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class Roll : List<int>, IEquatable<Roll>
    {
        public Roll() : base() { }
        public Roll(Roll roll) : base(roll) { }

        public Roll Narrow(int remove)
        {
            Roll newRoll = new Roll(this);
            newRoll.Remove(remove);
            return newRoll;
        }

        public Roll Narrow(List<int> remove)
        {
            Roll newRoll = new Roll(this);
            remove.ForEach(x => newRoll.Remove(x));
            return newRoll;
        }

        public bool Equals(Roll other)
        {
            return this.OrderBy(x => x).SequenceEqual(other.OrderBy(x => x));
        }

        public override int GetHashCode()
        {
            return this.Aggregate((x, y) => x.GetHashCode() ^ y.GetHashCode());
        }
    }

    class Dice
    {
        static Roll Faces = new Roll { 1, 2, 3, 4, 5, 6 };

        public static List<Roll> RollOne = RollPermutations(1, Faces);
        public static List<Roll> RollTwo = RollPermutations(2, Faces);
        public static List<Roll> RollThree = RollPermutations(3, Faces);
        public static List<Roll> RollFour = RollPermutations(4, Faces);
        public static List<Roll> RollFive = RollPermutations(5, Faces);
        public static List<Roll> RollSix = RollPermutations(6, Faces);

        public static Dictionary<int, List<Roll>> Permute = new Dictionary<int, List<Roll>> {
            {1, RollOne},
            {2, RollTwo},
            {3, RollThree},
            {4, RollFour},
            {5, RollFive},
            {6, RollSix}
        };

        public static List<Roll> RollPermutations(int dice, Roll roll)
        {
            switch (dice)
            {
                case 1:
                    return
                        (from a in roll
                        select new Roll { a }).ToList();
                case 2:
                    return
                        (from a in roll
                        from b in roll
                        select new Roll { a, b }).ToList();
                case 3:
                    return
                        (from a in roll
                        from b in roll
                        from c in roll
                        select new Roll { a, b, c }).ToList();
                case 4:
                    return
                        (from a in roll
                        from b in roll
                        from c in roll
                        from d in roll
                        select new Roll { a, b, c, d }).ToList();
                case 5:
                    return
                        (from a in roll
                        from b in roll
                        from c in roll
                        from d in roll
                        from e in roll
                        select new Roll { a, b, c, d, e }).ToList();
                case 6:
                    return
                        (from a in roll
                         from b in roll
                         from c in roll
                         from d in roll
                         from e in roll
                         from f in roll
                         select new Roll { a, b, c, d, e, f }).ToList();
                default:
                    return null;
            }
        }

        public static IEnumerable<Roll> RollCombinations(int dice, Roll roll)
        {
            if (dice == roll.Count()) return new List<Roll> { roll };

            List<Roll> combos = new List<Roll>();
            for (int a = 0; a < roll.Count; a++ )
            {
                if (dice == 1) { combos.Add(new Roll { roll[a] }); continue; }
                for (int b = a + 1; b < roll.Count; b++)
                {
                    if (dice == 2) { combos.Add(new Roll { roll[a], roll[b] }); continue; }
                    for (int c = b + 1; c < roll.Count; c++)
                    {
                        if (dice == 3) { combos.Add(new Roll { roll[a], roll[b], roll[c] }); continue; }
                        for (int d = c + 1; d < roll.Count; d++)
                        {
                            if (dice == 4) { combos.Add(new Roll { roll[a], roll[b], roll[c], roll[d] }); continue; }
                            for (int e = d + 1; e < roll.Count; e++)
                            {
                                if (dice == 5) { combos.Add(new Roll { roll[a], roll[b], roll[c], roll[d], roll[e] }); continue; }
                                for (int f = e + 1; f < roll.Count; f++)
                                {
                                    combos.Add(new Roll { roll[a], roll[b], roll[c], roll[d], roll[e], roll[f] });
                                }

                            }

                        }

                    }

                }

            }
            return combos;
        }

    }
}
