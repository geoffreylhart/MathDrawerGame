using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.Geom
{
    public class Line2D
    {
        public Vector2D p1;
        public Vector2D p2;
        public Color color = Color.White;

        public Line2D(Vector2D p1, Vector2D p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public override bool Equals(object o)
        {
            Line2D that = (Line2D)o;
            if (this.p1.Equals(that.p1) && this.p2.Equals(that.p2)) return true;
            if (this.p2.Equals(that.p1) && this.p1.Equals(that.p2)) return true;
            return false;
        }

        public double M { get { return DY / (double)DX; } }

        //y=mx+b
        //b=y-mx
        public double B { get { return p1.y - M * p1.x; } }
        public double Length { get { return Math.Sqrt(DX * DX + DY * DY); } }
        public double DX { get { return p2.x - p1.x; } }
        public double DY { get { return p2.y - p1.y; } }

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
            return (this.DX * that.DX + this.DY * that.DY) / (this.Length * that.Length);
        }

        internal Vector2D AsVector2DWithEnd(Vector2D end)
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

        internal Vector2D OtherP(Vector2D p)
        {
            if (p1 == p) return p2;
            return p1;
        }

        internal double PV(Vector2D vector2D)
        {
            return (DX * vector2D.x + DY * vector2D.y) / this.Length / vector2D.Length();
        }
    }
}
