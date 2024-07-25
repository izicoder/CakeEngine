using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;

namespace CakeEngine;
class InputThread {

    private readonly CancellationTokenSource _cancelSource = new();

    // keyboard status
    public ConcurrentDictionary<VK_KEY, bool> IsPressed = new();
    // mouse status
    public volatile int MouseX = 0;
    public volatile int MouseY = 0;

    private VK_KEY[] _keymap = [];
    public int sleepTime = 16;

    public InputThread(int targetUPS) {
        sleepTime = 1000 / targetUPS;
    }

    public void Start(VK_KEY[] keymap) {
        _keymap = keymap;
        foreach (VK_KEY key in keymap) {
            IsPressed.TryAdd(key, false);
        }
        Thread thread = new(() => PollInputs(_cancelSource.Token));
        thread.Start();
    }

    public void Stop() {
        Console.WriteLine("exiting thread");
        _cancelSource.Cancel();
    }

    private void PollInputs(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            // poll mouse pos
            (MouseX, MouseY) = User32.GetMousePositionRaw();

            // poll keyboard
            foreach (VK_KEY key in _keymap) {
                bool key_state = User32.IsPressed(key);
                //Console.WriteLine($"polling {key} = {key_state}");
                IsPressed[key] = key_state;
            }
            Console.WriteLine($"sleeping {Thread.CurrentThread.ManagedThreadId} for {sleepTime}ms");
            Thread.Sleep(sleepTime);
        }
    }


    ~InputThread() {
        _cancelSource.Dispose();
        Console.WriteLine("disposing token???");
    }
}
