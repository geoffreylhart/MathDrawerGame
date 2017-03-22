using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame
{
    class Movement2D
    {
        private float x = 0;
        private float y = 0;

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D)) x += -0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) x += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) y += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) y += -0.1f;
        }

        public Matrix AsMatrix()
        {
            return Matrix.CreateTranslation(x, y, 0);
        }
    }
}
