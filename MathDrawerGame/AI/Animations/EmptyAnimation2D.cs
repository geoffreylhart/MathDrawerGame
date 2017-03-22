using MathDrawerGame.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.Animations
{
    class EmptyAnimation2D: IAnimation2D
    {
        public EmptyAnimation2D()
        {
        }
        public override double Time()
        {
            return 0;
        }
        public override Vector2D Pos(double t)
        {
            return Vector2D.ZERO;
        }
        public override Vector2D V(double t)
        {
            return Vector2D.ZERO;
        }
    }
}
