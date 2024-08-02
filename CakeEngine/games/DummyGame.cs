using Raylib_cs;
using System.Numerics;


namespace CakeEngine.games;
internal class DummyGame : IGame {
    public string InititalTitle => "dummy";

    public Vector2 InitialWindowSize => new Vector2(800, 600);

    public bool IsRunning => !Raylib.WindowShouldClose();

    public Camera2D cam;

    TextureRegistry texReg;

    Input input = new Input();

    Vector2 playerPos = Vector2.Zero;
    int playerSpeed = 3;
    Vector2 aimVec = Vector2.Zero;

    public void PreWindow() {
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
    }

    public void Init() {
        input.GamepadStickDeadzone = 0f;
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
            Raylib.DrawFPS(5, 5);
        }
        Raylib.EndDrawing();
    }

    public void Update(double dt) {
        cam.Offset = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
        cam.Target = playerPos;

        if (input.MouseButtonJustPressed(MouseButton.Middle)) {
            cam.Zoom = 2f;
        }

        if (input.IsKeyDown(KeyboardKey.W)) {
            playerPos.Y -= playerSpeed;
        }
        if (input.IsKeyDown(KeyboardKey.S)) {
            playerPos.Y += playerSpeed;
        }
        if (input.IsKeyDown(KeyboardKey.A)) {
            playerPos.X -= playerSpeed;
        }
        if (input.IsKeyDown(KeyboardKey.D)) {
            playerPos.X += playerSpeed;
        }

        if (input.GamepadAvailable) {
            var leftx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftX);
            var lefty = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftY);
            var rightx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightX);
            var righty = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightY);
            var rightstick = input.RightStick();

            Console.WriteLine($"rstick: {rightstick}");
            aimVec = rightstick * 100;
            aimVec = aimVec + playerPos;
            var leftstick = input.LeftStick();

            var trig = input.LeftTrigger();
            playerPos += leftstick * playerSpeed * (trig > 0 ? Raymath.Remap(trig, 0, 1, 1, 2) : 1);
        } else {
            var mousevec = input.MouseWorldPosition(cam);
            mousevec = new Vector2(mousevec.X - playerPos.X, mousevec.Y - playerPos.Y);
            var clamped = Raymath.Vector2ClampValue(mousevec, -1, 1);
            Console.WriteLine($"clamped: {clamped} real: {mousevec}");
            aimVec = clamped * 100;
            aimVec = aimVec + playerPos;
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
