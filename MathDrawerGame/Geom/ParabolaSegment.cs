using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.Geom
{
    public class ParabolaSegment
    {
        private Vector2D start;
        private Vector2D velocity;
        private double accel;
        public double time;
        internal static ParabolaSegment FromBasics(Vector2D start, Vector2D velocity, double accel, double time)
        {
            ParabolaSegment newseg = new ParabolaSegment();
            newseg.start = start;
            newseg.velocity = velocity;
            newseg.accel = accel;
            newseg.time = time;
            return newseg;
        }

        internal Vector2D PosAt(double t)
        {
            return new Vector2D(start.x+velocity.x*t,start.y+velocity.y*t+accel*0.5*t*t);
        }

        internal double? FirstIntersectionTime(IntLine line) // TODO: make a Line2D class
        {
            if (line.Vertical) // infinite slope
            {
                if (velocity.x == 0) return null; // TODO: consider changing this case
                double t = (line.p1.x - start.x) / velocity.x;
                if (MathHelper.Between(t, 0, time))
                {
                    double newy = 0.5 * accel * t * t + velocity.y * t + start.y;
                    if (MathHelper.Between(newy, line.p1.y, line.p2.y))
                    {
                        return t;
                    }
                }
            }
            else
            {
                double a = 0.5 * accel;
                double b = velocity.y - line.M * velocity.x;
                double c = start.y - line.B - line.M * start.x;
                if (b * b - 4 * a * c < 0) return null;
                double t1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double t2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                foreach (double t in new double[] { t1, t2 })
                {
                    if (MathHelper.Between(t, 0, time))
                    {
                        double newx = start.x + t * velocity.x;
                        if (MathHelper.Between(newx, line.p1.x, line.p2.x))
                        {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        // copy of above but with line2d
        internal IntersectionInfo FirstIntersectionTime(Line2D line) // TODO: make a Line2D class
        {
            if (line.Vertical) // infinite slope
            {
                if (velocity.x == 0) return null; // TODO: consider changing this case
                double t = (line.p1.x - start.x) / velocity.x;
                if (MathHelper.Between(t, 0, time))
                {
                    double newy = 0.5 * accel * t * t + velocity.y * t + start.y;
                    if (MathHelper.Between(newy, line.p1.y, line.p2.y))
                    {
                        return new IntersectionInfo(t, (newy - line.p1.y) / line.DY);
                    }
                }
            }
            else
            {
                double a = 0.5 * accel;
                double b = velocity.y - line.M * velocity.x;
                double c = start.y - line.B - line.M * start.x;
                if (b * b - 4 * a * c < 0) return null;
                double t1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double t2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                foreach (double t in new double[] { t1, t2 })
                {
                    if (MathHelper.Between(t, 0, time))
                    {
                        double newx = start.x + t * velocity.x;
                        if (MathHelper.Between(newx, line.p1.x, line.p2.x))
                        {
                            return new IntersectionInfo(t, (newx-line.p1.x)/line.DX);
                        }
                    }
                }
            }
            return null;
        }

        public class IntersectionInfo
        {
            public double parabtime;
            public double linep;

            public IntersectionInfo(double parabtime, double linep)
            {
                this.parabtime = parabtime;
                this.linep = linep;
            }
        }

        internal double? NonZeroIntersectionTime(IntLine line) // when you expect a zero - TODO: rename method to something like "closest absolute time"
        {
            if (line.Vertical) // infinite slope
            {
                return null; // TODO: consider changing this case
            }
            else
            {
                double a = 0.5 * accel;
                double b = velocity.y - line.M * velocity.x;
                double c = start.y - line.B - line.M * start.x;
                if (b * b - 4 * a * c < 0) return null;
                double t1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double t2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double t = t1;
                if (Math.Abs(t2) > Math.Abs(t1))
                {
                    t = t2;
                }
                if (MathHelper.Between(t, 0, time))
                {
                    double newx = start.x + t * velocity.x;
                    if (MathHelper.Between(newx, line.p1.x, line.p2.x))
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        // copy of above but with line2d
        internal IntersectionInfo NonZeroIntersectionTime(Line2D line) // when you expect a zero - TODO: rename method to something like "closest absolute time"
        {
            if (line.Vertical) // infinite slope
            {
                return null; // TODO: consider changing this case
            }
            else
            {
                double a = 0.5 * accel;
                double b = velocity.y - line.M * velocity.x;
                double c = start.y - line.B - line.M * start.x;
                if (b * b - 4 * a * c < 0) return null;
                double t1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double t2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double t = t1;
                if (Math.Abs(t2) > Math.Abs(t1))
                {
                    t = t2;
                }
                if (MathHelper.Between(t, 0, time))
                {
                    double newx = start.x + t * velocity.x;
                    if (MathHelper.Between(newx, line.p1.x, line.p2.x))
                    {
                        return new IntersectionInfo(t, (newx - line.p1.x)/line.DX);
                    }
                }
            }
            return null;
        }

        internal Vector2D VelAt(double t)
        {
            return new Vector2D(velocity.x, velocity.y + accel * t);
        }

        internal static ParabolaSegment FromIntersectsAndAccelAndDirection(Vector2D start, Vector2D end, double accel, Vector2D direction)
        {
            //start.x+t*vx=end.x
            //vy/vx=M
            //start.y+0.5*accel*t*t+vy*t=end.y
            //vx=(end.x-start.x)/t
            //vy=M*(end.x-start.x)/t
            //start.y+0.5*accel*t*t+M*(end.x-start.x)=end.y
            double a = 0.5*accel;
            //double b = 0;
            double c = start.y+direction.M*(end.x-start.x)-end.y;
            if(-4*a*c<0) return null;
            double t = Math.Sqrt(-4*a*c)/(2*a);
            double vy = direction.M * (end.x - start.x) / t;
            double vx = vy / direction.M;
            ParabolaSegment newseg = new ParabolaSegment();
            newseg.accel = accel;
            newseg.start = start;
            newseg.velocity = new Vector2D(vx, vy);
            newseg.time = t;
            return newseg;
        }

        internal static ParabolaSegment FromIntersectAndTangentAndAccelAndDirection(Vector2D start, IntLine tangent, double accel, Vector2D direction)
        {
            //y=tangent.M*x+tangent.B
            //x=start.x+vx*t
            //y=0.5*accel*t*t+vy*t+start.y
            //vy/vx=direction.M
            //(vy+t*accel)/vx=tangent.M

            //vy/direction.M=(vy+t*accel)/tangent.M
            //vy=(vy+t*accel)/tangent.M*direction.M
            //vy-vy/tangent.M*direction.M=(t*accel)/tangent.M*direction.M
            //vy=(t*accel/tangent.M*direction.M)/(1-1/tangent.M*direction.M)
            //0.5*accel*t*t+vy*t+start.y=tangent.M*(start.x+vy/direction.M*t)+tangent.B
            //0.5*accel*t*t+vy*t+start.y=tangent.M*start.x+tangent.M*vy/direction.M*t+tangent.B
            //vy=(0.5*accel*t*t+start.y-tangent.M*start.x-tangent.B)/(tangent.M/direction.M*t-t)
            //(0.5*accel*t*t+start.y-tangent.M*start.x-tangent.B)/(tangent.M/direction.M*t-t)=(t*accel/tangent.M*direction.M)/(1-1/tangent.M*direction.M)
            //(0.5*accel*t*t+start.y-tangent.M*start.x-tangent.B)/(tangent.M/direction.M-1)=(t*t*accel/tangent.M*direction.M)/(1-direction.M/tangent.M)
            double a = 0.5*accel/(tangent.M/direction.M-1)-accel/tangent.M*direction.M/(1-direction.M/tangent.M);
            //double b = 0;
            double c = (start.y - tangent.M * start.x - tangent.B) / (tangent.M / direction.M-1);
            if (-4 * a * c < 0) return null;
            double t = Math.Sqrt(-4 * a * c) / (2 * a);
            double vy = (t * accel / tangent.M * direction.M) / (1 - 1 / tangent.M * direction.M);
            if (vy / direction.y < 0) return null; // wrong way
            double vx = vy / direction.M;
            double x = vx * t + start.x;
            if (!MathHelper.Between(x, tangent.p1.x, tangent.p2.x)) return null; // doesnt actually hit the line
            double y = 0.5 * accel * t * t + vy * t + start.y;
            ParabolaSegment newseg = new ParabolaSegment();
            newseg.accel = accel;
            newseg.start = start;
            newseg.velocity = new Vector2D(vx, vy);
            newseg.time = t;
            return newseg;
        }
    }
}
