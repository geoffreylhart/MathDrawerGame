using MathDrawerGame.Geom;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MathDrawerGame.LevelEditor
{
    public class SavableTerrain
    {
        public List<IntPoint> points = new List<IntPoint>();
        public List<IntLine> lines = new List<IntLine>();

        internal void Save(String file)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(file, FileMode.Create));
            writer.Write(points.Count);
            foreach (IntPoint point in points)
            {
                writer.Write(point.x);
                writer.Write(point.y);
            }
            writer.Write(lines.Count);
            foreach (IntLine line in lines)
            {
                int p1index = points.IndexOf(line.p1);
                int p2index = points.IndexOf(line.p2);
                writer.Write(p1index);
                writer.Write(p2index);
            }
        }

        internal void Load(String file)
        {
            BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.Open));
            int count = reader.ReadInt32();
            points = new List<IntPoint>();
            for (int i = 0; i < count; i++)
            {
                if (i == 9)
                {
                    points.Add(new IntPoint(reader.ReadInt32(), reader.ReadInt32()+15));
                }
                else
                {
                    points.Add(new IntPoint(reader.ReadInt32(), reader.ReadInt32()));
                }
            }
            count = reader.ReadInt32();
            lines = new List<IntLine>();
            for (int i = 0; i < count; i++)
            {
                lines.Add(new IntLine(points[reader.ReadInt32()], points[reader.ReadInt32()]));
            }
        }

        internal void Draw(BasicEffect basicEffect, GraphicsDevice GraphicsDevice, SpriteFont font)
        {
            foreach (IntLine line in lines)
            {
                basicEffect.DrawLine(GraphicsDevice, line.p1.x, line.p1.y, line.p2.x, line.p2.y, line.color);
            }
            foreach (IntPoint point in points)
            {
                basicEffect.DrawPoint(GraphicsDevice, point.x, point.y);
            }
            basicEffect.DrawStringAbsolute(GraphicsDevice, font, "Points: " + points.Count + "\nLines: " + lines.Count, 5, 5);
        }

        public IntLine OtherLine(IntPoint p, IntLine wrongline)
        {
            foreach (IntLine line in lines)
            {
                if ((line.p1 == p || line.p2 == p) && line != wrongline)
                {
                    return line;
                }
            }
            return null;
        }
        internal IntLine[] Attached(IntPoint p)
        {
            return lines.Where(x => x.p1 == p || x.p2 == p).ToArray();
        }

        internal SavableTerrain Clone()
        {
            var newterrain = new SavableTerrain();
            newterrain.points = new List<IntPoint>();
            foreach (IntPoint point in points)
            {
                newterrain.points.Add(new IntPoint(point.x, point.y));
            }
            newterrain.lines = new List<IntLine>();
            foreach (IntLine line in lines)
            {
                int p1index = points.IndexOf(line.p1);
                int p2index = points.IndexOf(line.p2);
                newterrain.lines.Add(new IntLine(newterrain.points[p1index], newterrain.points[p2index]));
            }
            return newterrain;
        }
        internal SavableTerrain2D To2D()
        {
            var newterrain = new SavableTerrain2D();
            newterrain.points = new List<Vector2D>();
            foreach (IntPoint point in points)
            {
                newterrain.points.Add(new Vector2D(point.x, point.y));
            }
            newterrain.lines = new List<Line2D>();
            foreach (IntLine line in lines)
            {
                int p1index = points.IndexOf(line.p1);
                int p2index = points.IndexOf(line.p2);
                newterrain.lines.Add(new Line2D(newterrain.points[p1index], newterrain.points[p2index]));
            }
            return newterrain;
        }
    }
}
