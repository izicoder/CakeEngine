using CakeEngine.games;

namespace CakeEngine;

class Program {
    static void Main(string[] args) {
        var runner = new GameRunner();
        runner.Load(new DummyGame());
        runner.Run();
    }
}
