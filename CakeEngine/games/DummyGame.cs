using Raylib_cs;
using System.Numerics;


namespace CakeEngine.games;
internal class DummyGame : IGame {
    public string InititalTitle => "dummy";

    public Vector2 InitialWindowSize => new Vector2(800, 600);

    public bool IsRunning => !Raylib.WindowShouldClose();

    public Camera2D cam;

    TextureRegistry texReg;

    public Vector2 playerPos = Vector2.Zero;
    public int playerSpeed = 3;
    public Vector2 aimVec = Vector2.Zero;

    public void PreWindow() {
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
    }

    public void Init() {
        Console.WriteLine(Directory.GetCurrentDirectory());
        Raylib.MaximizeWindow();
        texReg = new TextureRegistry();
        texReg.Add("map", "assets/map.png", new Vector2(2000, 2000));
        texReg.Add("player", "assets/scary.jpg", new Vector2(50, 50));
        texReg.Load("map");
        texReg.Load("player");
        cam = new Camera2D();
        cam.Rotation = 0;
        cam.Zoom = 2f;
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(0));
    }


    public void Render(double dt) {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
        Raylib.BeginMode2D(cam);
        {
            var tex = texReg.Get("map");
            var player = texReg.Get("player");
            Raylib.DrawTexture(tex, -500, -500, Color.White);
            Raylib.DrawTexture(player, (int) playerPos.X - (player.Width / 2), (int) playerPos.Y - (player.Height / 2), Color.White);
            Raylib.DrawLineEx(playerPos, aimVec, 3f, Color.Red);
        }
        Raylib.EndMode2D();
        {

        }
        Raylib.EndDrawing();
    }

    public void Update(double dt) {
        cam.Offset = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
        cam.Target = playerPos;

        if (Raylib.IsMouseButtonPressed(MouseButton.Middle)) {
            cam.Zoom = 2f;
        }

        if (Raylib.IsKeyDown(KeyboardKey.W)) {
            playerPos.Y -= playerSpeed;
        }
        if (Raylib.IsKeyDown(KeyboardKey.S)) {
            playerPos.Y += playerSpeed;
        }
        if (Raylib.IsKeyDown(KeyboardKey.A)) {
            playerPos.X -= playerSpeed;
        }
        if (Raylib.IsKeyDown(KeyboardKey.D)) {
            playerPos.X += playerSpeed;
        }

        if (Raylib.IsGamepadAvailable(0)) {
            var leftx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftX);
            var lefty = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftY);
            var rightx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightX);
            var righty = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightY);
            var gprightaxis = new Vector2(rightx, righty);
            aimVec = gprightaxis * 100;
            aimVec = aimVec + playerPos;
            var gpleftaxis = new Vector2(leftx, lefty);
            var trig = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftTrigger);
            playerPos += gpleftaxis * playerSpeed * (trig > 0 ? Raymath.Remap(trig, 0, 1, 1, 2) : 1);
        }

        float wheel = Raylib.GetMouseWheelMove();
        if (wheel > 0) {
            cam.Zoom += 0.1f;
        } else if (wheel < 0) {
            cam.Zoom -= 0.1f;
        }
    }

    public void Close() {
    }
}
