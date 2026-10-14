namespace zombie_survival
{
    [System.Serializable]
    public class GameSettings
    {
        public int Size = 30;
        public bool Wrap = false;
        public int SleepDelay = 80;
        public int ZombieSpeed = 4;
        public bool TwoPlayerMode = false;
        public string Difficulty = "Medium";

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
}