using Raylib_cs;
using System.Collections.Concurrent;
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
}
