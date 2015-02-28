using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class Roll : List<int>, IEquatable<Roll>
    {
        public Roll() : base() { }
        public Roll(Roll roll) : base(roll) { }
        public Roll(IEnumerable<int> roll) : base(roll) { }
        public Roll(string roll)
            : base()
        {
            foreach (char c in roll) Add(int.Parse(c.ToString()));
        }

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

        public override string ToString()
        {
            return "[ " + this.OrderBy(x => x).Select(x => x.ToString()).Aggregate((x, y) => x + ' ' + y) + " ]";
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
}
