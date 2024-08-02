using Raylib_cs;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;

namespace CakeEngine;

record InputMap {
    public Tuple<KeyboardKey, string>[] KeyboardEvents = [];
    public Tuple<MouseButton, string>[] MouseEvents = [];
    public Tuple<GamepadButton, string>[] GamepadEvents = [];

    public InputMap(string filename) {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var save = JsonSerializer.Deserialize<InputMap>(File.ReadAllText(filename));
        if (save != null) {
            KeyboardEvents = save.KeyboardEvents;
            MouseEvents = save.MouseEvents;
            GamepadEvents = save.GamepadEvents;
        } else {
            throw new JsonException("Input Map Problem");
        }
    }
}

class KeyboardInput {
    public ConcurrentDictionary<KeyboardKey, bool> IsDown = [];

    public KeyboardInput() {

    }
}

class MouseInput {
    public ConcurrentDictionary<MouseButton, bool> IsDown = [];

    public MouseInput() {
    }

}

class GamepadInput {
    public ConcurrentDictionary<GamepadButton, bool> IsDown = [];

    public GamepadInput() {

    }
}

class Input {
    public float GamepadStickDeadzone = 0.1f;
    public int GamepadID = 0;
    public bool GamepadAvailable => Raylib.IsGamepadAvailable(GamepadID);

    public Vector2 MousePosition => Raylib.GetMousePosition();

    public Vector2 MouseWorldPosition(Camera2D cam) {
        return Raylib.GetScreenToWorld2D(MousePosition, cam);
    }

    public bool IsKeyDown(KeyboardKey key) {
        return Raylib.IsKeyDown(key);
    }

    public bool IsKeyUp(KeyboardKey key) {
        return Raylib.IsKeyUp(key);
    }

    public bool KeyJustPressed(KeyboardKey key) {
        return Raylib.IsKeyPressed(key);
    }

    public bool KeyJustReleased(KeyboardKey key) {
        return Raylib.IsKeyReleased(key);
    }

    public float MouseWheel() {
        return Raylib.GetMouseWheelMove();
    }

    public Vector2 LeftStick() {
        var x = Raylib.GetGamepadAxisMovement(GamepadID, GamepadAxis.LeftX);
        var y = Raylib.GetGamepadAxisMovement(GamepadID, GamepadAxis.LeftY);
        Vector2 res = Vector2.Zero;
        res.X = x > GamepadStickDeadzone || x < (-GamepadStickDeadzone) ? x : 0;
        res.Y = y > GamepadStickDeadzone || y < (-GamepadStickDeadzone) ? y : 0;
        return res;
    }

    public Vector2 RightStick() {
        var x = Raylib.GetGamepadAxisMovement(GamepadID, GamepadAxis.RightX);
        var y = Raylib.GetGamepadAxisMovement(GamepadID, GamepadAxis.RightY);
        Vector2 res = Vector2.Zero;
        res.X = x > GamepadStickDeadzone || x < (-GamepadStickDeadzone) ? x : 0;
        res.Y = y > GamepadStickDeadzone || y < (-GamepadStickDeadzone) ? y : 0;
        return res;
    }

    public float LeftTrigger() {
        return Raylib.GetGamepadAxisMovement(GamepadID, GamepadAxis.LeftTrigger);
    }

    public float RightTrigger() {
        return Raylib.GetGamepadAxisMovement(GamepadID, GamepadAxis.RightTrigger);
    }

    public bool IsButtonDown(GamepadButton button) {
        return Raylib.IsGamepadButtonDown(GamepadID, button);
    }

    public bool IsButtonUp(GamepadButton button) {
        return Raylib.IsGamepadButtonUp(GamepadID, button);
    }

    public bool ButtonJustPressed(GamepadButton button) {
        return Raylib.IsGamepadButtonPressed(GamepadID, button);
    }

    public bool ButtonJustReleased(GamepadButton button) {
        return Raylib.IsGamepadButtonReleased(GamepadID, button);
    }

    public bool IsMouseButtonDown(MouseButton button) {
        return Raylib.IsMouseButtonDown(button);
    }
    public bool IsMouseButtonUp(MouseButton button) {
        return Raylib.IsMouseButtonUp(button);
    }
    public bool MouseButtonJustPressed(MouseButton button) {
        return Raylib.IsMouseButtonPressed(button);
    }
    public bool MouseButtonJustReleased(MouseButton button) {
        return Raylib.IsMouseButtonReleased(button);
    }

}
