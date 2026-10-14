namespace zombie_survival
{
    [System.Serializable]
    public class ScoreRecord
    {
        public int Score = 0;
        public string PlayerName = "--------";

        public ScoreRecord() { }

        public ScoreRecord(int score, string playerName)
        {
            this.Score = score;
            this.PlayerName = playerName;
        }
    }
}