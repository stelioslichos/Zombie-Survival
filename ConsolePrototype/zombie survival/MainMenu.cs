using System;

namespace zombie_survival;

public class MainMenu
{
    public GameSettings Run()
    {
        SaveManager save = new SaveManager();
        SaveData data = save.Load();

        int size = data.Settings.Size;
        bool wrap = data.Settings.Wrap;
        int sleepDelay = data.Settings.SleepDelay;
        int zombieSpeed = data.Settings.ZombieSpeed;
        bool twoPlayer = data.Settings.TwoPlayerMode;
        string difficulty = data.Settings.Difficulty;

        bool readyToStart = false;

        while (!readyToStart)
        {
            Console.Clear();
            Console.WriteLine("====== ZOMBIE SURVIVAL MAIN MENU ======");
            Console.WriteLine($"1. Grid Size  : {size}x{size}");
            Console.WriteLine($"2. Screen Wrap: {(wrap ? "Yes" : "No")}");
            Console.WriteLine($"3. Difficulty : {difficulty}");
            Console.WriteLine($"4. Players    : {(twoPlayer ? "2" : "1")}");
            Console.WriteLine($"5. Highscores");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Press 1, 2, 3 or 4 to change settings.\nPress 5 to see the highscore board");
            Console.WriteLine("Press ENTER to start the game.");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    if (size == 30)
                    {
                        size = 40;
                    }
                    else if (size == 40)
                    {
                        size = 50;
                    }
                    else
                    {
                        size = 30;
                    }
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    wrap = !wrap;
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    if (difficulty == "Easy")
                    {
                        difficulty = "Medium";
                        sleepDelay = 80;
                        zombieSpeed = 4;
                    }
                    else if (difficulty == "Medium")
                    {
                        difficulty = "Hard";
                        sleepDelay = 50;
                        zombieSpeed = 2;
                    }
                    else
                    {
                        difficulty = "Easy";
                        sleepDelay = 100;
                        zombieSpeed = 5;
                    }
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    twoPlayer = !twoPlayer;
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    Console.Clear();
                    Console.WriteLine("===== HIGH SCORE =====");
                    Console.WriteLine($"Player: {data.HighScore.PlayerName}");
                    Console.WriteLine($"Score : {data.HighScore.Score}");
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey(true);
                    break;
                case ConsoleKey.Enter:
                    readyToStart = true;
                    break;
            }
        }
        data.Settings.Size = size;
        data.Settings.Wrap = wrap;
        data.Settings.SleepDelay = sleepDelay;
        data.Settings.ZombieSpeed = zombieSpeed;
        data.Settings.TwoPlayerMode = twoPlayer;
        data.Settings.Difficulty = difficulty;

        save.Save(data);

        return data.Settings;
    }
}
