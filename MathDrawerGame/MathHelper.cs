using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame
{
    class MathHelper
    {
        internal static bool Between(double x, double x1, double x2)
        {
            if (x1 > x2) return Between(x, x2, x1);
            return x >= x1 && x <= x2;
        }
    }
}
