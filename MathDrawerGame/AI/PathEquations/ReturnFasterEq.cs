using MathDrawerGame.AI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI.PathEquations
{
    class ReturnFasterEq : ISpeedTimeFunction
    {
        private double a1;
        private double a2;
        private double d;

        public ReturnFasterEq(double a1, double a2, double d)
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
            //copy pasted almost straight from StraightEq, except replaced d with 0
            //0.5*a1*t1*t1+s*t1+0.5*a2*t2*t2+(s+a1*t1)*t2=0
            //a1*t1+a2*t2+s=e
            // therefores
            //t2=(e-s-a1*t1)/a2
            //0.5*a1*t1*t1+s*t1+0.5*(e-s-a1*t1)*(e-s-a1*t1)/a2+(s+a1*t1)*(e-s-a1*t1)/a2=0
            double a = 0.5 * a1 + 0.5 * a1 * a1 / a2 - a1 * a1 / a2;
            double b = s - 1 / a2 * (e - s) * a1 + a1 * (e - s) / a2 - a1 * s / a2;
            double c = 0.5 * (e - s) * (e - s) / a2 + s * (e - s) / a2;
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
            if (e > 0) return null;
            if (a2 >= 0) return null; // impossible to have returned, no way to accelerate back
            double turntime = (0 + e) / a2;
            if (turntime >= 0)
            {
                double turndis = turntime * turntime * a2 * 0.5 - turntime * e; // just a guess right now
                if (turndis > d)
                {
                    return null;
                }
            }
            double turntime2 = (0 + e) / a2; // using the slower return method
            double lowbound = 0;
            if (turntime2 >= 0 && a1 <= 0) // MISTAKE? added a check that a1 was <0
            {
                double turndis2 = turntime2 * turntime2 * a2 * 0.5 - turntime2 * e;
                if (turndis2 < d)
                {
                    lowbound = Math.Sqrt(-2 * turndis2 * a1);
                }
            }
            return new Bounds(lowbound, -e); // just a guess right now
        }

        // looks good
        // kind of halfassing all of these bounds
        // MISTAKE - forgot that speeds are actually returned exclusvely negative - had to switch method contents and make negative
        public Bounds EBound(double s)
        {
            if (s < 0) return null;
            if (a2 >= 0) return null; // impossible to return, no way to accelerate back
            double turntime = (0 - s) / a2;
            if (turntime >= 0)
            {
                double turndis = turntime * turntime * a2 * 0.5 + turntime * s;
                if (turndis > d)
                {
                    return null;
                }
            }
            double turntime2 = (0 - s) / a1; // using the slower return method
            double lowbound = -Math.Sqrt(-2 * d * a2);
            if (turntime2 >= 0 && a2 <= 0)
            {
                double turndis2 = turntime2 * turntime2 * a1 * 0.5 + turntime2 * s;
                if (turndis2 < d)
                {
                    lowbound = -Math.Sqrt(-2 * turndis2 * a2);
                }
            }
            // 0.5a2*t*t=-d (negative d because this is the return trip from d)
            //t=sqrt(-2d/a2)
            //e=a1*sqrt(2d/a2)
            return new Bounds(lowbound, -s);
        }

        public IAnimation Animate(double s, double e)
        {
            // literally copy pasted from F
            double a = 0.5 * a1 + 0.5 * a1 * a1 / a2 - a1 * a1 / a2;
            double b = s - 1 / a2 * (e - s) * a1 + a1 * (e - s) / a2 - a1 * s / a2;
            double c = 0.5 * (e - s) * (e - s) / a2 + s * (e - s) / a2;
            if (b * b - 4 * a * c < 0) return null;
            double t1n = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t1p = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double t2n = (e - s - a1 * t1n) / a2;
            double t2p = (e - s - a1 * t1p) / a2;
            double tn = t1n + t2n;
            double tp = t1p + t2p;
            // copy pasted below from StraightEq
            if (tn < tp && Valid(t1n, t2n, s))
            {
                IAnimation anim = new ParabolicAnimation(0, s, a1, t1n);
                anim = anim + new ParabolicAnimation(anim.EndPos(), anim.EndV(), a2, t2n);
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
