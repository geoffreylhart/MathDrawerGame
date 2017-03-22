using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame
{
    class BasicEffectGen
    {
        // the standard copy-paste effect
        internal static BasicEffect MakeBasic(GraphicsDevice GraphicsDevice)
        {
            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Matrix viewMatrix = Matrix.CreateLookAt(Vector3.UnitZ,Vector3.Zero,Vector3.Up);
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0,
                (float)GraphicsDevice.Viewport.Width,
                (float)GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            BasicEffect effect = new BasicEffect(GraphicsDevice);
            effect.World = world;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.VertexColorEnabled = true;
            return effect;
        }
    }
}
