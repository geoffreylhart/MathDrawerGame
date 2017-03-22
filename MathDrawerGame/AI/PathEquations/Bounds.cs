using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.PathEquations
{
    public class Bounds
    {
        public double low, high;

        public Bounds(double low, double high)
        {
            this.low = low;
            this.high = high;
        }

        internal bool Includes(double x)
        {
            return x >= low && x <= high;
        }
    }
}
