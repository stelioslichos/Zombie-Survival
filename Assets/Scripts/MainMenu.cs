using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using zombie_survival;

public class MainMenu : MonoBehaviour
{
    private SaveManager save;
    private SaveData data;

    [Header("Logic References")]

    public GameObject mainMenuContainer;

    [Header("UI Text References")]
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI screenWrapText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI playerText;

    [Header("Highscore References")]
    public GameObject HighScore;
    public TextMeshProUGUI highscoreContent;

    void Start()
    {
        save = new SaveManager();
        data = save.Load();

        if (data == null)
        {
            data = new SaveData();
        }

        UpdateUI();
    }

    public void ToggleGridSize()
    {
        if (data.Settings.Size == 15) data.Settings.Size = 20;
        else if (data.Settings.Size == 20) data.Settings.Size = 30;
        else data.Settings.Size = 15;
        UpdateUI();
        save.Save(data);
    }

    public void ToggleScreenWrap()
    {
        data.Settings.Wrap = !data.Settings.Wrap;
        UpdateUI();
        save.Save(data);
    }

    public void ToggleDifficulty()
    {
        if (data.Settings.Difficulty == "Easy")
        {
            data.Settings.Difficulty = "Medium";
            data.Settings.SleepDelay = 80;
            data.Settings.ZombieSpeed = 4;
        }
        else if (data.Settings.Difficulty == "Medium") 
        {
            data.Settings.Difficulty = "Hard";
            data.Settings.SleepDelay = 50;
            data.Settings.ZombieSpeed = 2;
        }
        else
        {
            data.Settings.Difficulty = "Easy";
            data.Settings.SleepDelay = 100;
            data.Settings.ZombieSpeed = 5;
        }
            UpdateUI();
        save.Save(data);
    }

    public void TogglePlayers()
    {
        data.Settings.TwoPlayerMode = !data.Settings.TwoPlayerMode;
        UpdateUI();
        save.Save(data);
    }

    public void StartGame()
    {
        save.Save(data);
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateUI()
    {
        sizeText.text = $"Grid Size: {data.Settings.Size}x{data.Settings.Size}";
        screenWrapText.text = $"Screen Wrap: {(data.Settings.Wrap ? "Yes" : "No")}";
        difficultyText.text = $"Difficulty: {data.Settings.Difficulty}";
        playerText.text = $"Players: {(data.Settings.TwoPlayerMode ? "2" : "1")}";
    }

    public void OpenHighscores()
    {
        mainMenuContainer.SetActive(false);
        
        if (data.HighScore != null && data.HighScore.Score > 0)
        {
            highscoreContent.text = $"LEGENDARY SURVIVOR\n\n" +
                                     $"Name: {data.HighScore.PlayerName}\n" +
                                     $"Score: {data.HighScore.Score}";
        }
        else
        {
            highscoreContent.text = "NO SURVIVORS YET...";
        }

        HighScore.SetActive(true);
    }

    public void CloseHighscores()
    {
        mainMenuContainer.SetActive(true);
        HighScore.SetActive(false);
    }
}
