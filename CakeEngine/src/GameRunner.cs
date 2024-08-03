using Raylib_cs;


namespace CakeEngine;
class GameRunner {
    public IGame game;

    public void Load(IGame newGame) {
        game = newGame;
    }


    public void Run() {
        game.PreWindow();
        Raylib.InitWindow((int) game.InitialWindowSize.X, (int) game.InitialWindowSize.Y, game.InititalTitle);
        game.Init();
        double delta = 0;
        while (game.IsRunning) {
            delta = Raylib.GetFrameTime();
            game.Update(delta);
            game.Render(delta);
        }
        game.Close();
        Raylib.CloseWindow();
    }
}
