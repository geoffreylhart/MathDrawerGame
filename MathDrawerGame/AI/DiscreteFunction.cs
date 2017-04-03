using MathDrawerGame.AI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI
{
    class DiscreteFunction
    {
        public Dictionary<int, IAnimation2D> times = new Dictionary<int, IAnimation2D>();

        internal void Put(int i, IAnimation2D animation)
        {
            times[i] = animation;
        }

        internal IAnimation2D Get(int p)
        {
            if (!times.ContainsKey(p)) return null;
            return times[p];
        }
    }
}
