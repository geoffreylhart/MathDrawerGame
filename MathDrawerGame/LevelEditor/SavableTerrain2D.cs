using MathDrawerGame.Geom;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.LevelEditor
{
    class SavableTerrain2D
    {
        public List<Vector2D> points = new List<Vector2D>();
        public List<Line2D> lines = new List<Line2D>();

        public Line2D OtherLine(Vector2D p, Line2D wrongline)
        {
            foreach (Line2D line in lines)
            {
                if ((line.p1 == p || line.p2 == p) && line != wrongline)
                {
                    return line;
                }
            }
            return null;
        }
        internal Line2D[] Attached(Vector2D p)
        {
            return lines.Where(x => x.p1 == p || x.p2 == p).ToArray();
        }
    }
}
