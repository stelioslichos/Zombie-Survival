namespace zombie_survival;

public class GameSettings
{
    public int Size { get; set; } = 30;
    public bool Wrap { get; set; } = false;
    public int SleepDelay { get; set; } = 80;
    public int ZombieSpeed { get; set; } = 4;
    public bool TwoPlayerMode { get; set; } = false;
    public string Difficulty { get; set; } = "Medium";

    public GameSettings() { }

    public GameSettings(int size, bool wrap, int sleepDelay, int zombieSpeed, bool twoPlayerMode, string difficulty)
    {
        this.Size = size;
        this.Wrap = wrap;
        this.SleepDelay = sleepDelay;
        this.ZombieSpeed = zombieSpeed;
        this.TwoPlayerMode = twoPlayerMode;
        this.Difficulty = difficulty;
    }
}