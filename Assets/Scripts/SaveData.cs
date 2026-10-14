namespace zombie_survival
{
    [System.Serializable]
    public class SaveData
    {
        public GameSettings Settings = new GameSettings();
        public ScoreRecord HighScore = new ScoreRecord();
    }
}