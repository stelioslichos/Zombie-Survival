namespace zombie_survival;

public class SaveData
{
	public GameSettings Settings { get; set; } = new GameSettings();
    public ScoreRecord HighScore { get; set; } = new ScoreRecord();
}