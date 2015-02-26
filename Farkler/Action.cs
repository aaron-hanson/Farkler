using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class Action
    {
        public int ScoreToAdd { get; set; }
        public int DiceToRoll { get; set; }

        public Action(int s, int d)
        {
            ScoreToAdd = s;
            DiceToRoll = d;
        }
    }
}
