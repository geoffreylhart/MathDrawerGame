using MathDrawerGame.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    class ParabolicAnimation2D : IAnimation2D
    {
        public double time;
        private Vector2D pos;
        private Vector2D v;
        private Vector2D a;
        public ParabolicAnimation2D(Vector2D pos, Vector2D v, Vector2D a, double time)
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
        public override Vector2D Pos(double t)
        {
            return 0.5 * a * t * t + v * t + pos;
        }
        public override Vector2D V(double t)
        {
            return a * t + v;
        }

        public ParabolaSegment ToSegment()
        {
            return ParabolaSegment.FromBasics(pos, v, a.y, time);
        }
    }
}
