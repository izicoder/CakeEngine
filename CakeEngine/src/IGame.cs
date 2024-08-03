using System.Numerics;

namespace CakeEngine;
interface IGame {
    string InititalTitle {
        get;
    }
    Vector2 InitialWindowSize {
        get;
    }
    bool IsRunning {
        get;
    }

    void PreWindow();
    void Init();
    void Close();

    void Update(double dt);
    void Render(double dt);

}
