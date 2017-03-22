using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    public abstract class IAnimation
    {
        public abstract double Time();
        public abstract double Pos(double t);
        public abstract double V(double t);
        public double EndPos()
        {
            return Pos(Time());
        }
        public double EndV()
        {
            return V(Time());
        }
        public static IAnimation operator +(IAnimation a1, IAnimation a2)
        {
            return new CombinedAnimation(a1,a2);
        }
    }
}
