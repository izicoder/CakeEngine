namespace CakeEngine.games;
interface ICakeGame {
    void Run(string[] args);
    void Init();
    void Update(double dt);
    void Render(double dt);
}
