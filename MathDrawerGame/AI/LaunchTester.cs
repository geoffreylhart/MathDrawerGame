using MathDrawerGame.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI
{
    class LaunchTester : PointDebugger
    {
        List<ParabolaSegment> parabs = new List<ParabolaSegment>();
        internal void Draw(BasicEffect basicEffect, GraphicsDevice GraphicsDevice)
        {
            if (SelectedPoint != null)
            {
                basicEffect.DrawPoint(GraphicsDevice, SelectedPoint.x, SelectedPoint.y, Color.Red);
            }
            foreach (ParabolaSegment parab in parabs)
            {
                basicEffect.DrawParabola(GraphicsDevice, parab, Color.Red);
            }
        }

        double v = 0;
        internal override void Update(HumanPlayer player, MainTerrain mainTerrain, Vector2D mouseRelativeCoords)
        {
            base.Update(player, mainTerrain, mouseRelativeCoords);
            if (v * 1.005 > v + 0.02)
            {
                v *= 1.005;
            }
            else
            {
                v += 0.02;
            }
            if (v > 100) v = 0; // TDOD: debug pure zero v, I'm not sure we expect it to fall through
            parabs.Clear();
            if (SelectedPoint != null)
            {
                IntLine[] lines = mainTerrain.Attached(SelectedPoint);
                foreach (IntLine line in lines)
                {
                    Vector2D velocity = line.AsVector2DWithEnd(SelectedPoint).Normalize(v);
                    ParabolaSegment newparab = ParabolaSegment.FromBasics(SelectedPoint.AsVector2D(), velocity, player.ay, 10);
                    IntLine linetonotcollide = line;
                    IntLine[] linestobewaryof = lines;
                    for (int i = 0; i < 10; i++)
                    {
                        double mint = double.PositiveInfinity;
                        IntLine collider = null;
                        foreach (IntLine collide in mainTerrain.lines)
                        {
                            double? collidetime;
                            if (linetonotcollide == collide)
                            {
                                continue; // we expect two zeroes
                            }
                            else if (linestobewaryof.Contains(collide))
                            {
                                collidetime = newparab.NonZeroIntersectionTime(collide); // we expect one zero
                            }
                            else
                            {
                                collidetime = newparab.FirstIntersectionTime(collide);
                            }
                            if (collidetime.HasValue && collidetime.Value < mint)
                            {
                                mint = collidetime.Value;
                                collider = collide;
                            }
                        }
                        parabs.Add(newparab);
                        if (collider != null)
                        {
                            linestobewaryof = new IntLine[0];
                            linetonotcollide = collider;
                            newparab.time = mint;
                            Vector2D velat = newparab.VelAt(mint);
                            double crossp = (velat.x * collider.DY - velat.y * collider.DX) / (velat.Length() * collider.Length);
                            bool oncw = crossp > 0;
                            if (!(oncw && collider.DX > 0) && !(!oncw && collider.DX < 0))
                            {
                                break;
                            }
                            newparab = ParabolaSegment.FromBasics(newparab.PosAt(mint), velat.Flatten(collider), player.ay, 10); // TODO: figure out why t=1000 looked weird (renderer duh)
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
