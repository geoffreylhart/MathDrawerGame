using MathDrawerGame.AI;
using MathDrawerGame.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace MathDrawerGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        MainTerrain mainTerrain;
        Movement2D movement;
        BasicEffect basicEffect;
        SpriteFont font;
        HumanPlayer player;
        LaunchTester debugger;
        CritcalLaunchTester debugger2;
        SimpleAITester debugger3;

        public Game1(string file)
        {
            graphics = new GraphicsDeviceManager(this);
            mainTerrain = new MainTerrain();
            mainTerrain.Load(file);
            movement = new Movement2D();
            Content.RootDirectory = "Content";
            player = new HumanPlayer();
            debugger = new LaunchTester();
            //debugger2 = new CritcalLaunchTester();
            //debugger3 = new SimpleAITester();
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            basicEffect = BasicEffectGen.MakeBasic(GraphicsDevice);
            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            movement.Update();
            basicEffect.World = movement.AsMatrix() * Matrix.CreateScale(20);
            Vector2D mouseRelativeCoords = basicEffect.GetMouseRelative(GraphicsDevice);
            player.Update(gameTime.ElapsedGameTime.TotalSeconds, mainTerrain, mouseRelativeCoords);
            debugger.Update(player, mainTerrain, mouseRelativeCoords);
            //debugger2.Update(player, mainTerrain, mouseRelativeCoords);
            //debugger3.Update(player, mainTerrain, mouseRelativeCoords, gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            mainTerrain.Draw(basicEffect, GraphicsDevice, font);
            player.Draw(basicEffect, GraphicsDevice);
            basicEffect.DrawStringAbsolute(GraphicsDevice, font, player.errortext, 5, 50);
            debugger.Draw(basicEffect, GraphicsDevice);
            //debugger2.Draw(basicEffect, GraphicsDevice);
            //debugger3.Draw(basicEffect, GraphicsDevice);
        }
    }
}
