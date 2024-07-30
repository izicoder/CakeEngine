using CakeEngine.games;
using Raylib_cs;
using System.Numerics;

namespace CakeEngine;

class Game : ICakeGame {
    #region vars
    Font mainFont;

    int monitorID = 0;
    int monitorHZ => Raylib.GetMonitorRefreshRate(monitorID);
    int targetFPS;

    int fontSize = 18;
    float spacing = 1f;

    Color fontColor = Color.Pink;



    Vector2 dirstep = Vector2.Zero;
    Camera2D playercam;

    Rectangle floorrect = new(-1000, (Raylib.GetScreenHeight() / 4) * 3, new Vector2(10000, 50));

    TextDrawer tx;

    Entity player;
    int currentFrame = 0;

    #endregion vars

    //methods
    public void Init() {

        Raylib.SetTraceLogLevel(TraceLogLevel.Info);
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(800, 600, "test");
        Raylib.DisableEventWaiting();
        Raylib.MaximizeWindow();
        targetFPS = monitorHZ;
        Raylib.SetTargetFPS(targetFPS);


        //var tempimg = Raylib.LoadImage("assets/idle.png");
        ////Raylib.ImageResize(ref tempimg, 100, 100);
        //var playersprite =  Sprite.FromImage(tempimg);
        //playersprite.origin =  Sprite.SetOrigin(playersprite, OriginType.Center);

        var spritesheet = Raylib.LoadImage("assets/idle.png");
        var animSprite = AnimatedSprite.FromSpriteSheet(spritesheet, new Rectangle(0, 0, 64, 128), currentFrame);
        animSprite.ViewSprite.origin = StaticSprite.SetOrigin(animSprite.ViewSprite, OriginType.Center);

        player = new Entity(new Vector2(0, -200), animSprite);

        Console.WriteLine($"frames loaded = {animSprite.Frames.Length}");
        for (int i = 0; i < animSprite.Frames.Length; i++) {
            Console.WriteLine($"f{i} = {animSprite.Frames[i]}");
        }
        Console.WriteLine($"asdasd = {spritesheet.Width / 64}");

        playercam.Target = player.Position;
        playercam.Offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
        playercam.Zoom = 1;
        playercam.Rotation = 0;
        playercam.Target = Vector2.Zero;

        var codepoints = Util.GetCodepoints();
        mainFont = Raylib.LoadFontEx("assets/consola.ttf", fontSize, codepoints, codepoints.Length);
        Raylib.SetTextureFilter(mainFont.Texture, TextureFilter.Point);
        tx = new(mainFont, fontSize, spacing, Color.White);
    }

    bool following = true;
    public void Update(double dt) {
        currentFrame++;

        player.Sprite.CurrentFrame = Util.WrapAround(currentFrame, player.Sprite.Frames.Length);
        currentFrame = player.Sprite.CurrentFrame;

        playercam.Offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);

        if (following) {
            playercam.Target = player.Position;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            playercam.Target = Util.WorldMousePosition(playercam);
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Right)) {
            following = !following;
        }

        if (playercam.Zoom < 0)
            playercam.Zoom = 0.05f;

        //apply gravity

        if (!Raylib.CheckCollisionRecs(player.CollisionRect, floorrect)) {
            player.Position.Y += 5;
        }




        var zoomstep = 0.05f;
        if (Raylib.IsKeyDown(KeyboardKey.LeftControl)) {
            zoomstep = 0.1f;
        }
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift)) {
            zoomstep = 0.5f;
        }

        var newzoom = playercam.Zoom;
        var wheel = Raylib.GetMouseWheelMove();
        if (wheel > 0) {
            newzoom += zoomstep;
        } else if (wheel < 0) {
            newzoom -= zoomstep;
        }
        playercam.Zoom = newzoom > zoomstep ? newzoom : zoomstep;

        if (Raylib.IsMouseButtonPressed(MouseButton.Middle)) {
            playercam.Zoom = 1f;
        }


        if (Raylib.IsKeyDown(KeyboardKey.A)) {
            player.Position.X -= 10;
        }
        if (Raylib.IsKeyDown(KeyboardKey.D)) {
            player.Position.X += 10;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) {
            player.Position.Y -= 200;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.R)) {
            player.Position = Vector2.Zero;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Minus)) {
            targetFPS--;
            Raylib.SetTargetFPS(targetFPS);
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Equal)) {
            targetFPS++;
            Raylib.SetTargetFPS(targetFPS);
        }
    }


    public void Render(double dt) {
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(playercam);
        // 2d camera
        {

            Raylib.DrawRectangle((int) floorrect.X, (int) floorrect.Y, (int) floorrect.Width, (int) floorrect.Height, Color.Yellow);
            Entity.Draw(player);
            Util.DrawRect(player.CollisionRect, Color.Red);

            tx.PrintAt(new Vector2(player.CollisionPoint.X, player.CollisionPoint.Y - fontSize), $"pos/rect: {player.Position}/{player.CollisionRect}");

            var mpos = new Rectangle(Util.WorldMousePosition(playercam) - new Vector2(50, 50), 100, 100);
            Raylib.DrawRectangleLinesEx(mpos, 2f, Color.Orange);


            Raylib.DrawRectangleV(Vector2.Zero, new Vector2(10, 10), Color.Purple);

        }
        //end 2d camera
        Raylib.EndMode2D();

        tx.Print(0, $"fps (monitor/target/real/delta): {monitorHZ}/{targetFPS}/{Raylib.GetFPS()}/{dt:f4}");
        tx.Print(1, $"zoom: {playercam.Zoom}");
        tx.Print(2, $"screen mouse: {Raylib.GetMousePosition()}");
        tx.Print(3, $"world mouse: {Util.WorldMousePosition(playercam)}");
        tx.Print(4, $"cam target: {playercam.Target}");
        tx.Print(5, $"current frame: {player.Sprite.CurrentFrame}");
        tx.Print(6, $"fc: {FrameCounter}");
    }

    uint FrameCounter = 0;
    public void Run(string[] args) {
        Init();
        float delta;
        while (!Raylib.WindowShouldClose()) {
            delta = Raylib.GetFrameTime();

            Update(delta);

            Raylib.BeginDrawing();

            Render(delta);

            Raylib.EndDrawing();
            FrameCounter++;
        }
        Raylib.CloseWindow();
    }
}
