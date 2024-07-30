using Raylib_cs;
using System.Numerics;


namespace CakeEngine.games;
internal class DummyGame : IGame {
    public string InititalTitle => "dummy";

    public Vector2 InitialWindowSize => new Vector2(800, 600);

    public bool IsRunning => !Raylib.WindowShouldClose();

    TextureRegistry texReg;
    Texture2D missingTexture;

    public void PreWindow() {
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
    }

    public void Init() {
        Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
        texReg = new TextureRegistry();
        texReg.Add("idk", "assets/001.png", new Vector2(500, 500));
        texReg.Load("idk");
        missingTexture = Raylib.LoadTexture("assets/missing.jpg");
    }


    public void Render(double dt) {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
        var tex = texReg.Get("pizda");
        Raylib.DrawTexture(tex, 0, 0, Color.White);
        Raylib.EndDrawing();
    }

    public void Update(double dt) {
    }

    public void Close() {
    }
}
