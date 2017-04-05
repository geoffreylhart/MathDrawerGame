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
        List<MathDrawerGame.Geom.AdvGeomLogic.JumpInfo> fullinfos = new List<AdvGeomLogic.JumpInfo>();
        internal void Draw(BasicEffect basicEffect, GraphicsDevice GraphicsDevice)
        {
            if (SelectedPoint != null)
            {
                basicEffect.DrawPoint(GraphicsDevice, SelectedPoint.x, SelectedPoint.y, Color.Red);
            }
            foreach (var fullinfo in fullinfos)
            {
                //basicEffect.DrawAnimation(GraphicsDevice, fullinfo.anim, Color.Red);
                foreach (ParabolaSegment parab in fullinfo.parabs)
                {
                    basicEffect.DrawParabola(GraphicsDevice, parab, Color.Red);
                }
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
            if (SelectedPoint != null)
            {
                fullinfos.Clear();
                IntLine[] lines = mainTerrain.Attached(SelectedPoint);
                foreach (IntLine line in lines)
                {
                    Vector2D velocity = line.AsVector2DWithEnd(SelectedPoint).Normalize(v);
                    IntLine otherline = mainTerrain.Attached(SelectedPoint).Where(x => x != line).First();
                    fullinfos.Add(AdvGeomLogic.FullJumpInfo(mainTerrain.lines, line, lines, SelectedPoint.AsVector2D(), velocity, player.ay));
                }
            }
        }
    }
}
