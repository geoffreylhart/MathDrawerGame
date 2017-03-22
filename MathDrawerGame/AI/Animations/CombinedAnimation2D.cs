using MathDrawerGame.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    class CombinedAnimation2D : IAnimation2D
    {
        private IAnimation2D a1;
        private IAnimation2D a2;

        public CombinedAnimation2D(IAnimation2D a1, IAnimation2D a2)
        {
            this.a1 = a1;
            this.a2 = a2;
        }

        public override double Time()
        {
            return a1.Time() + a2.Time();
        }

        public override Vector2D Pos(double t)
        {
            if (t > a1.Time()) return a2.Pos(t - a1.Time());
            return a1.Pos(t);
        }

        public override Vector2D V(double t)
        {
            if (t > a1.Time()) return a2.V(t - a1.Time());
            return a1.V(t);
        }
    }
}
