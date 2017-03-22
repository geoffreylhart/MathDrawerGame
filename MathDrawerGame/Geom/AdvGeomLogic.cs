using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.Geom
{
    static class AdvGeomLogic
    {
        internal static bool AnyCollisions(ParabolaSegment criticalpath, List<IntLine> linesToCollide, IntLine[] linestobewaryof)
        {
            double mint = criticalpath.time;
            IntLine collider = null;
            foreach (IntLine collide in linesToCollide)
            {
                double? collidetime;
                if (linestobewaryof.Contains(collide))
                {
                    collidetime = criticalpath.NonZeroIntersectionTime(collide); // we expect one zero
                }
                else
                {
                    collidetime = criticalpath.FirstIntersectionTime(collide);
                }
                if (collidetime.HasValue && collidetime.Value < mint)
                {
                    mint = collidetime.Value;
                    collider = collide;
                }
            }
            return collider != null;
        }

        internal static double FirstCollisionTime(ParabolaSegment criticalpath, List<IntLine> linesToCollide, IntLine[] linestobewaryof)
        {
            double mint = criticalpath.time;
            IntLine collider = null;
            foreach (IntLine collide in linesToCollide)
            {
                double? collidetime;
                if (linestobewaryof.Contains(collide))
                {
                    collidetime = criticalpath.NonZeroIntersectionTime(collide); // we expect one zero
                }
                else
                {
                    collidetime = criticalpath.FirstIntersectionTime(collide);
                }
                if (collidetime.HasValue && collidetime.Value < mint)
                {
                    mint = collidetime.Value;
                    collider = collide;
                }
            }
            return mint;
        }
    }
}
