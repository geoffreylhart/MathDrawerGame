using MathDrawerGame.AI.Animations;
using MathDrawerGame.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame
{
    public static class BasicEffectHelper
    {
        public static void DrawLineStrip(this BasicEffect effect, GraphicsDevice GraphicsDevice, VertexPositionColor[] vertices)
        {
            if (vertices.Count() < 2) return;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length-1);
            }
        }
        public static void DrawParabola(this BasicEffect effect, GraphicsDevice GraphicsDevice, ParabolaSegment parabola, Color? color = null)
        {
            if (color == null) color = Color.White;
            VertexPositionColor[] vertices = new VertexPositionColor[101];
            for (int i = 0; i <= 100; i++)
            {
                Vector2D pos = parabola.PosAt(parabola.time * i / 100.0);
                vertices[i] = new VertexPositionColor(new Vector3((float)pos.x, (float)pos.y, 0), color.Value);
            }
            effect.DrawLineStrip(GraphicsDevice, vertices);
        }
        public static void DrawAnimation(this BasicEffect effect, GraphicsDevice GraphicsDevice, IAnimation2D anim, Color? color = null)
        {
            if (color == null) color = Color.White;
            VertexPositionColor[] vertices = new VertexPositionColor[101];
            for (int i = 0; i <= 100; i++)
            {
                Vector2D pos = anim.Pos(anim.Time() * i / 100.0);
                vertices[i] = new VertexPositionColor(new Vector3((float)pos.x, (float)pos.y, 0), color.Value);
            }
            effect.DrawLineStrip(GraphicsDevice, vertices);
        }
        public static void DrawLine(this BasicEffect effect, GraphicsDevice GraphicsDevice, float x1, float y1, float x2, float y2, Color? color = null)
        {
            if (color == null) color = Color.White;
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(new Vector3(x1, y1, 0), color.Value);
            vertices[1] = new VertexPositionColor(new Vector3(x2, y2, 0), color.Value);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
            }
        }
        public static Vector2D GetMouseRelative(this BasicEffect effect, GraphicsDevice GraphicsDevice)
        {
            MouseState mouseState = Mouse.GetState();
            Vector3 mouseCoords = new Vector3(mouseState.X, mouseState.Y, 0);
            Vector3 relativeCoords = GraphicsDevice.Viewport.Unproject(mouseCoords, effect.Projection, effect.View, effect.World);
            return new Vector2D(relativeCoords.X, relativeCoords.Y);
        }
        public static void DrawPoint(this BasicEffect effect, GraphicsDevice GraphicsDevice, double x, double y, Color? color = null)
        {
            effect.DrawPoint(GraphicsDevice, (float)x, (float)y, color);
        }
        public static void DrawPoint(this BasicEffect effect, GraphicsDevice GraphicsDevice, float x, float y, Color? color = null)
        {
            if (color == null) color = Color.White;
            VertexPositionColor[] corners = new VertexPositionColor[4];
            Vector3 projected = GraphicsDevice.Viewport.Project(new Vector3(x, y, 0), effect.Projection, effect.View, effect.World);
            float ux = projected.X;
            float uy = projected.Y;
            corners[0] = new VertexPositionColor(new Vector3(ux - 5, uy + 5, 0), color.Value);
            corners[1] = new VertexPositionColor(new Vector3(ux - 5, uy - 5, 0), color.Value);
            corners[2] = new VertexPositionColor(new Vector3(ux + 5, uy + 5, 0), color.Value);
            corners[3] = new VertexPositionColor(new Vector3(ux + 5, uy - 5, 0), color.Value);
            for (int i = 0; i < 4; i++)
            {
                Vector3 unprojected = GraphicsDevice.Viewport.Unproject(corners[i].Position, effect.Projection, effect.View, effect.World);
                corners[i] = new VertexPositionColor(unprojected, corners[i].Color);
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, corners, 0, 2);
            }
        }
        public static void DrawString(this BasicEffect effect, GraphicsDevice GraphicsDevice, SpriteFont font, String text, float x, float y)
        {
            Vector3 projected = GraphicsDevice.Viewport.Project(new Vector3(x, y, 0), effect.Projection, effect.View, effect.World);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, new Vector2(projected.X + 5, projected.Y + 5), Color.White);
            spriteBatch.End();
        }
        public static void DrawStringAbsolute(this BasicEffect effect, GraphicsDevice GraphicsDevice, SpriteFont font, String text, float x, float y)
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, new Vector2(x, y), Color.White);
            spriteBatch.End();
        }
    }
}
