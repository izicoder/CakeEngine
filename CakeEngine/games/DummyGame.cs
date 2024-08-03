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

    FMOD.System fmod;
    FMOD.Sound song;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelgroup;
    FMOD.RESULT fmodres;

    float channelVolume;
    float channelPan = 0;
    bool isPlaying = false;
    Dictionary<int, uint> channelPlaybackPositions = new();
    int numchannels;

    TextDrawer tx;
    public void PreWindow() {
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
    }

    void logRes(string msg) {
        Console.WriteLine($"{msg}: {fmodres}");
    }
    public void Init() {
        input.GamepadStickDeadzone = 0f;
        Console.WriteLine(Directory.GetCurrentDirectory());
        Raylib.SetWindowPosition(1000, 50);
        //Raylib.MaximizeWindow();
        texReg = new TextureRegistry();
        texReg.Add("map", "assets/map.png", new Vector2(2000, 2000));
        texReg.Add("player", "assets/scary.jpg", new Vector2(50, 50));
        texReg.Load("map");
        texReg.Load("player");
        cam = new Camera2D();
        cam.Rotation = 0;
        cam.Zoom = 2f;
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(0));


        FMOD.Factory.System_Create(out fmod);
        fmodres = fmod.init(100, FMOD.INITFLAGS.PROFILE_ENABLE | FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        logRes("sys init");

        fmod.createChannelGroup("master", out channelgroup);
        fmod.createSound("assets/deco.mp3", FMOD.MODE._3D_LINEARROLLOFF, out song);
        channelgroup.setVolume(0.5f);

        tx = new TextDrawer(Raylib.GetFontDefault(), 22, 1.2f, Color.White);
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
            tx.Print(0, $"fps: {Raylib.GetFPS()}");
            tx.Print(1, $"volume: {channelVolume}");
            tx.Print(2, $"playing: {isPlaying}");
            tx.Print(3, $"channels: {numchannels}");
            tx.Print(4, $"pan: {channelPan}");
            foreach (var e in channelPlaybackPositions) {
                tx.Print(5 + e.Key, $"ch {e.Key}: {e.Value}");
            }
        }
        Raylib.EndDrawing();
    }

    public void Update(double dt) {
        fmod.update();
        fmod.getChannelsPlaying(out numchannels);
        channelgroup.getVolume(out channelVolume);
        channelgroup.getNumChannels(out numchannels);


        for (int i = 0; i < numchannels; i++) {
            FMOD.Channel ch;
            channelgroup.getChannel(i, out ch);
            uint pos;
            ch.getPosition(out pos, FMOD.TIMEUNIT.MS);
            channelPlaybackPositions[i] = pos;
        }

        cam.Offset = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
        cam.Target = playerPos;

        if (input.KeyJustPressed(KeyboardKey.V)) {
            fmod.playSound(song, channelgroup, false, out channel);
            isPlaying = true;
        }
        if (input.KeyJustPressed(KeyboardKey.Space)) {
            if (isPlaying) {
                channelgroup.setPaused(true);
                isPlaying = false;
            } else {
                channelgroup.setPaused(false);
                isPlaying = true;
            }
        }
        if (input.KeyJustPressed(KeyboardKey.Up)) {
            channelVolume += 0.1f;
            channelgroup.setVolume(channelVolume);
        }
        if (input.KeyJustPressed(KeyboardKey.Down)) {
            channelVolume -= 0.1f;
            channelgroup.setVolume(channelVolume);
        }
        if (input.KeyJustPressed(KeyboardKey.Left)) {
            channelPan -= 0.1f;
            channelgroup.setPan(channelPan);
        }
        if (input.KeyJustPressed(KeyboardKey.Right)) {
            channelPan += 0.1f;
            channelgroup.setPan(channelPan);
        }

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

            //Console.WriteLine($"rstick: {rightstick}");
            aimVec = rightstick * 100;
            aimVec = aimVec + playerPos;
            var leftstick = input.LeftStick();

            var trig = input.LeftTrigger();
            playerPos += leftstick * playerSpeed * (trig > 0 ? Raymath.Remap(trig, 0, 1, 1, 2) : 1);
        } else {
            var mousevec = input.MouseWorldPosition(cam);
            mousevec = new Vector2(mousevec.X - playerPos.X, mousevec.Y - playerPos.Y);
            var clamped = Raymath.Vector2ClampValue(mousevec, -1, 1);
            //Console.WriteLine($"clamped: {clamped} real: {mousevec}");
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
        fmodres = fmod.close();
        logRes("end");
        fmod.release();
    }
}
