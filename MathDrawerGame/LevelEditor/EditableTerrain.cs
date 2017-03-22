using MathDrawerGame.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MathDrawerGame.LevelEditor
{
    class EditableTerrain : SavableTerrain
    {
        private Vector2D mouseRelativeCoords = null;
        IntPoint temppoint = null;

        public void EditUpdate()
        {
        }

        private ButtonState prevMouse = ButtonState.Released;
        private string file;

        public void EditUpdate(Vector2D mouse)
        {
            mouseRelativeCoords = mouse;
            int roundx = (int)Math.Round(mouseRelativeCoords.x);
            int roundy = (int)Math.Round(mouseRelativeCoords.y);
            ButtonState currMouse = Mouse.GetState().LeftButton;
            if (prevMouse == ButtonState.Pressed && currMouse == ButtonState.Released)
            {
                IntPoint newpoint = new IntPoint(roundx, roundy);
                if(points.Contains(newpoint)){
                    newpoint = points.Find(x => x.Equals(newpoint));
                }else{
                    points.Add(newpoint);
                }
                if (temppoint != null)
                {
                    IntLine newline = new IntLine(temppoint, newpoint);
                    if (!lines.Contains(newline))
                    {
                        lines.Add(newline);
                    }
                    temppoint = null;
                }else{
                    temppoint = newpoint;
                }
            }
            prevMouse = currMouse;
        }

        public void Draw(BasicEffect basicEffect, GraphicsDevice GraphicsDevice, SpriteFont font)
        {
            if (mouseRelativeCoords != null)
            {
                double roundx = Math.Round(mouseRelativeCoords.x);
                double roundy = Math.Round(mouseRelativeCoords.y);
                basicEffect.DrawPoint(GraphicsDevice, roundx, roundy);
                basicEffect.DrawString(GraphicsDevice, font, "(" + roundx + ", " + roundy + ")", (float)roundx, (float)roundy);
            }
            base.Draw(basicEffect, GraphicsDevice, font);
        }
    }
}
