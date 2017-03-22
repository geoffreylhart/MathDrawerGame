using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    public class ParabolicAnimation : IAnimation
    {
        private double time;
        private double pos;
        private double v;
        private double a;
        public ParabolicAnimation(double pos, double v, double a, double time)
        {
            this.time = time;
            this.pos = pos;
            this.v = v;
            this.a = a;
        }
        public override double Time()
        {
            return time;
        }
        public override double Pos(double t)
        {
            return 0.5 * a * t * t + v * t + pos;
        }
        public override double V(double t)
        {
            return a * t + v;
        }
    }
}
