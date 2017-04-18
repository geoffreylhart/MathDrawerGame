using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.Geom
{
    // do be usely for math - as a replacement for Vector2 which uses floats
    // ideally this class should be usable in all projects
    // immutable
    public class Vector2D
    {
        public double x;
        public double y;
        public static Vector2D ZERO = new Vector2D(0,0);

        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double Length()
        {
            return Math.Sqrt(this.LengthSquared());
        }

        public double LengthSquared()
        {
            return x * x + y * y;
        }

        public override bool Equals(object o)
        {
            Vector2D that = (Vector2D)o;
            return this.x == that.x && this.y == that.y;
        }

        public Vector2D Normalize(double n)
        {
            double len = this.Length();
            return new Vector2D(x * n / len, y * n / len);
        }
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.x+v2.x,v1.y+v2.y);
        }
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vector2D operator *(Vector2D v1, double a)
        {
            return new Vector2D(v1.x * a, v1.y * a);
        }
        public static Vector2D operator *(double a, Vector2D v1)
        {
            return new Vector2D(v1.x * a, v1.y * a);
        }

        internal Vector2D Flatten(IntLine collider)
        {
            double totalv = this.Length();
            // note, I think this works regardless of which direction the line is facing
            double dotp = (this.x * collider.DX + this.y * collider.DY) / (totalv * collider.Length);
            double vp = totalv / collider.Length;
            vp *= dotp;
            double vx = vp * collider.DX;
            double vy = vp * collider.DY;
            return new Vector2D(vx, vy);
        }

        // copy of above but with line2d
        internal Vector2D Flatten(Line2D collider)
        {
            double totalv = this.Length();
            // note, I think this works regardless of which direction the line is facing
            double dotp = (this.x * collider.DX + this.y * collider.DY) / (totalv * collider.Length);
            double vp = totalv / collider.Length;
            vp *= dotp;
            double vx = vp * collider.DX;
            double vy = vp * collider.DY;
            return new Vector2D(vx, vy);
        }

        public double M { get { return y / x; } }

        public double GravityMultiplier()
        {
            return Math.Abs(this.y) / this.Length();
        }

        internal double CrossProduct(Vector2D v2)
        {
            return this.x * v2.y - v2.x * this.y;
        }
    }
}
