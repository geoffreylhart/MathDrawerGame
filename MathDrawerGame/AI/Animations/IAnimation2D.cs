using MathDrawerGame.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    public abstract class IAnimation2D
    {
        public abstract double Time();
        public abstract Vector2D V(double t);
        public abstract Vector2D Pos(double t);
        public Vector2D EndPos()
        {
            return Pos(Time());
        }
        public Vector2D EndV()
        {
            return V(Time());
        }
        public static IAnimation2D operator +(IAnimation2D a1, IAnimation2D a2)
        {
            return new CombinedAnimation2D(a1, a2);
        }
    }
}
