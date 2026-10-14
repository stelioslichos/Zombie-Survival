using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using zombie_survival;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GridVisualizer visualizer;
    public GameGrid Grid { get; private set; }
    public GameSettings Settings { get; private set; }

    public bool IsGameOver { get; private set; } = false;
    public bool IsGameWon { get; private set; } = false;
    public bool keySpawned = false;
    public bool exitSpawned = false;
    public bool keyMessageShown = false;
    public int p1Score = 0;
    public int p2Score = 0;
    public int targetKillsToEscape = 75;

    public int CurrentScore
    {
        get { return Mathf.Max(p1Score, p2Score); }
    }

    private int tickCounter = 0;
    private System.Random rng =new System.Random();

    private MoveDirection? p1direction = null;
    private MoveDirection? p2direction = null;
    private bool p1Shoot = false;
    private bool p2Shoot = false;

    [Header("Player 1 HUD")]
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p1AmmoText;
    public TextMeshProUGUI p1ZombiesKilledText;
    public GameObject p1KeyIcon;

    [Header("Player 2 HUD")]
    public GameObject p2HUDContainer;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI p2AmmoText;
    public TextMeshProUGUI p2ZombiesKilledText;
    public GameObject p2KeyIcon;

    [Header("In-Game HUD")]
    public TextMeshProUGUI keyNotificationText;
    public GameObject notificationPanel;

    [Header("End Screens")]
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public TextMeshProUGUI loseScoreText;
    public TextMeshProUGUI winScoreText;

    [Header("High Score System")]
    public GameObject highScoreEntryPanel;
    public TMP_InputField nameInputField;

    private SaveData pendingSaveData;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SaveManager save = new SaveManager();
        SaveData data = save.Load();
        Settings = data.Settings;

        if (Settings.Difficulty == "Easy")
        {
            targetKillsToEscape = 25;
        }
        else if (Settings.Difficulty == "Hard")
        {
            targetKillsToEscape = 75;
        }
        else
        {
            targetKillsToEscape = 50;
        }

        if (Settings.Wrap)
        {
            Grid = new WrappingGameGrid(Settings.Size, Settings.Size, Settings.TwoPlayerMode);
        }
        else
        {
            Grid = new GameGrid(Settings.Size, Settings.Size, Settings.TwoPlayerMode);
        }

        SetupDynamicCamera();

        StartCoroutine(GameTickRoutine());

        visualizer.InitializeGrid(Grid);

        Grid.Collectables.Add(new SkeletonKey(rng.Next(Grid.X), rng.Next(Grid.Y)));
        keySpawned = true;
    }

    private void Update()
    {
        if (IsGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenu();
        }

        if (Input.GetKey(KeyCode.W)) p1direction = MoveDirection.Up;
        if (Input.GetKey(KeyCode.S)) p1direction = MoveDirection.Down;
        if (Input.GetKey(KeyCode.A)) p1direction = MoveDirection.Left;
        if (Input.GetKey(KeyCode.D)) p1direction = MoveDirection.Right;
        if (Input.GetKeyDown(KeyCode.Space)) p1Shoot = true;

        if (Input.GetKey(KeyCode.UpArrow)) p2direction = MoveDirection.Up;
        if (Input.GetKey(KeyCode.DownArrow)) p2direction = MoveDirection.Down;
        if (Input.GetKey(KeyCode.LeftArrow)) p2direction = MoveDirection.Left;
        if (Input.GetKey(KeyCode.RightArrow)) p2direction = MoveDirection.Right;
        if (Input.GetKeyDown(KeyCode.Return)) p2Shoot = true;

        if (p1ScoreText != null)
        {
            p1ScoreText.text = "Score: " + p1Score;
        }

        if (p1AmmoText != null && Grid != null && Grid.Player != null)
        {
            p1AmmoText.text = "Ammo: " + Grid.Player.Ammo;
        }

        if (p1ZombiesKilledText != null && Grid != null && Grid.Player != null)
        {
            p1ZombiesKilledText.text = "Zombies killed: " + Grid.Player.Kills;
        }

        if (p1KeyIcon != null && Grid != null && Grid.Player != null)
        {
            p1KeyIcon.SetActive(Grid.Player.HasKey);
        }

        if (Settings.TwoPlayerMode && p2HUDContainer != null)
        {
            p2HUDContainer.SetActive(true);

            if (p2ScoreText != null)
            {
                p2ScoreText.text = "Score: " + p2Score;
            }

            if (p2AmmoText != null && Grid != null && Grid.Player2 != null)
            {
                p2AmmoText.text = "Ammo: " + Grid.Player2.Ammo;
            }

            if (p2ZombiesKilledText != null && Grid != null && Grid.Player2 != null)
            {
                p2ZombiesKilledText.text = "Zombies killed: " + Grid.Player2.Kills;
            }

            if (p2KeyIcon != null && Grid != null && Grid.Player2 != null)
            {
                p2KeyIcon.SetActive(Grid.Player2.HasKey);
            }
        }
        else if (!Settings.TwoPlayerMode && p2HUDContainer != null)
        {
            p2HUDContainer.SetActive(false);
        }
    }

    private void SetupDynamicCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        float centerX = Grid.X / 2f;
        float centerY = Grid.Y / 2f;

        mainCam.transform.position = new Vector3(centerX, centerY, -10f);

        float padding = 1.5f;
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float gridRatio = (float)Grid.X / (float)Grid.Y;

        if (screenRatio >= gridRatio)
        {
            mainCam.orthographicSize = (Grid.Y / 2f) + padding;
        }
        else
        {
            mainCam.orthographicSize = ((Grid.X / 2f) + padding) / screenRatio;
        }
    }

    private IEnumerator GameTickRoutine()
    {
        float tickRate = Settings.SleepDelay / 1000f;

        while (!IsGameOver)
        {
            ProcessGameTick();
            visualizer.UpdateVisuals(Grid);

            p1direction = null; p2direction = null;
            p1Shoot = false; p2Shoot = false;

            yield return new WaitForSeconds(tickRate);
        }

        SaveManager save = new SaveManager();
        pendingSaveData = save.Load();

        string winner = "";
        if (Settings.TwoPlayerMode)
        {
            if (p1Score > p2Score)
            {
                winner = "Player 1 Wins\n";
            }
            else if (p1Score < p2Score)
            {
                winner = "Player 2 Wins\n";
            }
            else
            {
                winner = "It's a Tie!\n";
            }
        }

        if (IsGameWon)
        {
            winPanel.SetActive(true);
            winScoreText.text = winner + "Final Score: " + CurrentScore;
        }
        else
        {
            gameOverPanel.SetActive(true);
            loseScoreText.text = winner + "Final Score: " + CurrentScore;
        }

        int previousHighScore = 0;
        if (pendingSaveData != null && pendingSaveData.HighScore != null)
        {
            previousHighScore = pendingSaveData.HighScore.Score;
        }
        if (CurrentScore > previousHighScore)
        {
            highScoreEntryPanel.SetActive(true);
        }
    }

    public void SubmitScore()
    {
        string pname = nameInputField.text;
        if (string.IsNullOrWhiteSpace(pname)) pname = "Survivor";

        pendingSaveData.HighScore.Score = CurrentScore;
        pendingSaveData.HighScore.PlayerName = pname;

        SaveManager save = new SaveManager();
        save.Save(pendingSaveData);

        LoadMainMenu();
    }

    private void ProcessGameTick()
    {
        if (IsGameOver) return;

        int totalKills = Grid.Player.Kills;

        if (Grid.Player2 != null)
        {
            totalKills += Grid.Player2.Kills;
        }

        tickCounter++;

        if (tickCounter % 100 == 0)
        {
            p1Score += 1;
            if (Grid.Player2 != null) p2Score += 1;
        }

        if (tickCounter % 200 == 0)
        {
            Grid.Collectables.Add(new AmmoCrate(rng.Next(Grid.X), rng.Next(Grid.Y)));
        }
 
        if (p1Shoot && !Grid.Player.IsDead && Grid.Player.Shoot())
        {
            p1Score += Grid.FireShotgun(Grid.Player);
            if (visualizer != null) visualizer.SpawnShotEffects(Grid.Player);
        }

        if (p2Shoot && Grid.Player2 != null && !Grid.Player2.IsDead && Grid.Player2.Shoot())
        {
            p2Score += Grid.FireShotgun(Grid.Player2);
            if (visualizer != null) visualizer.SpawnShotEffects(Grid.Player2);
        }

        int playerSpeed = 2;

        if (tickCounter % playerSpeed == 0)
        {
            if (!Grid.Player.IsDead && p1direction.HasValue)
            {
                Grid.Player.FacingDirection = p1direction.Value;
                Grid.MakeMove(Grid.Player, p1direction.Value);
                Grid.CheckCollectables(Grid.Player);
            }
            if (Grid.Player2 != null && !Grid.Player2.IsDead && p2direction.HasValue)
            {
                Grid.Player2.FacingDirection = p2direction.Value;
                Grid.MakeMove(Grid.Player2, p2direction.Value);
                Grid.CheckCollectables(Grid.Player2);
            }
        }

        bool p1Caught = !Grid.Player.IsDead && Grid.IsZombieAt(Grid.Player.Location.X, Grid.Player.Location.Y);
        bool p2Caught = Grid.Player2 != null && !Grid.Player2.IsDead && Grid.IsZombieAt(Grid.Player2.Location.X, Grid.Player2.Location.Y);

        if (p1Caught) Grid.Player.Die();
        if (p2Caught) Grid.Player2.Die();

        if (tickCounter % Settings.ZombieSpeed == 0)
        {
            Grid.MoveZombie();

            p1Caught = !Grid.Player.IsDead && Grid.IsZombieAt(Grid.Player.Location.X, Grid.Player.Location.Y);
            p2Caught = Grid.Player2 != null && !Grid.Player2.IsDead && Grid.IsZombieAt(Grid.Player2.Location.X, Grid.Player2.Location.Y);

            if (p1Caught) Grid.Player.Die();
            if (p2Caught) Grid.Player2.Die();

            if (Grid.Player.IsDead && (Grid.Player2 == null || Grid.Player2.IsDead))
            {
                IsGameOver = true;
            }
        }

        if (tickCounter % 125 == 0)
        {
            Grid.Zombies.Add(new Zombie(0, Grid.Y - 1));
        }

        bool keyFound = Grid.Player.HasKey || (Grid.Player2 != null && Grid.Player2.HasKey);

        if (keyFound && !keyMessageShown)
        {
            keyMessageShown = true;

            if (totalKills < targetKillsToEscape)
            {
                ShowNotification($"KEY FOUND! Now kill {targetKillsToEscape - totalKills} more zombies!", 7f);
            }
        }

        if (keyFound && totalKills >= targetKillsToEscape && !exitSpawned)
        {
            Grid.Collectables.Add(new ExitPortal(Grid.X / 2, Grid.Y / 2));
            exitSpawned = true;

            ShowNotification("EXIT OPEN! Get to the exit portal", 7f);
        }

        bool p1Active = !Grid.Player.IsDead && !Grid.Player.HasEscaped;
        bool p2Active = Grid.Player2 != null && !Grid.Player2.IsDead && !Grid.Player2.HasEscaped;

        if (!p1Active && !p2Active)
        {
            IsGameOver = true;

            bool p1Win = Grid.Player.HasEscaped;
            bool p2Win = (Grid.Player2 == null) || Grid.Player2.HasEscaped;

            if (p1Win && p2Win)
            {
                IsGameWon = true;
            }
        }
    }

    public void ShowNotification(string message, float duration)
    {
        if (keyNotificationText == null) return;

        StartCoroutine(NotificationRoutine(message, duration));
    }

    private IEnumerator NotificationRoutine(string message, float duration)
    {
        keyNotificationText.text = message;
        keyNotificationText.gameObject.SetActive(true);

        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);

            yield return new WaitForSeconds(duration);
        }

        keyNotificationText.gameObject.SetActive(false);
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}