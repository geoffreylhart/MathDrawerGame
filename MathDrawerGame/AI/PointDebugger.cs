using MathDrawerGame.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI
{
    class PointDebugger
    {
        private IntPoint selectedPoint = null;

        public IntPoint SelectedPoint { get { return selectedPoint; } }
        virtual internal void Update(HumanPlayer player, MainTerrain mainTerrain, Vector2D mouseRelativeCoords)
        {
            int roundx = (int)Math.Round(mouseRelativeCoords.x);
            int roundy = (int)Math.Round(mouseRelativeCoords.y);
            IntPoint point = new IntPoint(roundx, roundy);
            selectedPoint = null;
            if (mainTerrain.points.Contains(point))
            {
                selectedPoint = mainTerrain.points.Find(x => x.Equals(point));
            }
        }
    }
}
