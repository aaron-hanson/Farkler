using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkler
{
    class ExpectedValueCalc
    {
        public static Dictionary<Tuple<int, double>, double> EVCache = new Dictionary<Tuple<int,double>,double>();

        public static double EV(int d, double p)
        {
            if (p >= 1000) return p;

            double ev;
            Tuple<int, double> key = new Tuple<int,double>(d, p);
            if (EVCache.TryGetValue(key, out ev)) return ev;

            ev = Dice.Permute[d]
                .Average(x => Farkle.Gen(x)
                    .Max(y => (double?)EV(y.DiceToRoll, p + y.ScoreToAdd)) ?? 0D);

            ev = p < 300 ? ev : Math.Max(ev, p);
            EVCache.Add(key, ev);
            Console.Write('.');
            return ev;
        }

        public static double EVOld(int d, double p)
        {
            if (p >= 10000) return p;

            double ev;
            Tuple<int, double> key = new Tuple<int,double>(d, p);
            if (EVCache.TryGetValue(key, out ev)) return ev;

            switch (d)
            {
                case 1:
                    //ev = (
                    //    EV(6, p+50) + // 5
                    //    EV(6, p+100)  // 1
                    //    ) / 6D;
                    ev = Dice.RollOne
                        .Average(x => Farkle.Gen(x)
                            .Max(y => (double?)EV(y.DiceToRoll, p + y.ScoreToAdd)) ?? 0D);
                    break;
                case 2:
                    ev = (
                        Max( //11
                            EV(6, p+200),
                            EV(1, p+100)
                        ) +
                        Max( //55
                            EV(6, p+100),
                            EV(1, p+50)
                        ) +
                        2 * EV(6, p+150) + //15
                        8 * EV(1, p+100) + //1
                        8 * EV(1, p+50) //5
                    ) / 36D; //20 score
                    break;
                case 3:
                    ev = (
                        Max( //111
                            EV(6, p+1000),
                            EV(2, p+100),
                            EV(1, p+200)
                        ) +
                        EV(6, p+200) + //222
                        EV(6, p+300) + //333
                        EV(6, p+400) + //444
                        Max( //555
                            EV(6, p+500),
                            EV(2, p+50),
                            EV(1, p+100)
                        ) +
                        EV(6, p+600) + //666
                        3 * Max( //115
                            EV(6, p+250),
                            EV(1, p+200),
                            EV(2, p+100)
                        ) +
                        3 * Max( //155
                            EV(6, p+200),
                            EV(1, p+150),
                            EV(2, p+100)
                        ) +
                        12 * Max( //112,113,114,116
                            EV(1, p+200),
                            EV(2, p+100)
                        ) +
                        12 * Max( //255,355,455,556
                            EV(1, p+100),
                            EV(2, p+50)
                        ) +
                        24 * Max( //125,135,145,156
                            EV(1, p+150),
                            EV(2, p+100)
                        ) +
                        48 * EV(1, p+100) + //122,123,124,126,133,134,136,144,146,166
                        48 * EV(1, p+50) //225,235,245,256,335,345,356,445,456,566
                        ) / 216D;  //156 score
                    break;
                case 4:
                    ev = (
                        Max( //1111
                            EV(6, p+1100),
                            EV(3, p+100),
                            EV(2, p+200),
                            EV(1, p+1000)
                        ) +
                        4 * Max( //1115
                            EV(6, p+1050),
                            EV(3, p+100),
                            EV(2, p+200),
                            EV(1, p+1000)
                        ) +
                        6 * Max( //1155
                            EV(3, p+100),
                            EV(2, p+200),
                            EV(1, p+250),
                            EV(6, p+300)
                        ) +
                        4 * Max( //1555
                            EV(6, p+600),
                            EV(3, p+100),
                            EV(2, p+150),
                            EV(1, p+500)
                        ) +
                        Max( // 5555
                            EV(6, p+550),
                            EV(3, p+50),
                            EV(2, p+100),
                            EV(1, p+500)
                        ) +
                        4 * Max( //1222
                            EV(6, p+300),
                            EV(3, p+100),
                            EV(1, p+200)
                        ) +
                        4 * Max( //1333
                            EV(6, p+400),
                            EV(3, p+100),
                            EV(1, p+300)
                        ) +
                        4 * Max( //1444
                            EV(6, p+500),
                            EV(3, p+100),
                            EV(1, p+400)
                        ) +
                        4 * Max( //1666
                            EV(6, p+700),
                            EV(3, p+100),
                            EV(1, p+600)
                        ) +
                        4 * Max( //2225
                            EV(6, p+250),
                            EV(3, p+50),
                            EV(1, p+200)
                        ) +
                        4 * Max( //3335
                            EV(6, p+350),
                            EV(3, p+50),
                            EV(1, p+300)
                        ) +
                        4 * Max( //4445
                            EV(6, p+450),
                            EV(3, p+50),
                            EV(1, p+400)
                        ) +
                        4 * Max( //5666
                            EV(6, p+650),
                            EV(3, p+50),
                            EV(1, p+600)
                        ) +
                        16 * EV(1, p+1000) + //111(2,3,4,6)
                        13 * EV(1, p+200) + //222(2,3,4,6)
                        13 * EV(1, p+300) + //333(2,3,4,6)
                        13 * EV(1, p+400) + //444(2,3,4,6)
                        16 * EV(1, p+500) + //555(2,3,4,6)
                        13 * EV(1, p+600) + //666(2,3,4,6)
                        48 * Max ( //115(2,3,4,6)
                            EV(3, p+100),
                            EV(2, p+200),
                            EV(1, p+250)
                        ) +
                        48 * Max ( //155(2,3,4,6)
                            EV(3, p+100),
                            EV(2, p+150),
                            EV(1, p+200)
                        ) +
                        96 * Max( //11(2,3,4,6)(2,3,4,6)
                            EV(3, p+100),
                            EV(2, p+200)
                        ) +
                        96 * Max( //55(2,3,4,6)(2,3,4,6)
                            EV(3, p+50),
                            EV(2, p+100)
                        ) +
                        192 * Max( //15(2,3,4,6)(2,3,4,6)
                            EV(3, p+100),
                            EV(2, p+150)
                        ) +
                        240 * EV(3, p+100) + //1(2,3,4,6)(2,3,4,6)(2,3,4,6)
                        240 * EV(3, p+50) //5(2,3,4,6)(2,3,4,6)(2,3,4,6)
                    ) / 1296D; //1092 score
                    break;
                case 5:
                    ev = (
                        0
                        //11111
                        //55555
                        //11115
                        //11155
                        //11555
                        //15555
                        //1111(2346)
                        //1115(2346)
                        //1155(2346)
                        //1555(2346)
                        //5555(2346)
                        //111(2346)(2346)
                        //115(2346)(2346)
                        //155(2346)(2346)
                        //555(2346)(2346)
                        //11(2346)(2346)(2346)
                        //15(2346)(2346)(2346)
                        //55(2346)(2346)(2346)
                        //1(2346)(2346)(2346)(2346)
                        //5(2346)(2346)(2346)(2346)
                        //1222(2346)
                        //1333(2346)
                        //1444(2346)
                        //1666(2346)
                        //2225(2346)
                        //3335(2346)
                        //4445(2346)
                        //5666(2346)
                        //222(2346)(2346)
                        //333(2346)(2346)
                        //444(2346)(2346)
                        //666(2346)(2346)
                    ) / 0D;
                    break;
                case 6: //dont forget hot dice
                    ev = 548;
                    break;
            }

            ev = Math.Max(ev, p);
            EVCache.Add(key, ev);
            return ev;
        }

        static T Max<T>(params T[] values)
        {
            return values.Max<T>();
        }

    }
}
