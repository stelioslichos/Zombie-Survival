namespace zombie_survival;

public class ScoreRecord
{
    public int Score {  get; set; } = 0;
    public string PlayerName { get; set; } = "--------";

    public ScoreRecord() { }

    public ScoreRecord(int score, string playerName)
    {
        this.Score = score;
        this.PlayerName = playerName;
    }
}