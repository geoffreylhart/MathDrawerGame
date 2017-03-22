using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.Geom
{
    public class IntLine
    {
        public IntPoint p1;
        public IntPoint p2;
        public Color color = Color.White;

        public IntLine(IntPoint p1, IntPoint p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public override bool Equals(object o)
        {
            IntLine that = (IntLine)o;
            if (this.p1.Equals(that.p1) && this.p2.Equals(that.p2)) return true;
            if (this.p2.Equals(that.p1) && this.p1.Equals(that.p2)) return true;
            return false;
        }

        public double M { get { return DY / (double)DX; } }

        //y=mx+b
        //b=y-mx
        public double B { get { return p1.y - M * p1.x; } }
        public double Length { get { return Math.Sqrt(DX * DX + DY * DY); } }
        public int DX { get { return p2.x - p1.x; } }
        public int DY { get { return p2.y - p1.y; } }

        public bool Vertical { get { return DX == 0; } }

        internal double XAt(double p)
        {
            return p1.x + DX * p;
        }

        internal double YAt(double p)
        {
            return p1.y + DY * p;
        }

        internal Vector2D At(double p)
        {
            return new Vector2D(XAt(p), YAt(p));
        }

        internal double PFromX(double x)
        {
            return (x - p1.x) / DX;
        }

        internal double PFromY(double y)
        {
            return (y - p1.y) / DY;
        }

        internal double DotProduct(IntLine that)
        {
            return (this.DX * that.DX + this.DY * that.DY)/(this.Length*that.Length);
        }

        internal Vector2D AsVector2DWithEnd(IntPoint end)
        {
            if (end == this.p2)
            {
                return new Vector2D(DX, DY);
            }
            else
            {
                return new Vector2D(-DX, -DY);
            }
        }

        internal Vector2D AsVector2D()
        {
            return new Vector2D(DX, DY);
        }

        internal IntPoint OtherP(IntPoint p)
        {
            if (p1 == p) return p2;
            return p1;
        }
    }
}
