using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    class AnimationFrom1D : IAnimation2D
    {
        private Geom.Vector2D pos;
        private Geom.Vector2D vector;
        private IAnimation animation;

        public AnimationFrom1D(Geom.Vector2D pos, Geom.Vector2D vector, IAnimation animation)
        {
            this.pos = pos;
            this.vector = vector;
            this.animation = animation;
        }
        public override double Time()
        {
            return animation.Time();
        }

        public override Geom.Vector2D V(double t)
        {
            return animation.V(t) * vector;
        }

        public override Geom.Vector2D Pos(double t)
        {
            return animation.Pos(t) * vector+pos;
        }
    }
}
