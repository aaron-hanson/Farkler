using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class FarkleAction : IEquatable<FarkleAction>
    {
        public int ScoreToAdd { get; set; }
        public int DiceToRoll { get; set; }

        public FarkleAction(int s, int d)
        {
            ScoreToAdd = s;
            DiceToRoll = d;
        }

        public FarkleAction Combine(FarkleAction oldAction)
        {
            if (oldAction == null) return this;
            else return new FarkleAction(ScoreToAdd + oldAction.ScoreToAdd, DiceToRoll);
        }

        public override string ToString()
        {
            return string.Format("{0}d {1}p", DiceToRoll, ScoreToAdd);
        }

        public bool Equals(FarkleAction other)
        {
            return ScoreToAdd.Equals(other.ScoreToAdd) && DiceToRoll.Equals(other.DiceToRoll);
        }

        public override int GetHashCode()
        {
            return ScoreToAdd.GetHashCode() ^ DiceToRoll.GetHashCode();
        }
    }
}
