using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class Action : IEquatable<Action>
    {
        public int ScoreToAdd { get; set; }
        public int DiceToRoll { get; set; }

        public Action(int s, int d)
        {
            ScoreToAdd = s;
            DiceToRoll = d;
        }

        public Action Combine(Action oldAction)
        {
            if (oldAction == null) return this;
            else return new Action(ScoreToAdd + oldAction.ScoreToAdd, DiceToRoll);
        }

        public override string ToString()
        {
            return string.Format("{0}d {1}p", DiceToRoll, ScoreToAdd);
        }

        public bool Equals(Action other)
        {
            return ScoreToAdd.Equals(other.ScoreToAdd) && DiceToRoll.Equals(other.DiceToRoll);
        }

        public override int GetHashCode()
        {
            return ScoreToAdd.GetHashCode() ^ DiceToRoll.GetHashCode();
        }
    }
}
