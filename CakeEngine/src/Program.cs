using Raylib_cs;
using System.Numerics;

namespace CakeEngine;

class Program {
    //fields
    static Font mainFont;

    static int monitor = 0;
    static int targetFPS = Raylib.GetMonitorRefreshRate(monitor);

    static int fontSize = 32;
    static float spacing = 1f;

    static Color fontColor = Color.Pink;

    static List<(Vector2, bool)> points = [];


    static Vector2 dirstep = Vector2.Zero;
    static Rectangle playerrect = new(0, 0, new Vector2(200, 200));
    static Texture2D playertex;
    static Camera2D playercam;
    static Vector2 playertarget => new(playerrect.X + playerrect.Width / 2, playerrect.Y + playerrect.Height / 2);

    static Rectangle floorrect = new(-1000, (int) ((Raylib.GetScreenHeight() / 4) * 3), new Vector2(10000, 50));

    static TextDrawer tx;

    static InputThread input;
    static VK_KEY[] keymap = [VK_KEY.A, VK_KEY.D, VK_KEY.Space, VK_KEY.Escape];
    //methods
    static void Init() {

        Raylib.SetTraceLogLevel(TraceLogLevel.Info);
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(800, 600, "test");
        Raylib.SetTargetFPS(targetFPS);

        input = new InputThread(500);
        input.Start(keymap);

        var tempimg = Raylib.LoadImage("assets/scary.jpg");
        Raylib.ImageResize(ref tempimg, (int) playerrect.Width, (int) playerrect.Height);
        playertex = Raylib.LoadTextureFromImage(tempimg);

        playercam.Target = new Vector2(playerrect.X + playerrect.Width / 2, playerrect.Y + playerrect.Height / 2);
        playercam.Offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
        playercam.Zoom = 1;
        playercam.Rotation = 0;

        var codepoints = Util.GetCodepoints();
        mainFont = Raylib.LoadFontEx("assets/consola.ttf", fontSize, codepoints, codepoints.Length);
        Raylib.SetTextureFilter(mainFont.Texture, TextureFilter.Point);
        tx = new(mainFont, fontSize, spacing, Color.White);
    }


    static void Update(double dt) {
        playercam.Offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);

        playercam.Target = playertarget;
        if (playercam.Zoom < 0)
            playercam.Zoom = 0.1f;


        //if (Raylib.IsMouseButtonDown(MouseButton.Left)) {
        //    Vector2 pos = Raylib.GetMousePosition();
        //    points.Add((pos, true));
        //}

        //if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        //    points.Clear();

        //apply gravity

        if (!Raylib.CheckCollisionRecs(playerrect, floorrect)) {
            playerrect.Y += 5;
        }


        var wheel = Raylib.GetMouseWheelMove();
        //int step = 10;
        //if (Raylib.IsKeyDown(KeyboardKey.LeftControl))
        //    step = 1;
        //else if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
        //    step = 100;

        //if (wheel > 0) {
        //    targetFPS += step;
        //    Raylib.SetTargetFPS(targetFPS);
        //} else if (wheel < 0) {
        //    targetFPS -= step;
        //    targetFPS = targetFPS > 0 ? targetFPS : step;
        //    Raylib.SetTargetFPS(targetFPS);
        //} else if (Raylib.IsKeyPressed(KeyboardKey.F)) {
        //    targetFPS = Raylib.GetMonitorRefreshRate(monitor);
        //    Raylib.SetTargetFPS(targetFPS);
        //}
        if (wheel > 0) {
            playercam.Zoom += 0.1f;
        } else if (wheel < 0) {
            playercam.Zoom -= 0.1f;
        }
        if (Raylib.IsMouseButtonPressed(MouseButton.Middle)) {
            playercam.Zoom = 1f;
        }


        if (input.IsPressed[VK_KEY.A]) {
            playerrect.X -= 10;
        }
        if (input.IsPressed[VK_KEY.D]) {
            playerrect.X += 10;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) {
            playerrect.Y -= 200;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.R)) {
            playerrect = new Rectangle(50, 50, playerrect.Size);
        }

        if (points.Count > 0) {
            points.RemoveAll(x => x.Item2 == false);

            var screenRect = new Rectangle(0, 0, Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
            for (int i = 0; i < points.Count; i++) {
                var oldp = points[i];
                if (!Raylib.CheckCollisionPointRec(oldp.Item1, screenRect)) {
                    points[i] = (oldp.Item1, false);
                }
            }


            for (int i = 0; i < points.Count; i++) {
                var oldp = points[i];
                var newp = (new Vector2(oldp.Item1.X, oldp.Item1.Y) + dirstep, oldp.Item2);
                points[i] = newp;
            }
        }
    }


    static unsafe void Render(double dt) {
        Raylib.ClearBackground(Color.Black);

        //for (int i = 0; i < points.Count; i++) {
        //    var p1 = points[i];
        //    var p2index = (i + 1) % points.Count;
        //    var p2 = points[p2index];
        //    Raylib.DrawLine((int) p1.Item1.X, (int) p1.Item1.Y, (int) p2.Item1.X, (int) p2.Item1.Y, Color.Orange);
        //    Raylib.DrawTexture(playertex, (int) p1.Item1.X, (int) p1.Item1.Y, Color.White);
        //}

        Raylib.BeginMode2D(playercam);
        // 2d camera

        Raylib.DrawRectangle((int) floorrect.X, (int) floorrect.Y, (int) floorrect.Width, (int) floorrect.Height, Color.Yellow);
        Raylib.DrawTexture(playertex, (int) playerrect.X, (int) playerrect.Y, Color.White);

        Raylib.DrawText($"x: {playerrect.X} y: {playerrect.Y}", (int) playerrect.X, (int) playerrect.Y, 12, Color.White);

        //end 2d camera
        Raylib.EndMode2D();

        tx.Print($"fps (target/real/delta): {targetFPS}/{Raylib.GetFPS()}/{dt:f4}", 0);
        tx.Print($"pixels: {points.Count}", 1);
        tx.Print($"zoom: {playercam.Zoom}", 2);
        string kb = "";
        foreach (var k in keymap) {
            kb += $"{k}:{(input.IsPressed[k] ? 1 : 0)} ";
        }
        tx.Print($"keys: {kb}", 4);
        tx.Print($"input queue: {input.IsPressed.Count}", 5);
        tx.Print($"raw mouse x/y: {input.MouseX}/{input.MouseY}", 6);
        tx.Print($"ray mouse x/y: {Raylib.GetMousePosition()}", 7);
        tx.Print($"main thread id: {Thread.CurrentThread.ManagedThreadId}", 8);

    }

    static float LastFrameTime = 0;
    static void Main(string[] args) {
        Init();
        while (!Raylib.WindowShouldClose()) {
            LastFrameTime = Raylib.GetFrameTime();
            Update(LastFrameTime);
            Raylib.BeginDrawing();
            Render(LastFrameTime);
            Raylib.EndDrawing();
        }
        input.Stop();
        Raylib.CloseWindow();
    }
}
