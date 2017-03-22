using MathDrawerGame.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI
{
    // generates critical launch paths - launch paths which define the range of velocities you can jump to other lines 
    class CritcalLaunchTester : PointDebugger
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
                basicEffect.DrawParabola(GraphicsDevice, parab, Color.Green);
            }
        }
        internal override void Update(HumanPlayer player, MainTerrain mainTerrain, Vector2D mouseRelativeCoords)
        {
            base.Update(player, mainTerrain, mouseRelativeCoords);
            parabs.Clear();
            if (SelectedPoint != null)
            {
                IntLine[] lines = mainTerrain.Attached(SelectedPoint);
                foreach (IntLine line in lines)
                {
                    List<IntLine> linesToCollide = mainTerrain.lines.ToList();
                    linesToCollide.Remove(line);
                    foreach (var point in mainTerrain.points)
                    {
                        IntLine[] targettedlines = mainTerrain.Attached(point); // TODO: what was this for again?
                        Vector2D direction = line.AsVector2DWithEnd(SelectedPoint);
                        ParabolaSegment criticalpath = ParabolaSegment.FromIntersectsAndAccelAndDirection(SelectedPoint.AsVector2D(), point.AsVector2D(), player.ay, direction);
                        if (criticalpath != null)
                        {
                            // test if collision with other lines happen first
                            bool anyCollisions = AdvGeomLogic.AnyCollisions(criticalpath, linesToCollide, lines);
                            if (!anyCollisions)
                            {
                                parabs.Add(criticalpath);
                            }
                        }
                    }
                    foreach (var tangent in mainTerrain.lines)
                    {
                        List<IntLine> newlinesToCollide = linesToCollide.ToList();
                        newlinesToCollide.Remove(tangent);
                        Vector2D direction = line.AsVector2DWithEnd(SelectedPoint);
                        if (lines.Contains(tangent)) continue;//always achievable with zero speed
                        ParabolaSegment criticalpath = ParabolaSegment.FromIntersectAndTangentAndAccelAndDirection(SelectedPoint.AsVector2D(), tangent, player.ay, direction);
                        if (criticalpath != null)
                        {
                            // test if collision with other lines happen first
                            bool anyCollisions = AdvGeomLogic.AnyCollisions(criticalpath, newlinesToCollide, lines);
                            if (!anyCollisions)
                            {
                                criticalpath.time = double.PositiveInfinity;
                                criticalpath.time = AdvGeomLogic.FirstCollisionTime(criticalpath, newlinesToCollide, lines);
                                parabs.Add(criticalpath);
                            }
                        }
                    }
                    // TODO: don't forget infinite velocity
                }
            }
        }
    }
}
