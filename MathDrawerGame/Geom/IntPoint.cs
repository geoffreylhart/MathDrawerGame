using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.Geom
{
    public class IntPoint
    {
        public int x;
        public int y;

        public IntPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object o)
        {
            IntPoint that = (IntPoint)o;
            return this.x == that.x && this.y == that.y;
        }

        internal Vector2D AsVector2D()
        {
            return new Vector2D(x, y);
        }
    }
}
