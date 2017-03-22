using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    class CombinedAnimation : IAnimation
    {
        private IAnimation a1;
        private IAnimation a2;

        public CombinedAnimation(IAnimation a1, IAnimation a2)
        {
            this.a1 = a1;
            this.a2 = a2;
        }

        public override double Time()
        {
            return a1.Time() + a2.Time();
        }

        public override double Pos(double t)
        {
            if (t > a1.Time()) return a2.Pos(t - a1.Time());
            return a1.Pos(t);
        }

        public override double V(double t)
        {
            if (t > a1.Time()) return a2.V(t - a1.Time());
            return a1.V(t);
        }
    }
}
