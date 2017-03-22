using MathDrawerGame.LevelEditor;
using System;

namespace MathDrawerGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1("level1.data")) game.Run();
            //using (var game = new LevelEditorGame("level1.data")) game.Run();
        }
    }
}
