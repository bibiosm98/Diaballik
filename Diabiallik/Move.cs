using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Diabiallik
{
    class Move : IComparable<Move>
    {
        /// <summary>
        /// W związku z tym, że ruchy są 2, tym samym pionkiem albo dwoma i kilka wczesniejszych koncepcji zawiodło,
        /// robię ruch podwójny, z_1, do_1 odpowiadają za 1 ruch, i z_2, do_2 za 2 lub bez ruchu, wtedy są -1
        /// </summary>
        public int
            from_1,
            to_1,
            from_2,
            to_2,
            score = 0;
        public Move(int i, int y)
        {
            this.from_1 = i;
            this.to_1 = y;
            this.from_2 = -1;
            this.to_2 = -1;
        }
        public Move(int i, int y, int j, int h)
        {
            this.from_1 = i;
            this.to_1 = y;
            this.from_2 = j;
            this.to_2 = h;
        }

        public int CompareTo([AllowNull] Move move)
        {
            if (move == null)
                return 1;
            else
                return this.score.CompareTo(move.score);
        }

        public override string ToString()
        {
            return base.ToString() + " " + from_1 + ":" + to_1 + ", " + from_2 + ":" + to_2 + " score = " + score;
        }
    }
}
