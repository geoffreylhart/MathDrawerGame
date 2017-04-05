using MathDrawerGame.AI.Animations;
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


        // linetoavoid -  no collision at all possible with this
        // linetomostlyavoid - only 1 of 2 possible collisions with this
        internal static JumpInfo FullJumpInfo(List<IntLine> list, IntLine linetoavoid, IntLine[] linetomostlyavoid, Vector2D start, Vector2D velocity, double a)
        {
            JumpInfo fullinfo = new JumpInfo();
            ParabolicAnimation2D newparab = new ParabolicAnimation2D(start, velocity, new Vector2D(0, a), 10);
            for (int i = 0; i < 10; i++)
            {
                double mint = double.PositiveInfinity;
                IntLine collider = null;
                foreach (IntLine collide in list)
                {
                    double? collidetime;
                    if (linetoavoid == collide)
                    {
                        continue; // we expect two zeroes
                    }
                    else if (linetomostlyavoid.Contains(collide))
                    {
                        collidetime = newparab.ToSegment().NonZeroIntersectionTime(collide); // we expect one zero
                    }
                    else
                    {
                        collidetime = newparab.ToSegment().FirstIntersectionTime(collide);
                    }
                    if (collidetime.HasValue && collidetime.Value < mint)
                    {
                        mint = collidetime.Value;
                        collider = collide;
                    }
                }
                fullinfo.anim += newparab;
                ParabolaSegment newparabasseg = newparab.ToSegment();
                fullinfo.parabs.Add(newparabasseg);
                if (collider != null)
                {
                    linetomostlyavoid = new IntLine[0];
                    linetoavoid = collider;
                    newparab.time = mint;
                    newparabasseg.time = mint;
                    Vector2D velat = newparab.V(mint);
                    double crossp = (velat.x * collider.DY - velat.y * collider.DX) / (velat.Length() * collider.Length);
                    bool oncw = crossp > 0;
                    if (!(oncw && collider.DX > 0) && !(!oncw && collider.DX < 0))
                    {
                        break;
                    }
                    newparab = new ParabolicAnimation2D(newparab.Pos(mint), velat.Flatten(collider), new Vector2D(0, a), 10); // TODO: figure out why t=1000 looked weird (renderer duh)
                }
                else
                {
                    break;
                }
            }
            return fullinfo;
        }

        public class JumpInfo
        {
            public IAnimation2D anim = new EmptyAnimation2D();
            public List<ParabolaSegment> parabs = new List<ParabolaSegment>();
        }
    }
}
