using MathDrawerGame.AI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.PathEquations
{// all three of these equations work like this:
    // negative numbers mean acceleration towards 0, the start
    // positive numbers mean acceleration away
    // s will always be positive
    // a2 is always the most backpedally number, and so the acceleration youd want to use to slow down/reverse
    // e can be negative, only when you return to orgin
    // e must be positive when exiting from the end at d
    // d is always positive
    class StraightEq : ISpeedTimeFunction
    {
        private double a1;
        private double a2;
        private double d;

        public StraightEq(double a1, double a2, double d)
        {
            if (a1 > a2)
            {
                this.a1 = a1;
                this.a2 = a2;
            }
            else
            {
                this.a2 = a1;
                this.a1 = a2;
            }
            this.d = d;
        }
        public double? F(double s, double e)
        {
            //0.5*a1*t1*t1+s*t1+0.5*a2*t2*t2+(s+a1*t1)*t2=d
            //a1*t1+a2*t2+s=e
            // therefores
            //t2=(e-s-a1*t1)/a2
            //0.5*a1*t1*t1+s*t1+0.5*(e-s-a1*t1)*(e-s-a1*t1)/a2+(s+a1*t1)*(e-s-a1*t1)/a2=d
            double a = 0.5 * a1 + 0.5 * a1 * a1 / a2 - a1 * a1 / a2;
            double b = s - 1 / a2 * (e - s) * a1 + a1 * (e - s) / a2 - a1 * s / a2;
            double c = 0.5 * (e - s) * (e - s) / a2 + s * (e - s) / a2 - d;
            if (b * b - 4 * a * c < 0) return null;
            double t1n = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t1p = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t2n = (e - s - a1 * t1n) / a2;
            double t2p = (e - s - a1 * t1p) / a2;
            double tn = t1n + t2n;
            double tp = t1p + t2p;
            if (tn < tp && Valid(t1n, t2n, s))
            {
                return tn;
            }
            if (Valid(t1p, t2p, s))
            {
                return tp;
            }
            if (Valid(t1n, t2n, s))
            {
                return tn;
            }
            return null;
        }

        private bool Valid(double t1, double t2, double s)
        {
            if (t1 < 0 || t2 < 0) return false;
            if (t1 + t2 <= 0) return false; // MISTAKE: extra check so guy doesnt stand still to reach d=0
            double disafterfirst = 0.5 * a1 * t1 * t1 + s * t1;
            if (disafterfirst < 0 || disafterfirst > d) return false;
            double timeturn1 = (0 - s) / a1;
            if (timeturn1 > 0 && timeturn1 < t1)
            {
                double disatturn1 = 0.5 * a1 * timeturn1 * timeturn1 + s * timeturn1;
                if (disatturn1 < 0 || disatturn1 > d) return false;
            }
            double timeturn2 = (0 - s - a1 * t1) / a2;
            if (timeturn2 > 0 && timeturn2 < t2)
            {
                double disatturn2 = 0.5 * a1 * t1 * t1 + s * t1 + 0.5 * a2 * timeturn2 * timeturn2 + (s + a1 * t1) * timeturn2;
                if (disatturn2 < 0 || disatturn2 > d) return false;
            }
            return true;
        }

        public Bounds SBound(double e)
        {
            if (e < 0) return null; // not possible, you cant leave the right side while moving left
            //0.5*a2*t*t+s*t=d
            //s+t*a2=e
            //0.5*(e-s)*(e-s)/a2+s*(e-s)/a2=d
            //0.5*(e+s)*(e-s)/a2=d
            //0.5(e*e-s*s)/a2=d
            //s=Math.sqrt(e*e-2*d*a2)
            if (e * e - 2 * d * a2 < 0) // means youre accelerating too fast to reach speed e (and starting with negative s is no help)
            {
                return null;
            }
            double lowbound;
            if (e * e - 2 * d * a1 >= 0) // but not a2
            {
                lowbound = Math.Sqrt(e * e - 2 * d * a1);
            }
            else
            {
                lowbound = 0;
            }
            return new Bounds(lowbound, Math.Sqrt(e * e - 2 * d * a2));
        }

        public Bounds EBound(double s)
        {
            // copy paste from above, just invert equations (replaced minuses with plus and e with s
            // also swapped a2 and a1
            // visually looks correct
            if (s < 0) return null;
            if (s * s + 2 * d * a1 < 0) // means youre accelerating too fast to reach speed e (and starting with negative s is no help)
            {
                return null;
            }
            double lowbound;
            if (s * s + 2 * d * a2 >= 0) // but not a2
            {
                lowbound = Math.Sqrt(s * s + 2 * d * a2);
            }
            else
            {
                lowbound = 0;
            }
            return new Bounds(lowbound, Math.Sqrt(s * s + 2 * d * a1));
        }

        public IAnimation Animate(double s, double e)
        {
            //literally copy pasted
            double a = 0.5 * a1 + 0.5 * a1 * a1 / a2 - a1 * a1 / a2;
            double b = s - 1 / a2 * (e - s) * a1 + a1 * (e - s) / a2 - a1 * s / a2;
            double c = 0.5 * (e - s) * (e - s) / a2 + s * (e - s) / a2 - d;
            if (b * b - 4 * a * c < 0) return null;
            double t1n = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t1p = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t2n = (e - s - a1 * t1n) / a2;
            double t2p = (e - s - a1 * t1p) / a2;
            double tn = t1n + t2n;
            double tp = t1p + t2p;
            if (tn < tp && Valid(t1n, t2n, s))
            {
                IAnimation anim = new ParabolicAnimation(0, s, a1, t1n);
                anim = anim + new ParabolicAnimation(anim.EndPos(), anim.EndV(),a2, t2n);
                return anim;
            }
            if (Valid(t1p, t2p, s))
            {
                IAnimation anim = new ParabolicAnimation(0, s, a1, t1p);
                anim = anim + new ParabolicAnimation(anim.EndPos(), anim.EndV(), a2, t2p);
                return anim;
            }
            if (Valid(t1n, t2n, s))
            {
                IAnimation anim = new ParabolicAnimation(0, s, a1, t1n);
                anim = anim + new ParabolicAnimation(anim.EndPos(), anim.EndV(), a2, t2n);
                return anim;
            }
            return null;
        }
    }
}
