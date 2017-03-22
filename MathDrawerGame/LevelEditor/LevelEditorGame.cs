using MathDrawerGame.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace MathDrawerGame.LevelEditor
{
    public class LevelEditorGame : Game
    {
        GraphicsDeviceManager graphics;
        BasicEffect basicEffect;
        EditableTerrain editTerrain;
        Movement2D movement;
        SpriteFont font;
        String file;

        public LevelEditorGame(string file)
        {
            graphics = new GraphicsDeviceManager(this);
            editTerrain = new EditableTerrain();
            this.file = file;
            movement = new Movement2D();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            basicEffect = BasicEffectGen.MakeBasic(GraphicsDevice);
            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                editTerrain.Save(file);
                Exit();
            }
            movement.Update();
            basicEffect.World = movement.AsMatrix() * Matrix.CreateScale(20);
            Vector2D mouseRelativeCoords = basicEffect.GetMouseRelative(GraphicsDevice);
            editTerrain.EditUpdate(mouseRelativeCoords);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            editTerrain.Draw(basicEffect, GraphicsDevice, font);
            basicEffect.DrawStringAbsolute(GraphicsDevice, font, "WASD to pan\nESC to save\nLEFT-CLICK to edit", 5, 41);
        }
    }
}
