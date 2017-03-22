using MathDrawerGame.AI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.PathEquations
{
    public interface ISpeedTimeFunction
    {
        double? F(double s, double e);
        IAnimation Animate(double s, double e);
        Bounds SBound(double e);
        Bounds EBound(double s);
    }
}
