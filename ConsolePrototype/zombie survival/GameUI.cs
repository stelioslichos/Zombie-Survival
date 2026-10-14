using System;
using System.Threading;

namespace zombie_survival;

public class GameUI
{
	private static GameUI uiInstance;
	private Game game;
	int sleepDelay;

	private GameUI() { }
	
	public static GameUI Instance()
	{
		if (uiInstance == null)
		{
			uiInstance = new GameUI();
		}
		return uiInstance;
	}

	public void InitialiseGame()
	{
		MainMenu menu = new MainMenu();
		GameSettings settings = menu.Run();

		this.sleepDelay = settings.SleepDelay;

		GameGrid selectedGrid;
		if (settings.Wrap)
		{
			selectedGrid = new WrappingGameGrid(settings.Size, settings.Size, settings.TwoPlayerMode);
		}
		else
		{
			selectedGrid = new GameGrid(settings.Size, settings.Size, settings.TwoPlayerMode);
		}
		game = new Game(selectedGrid, settings.ZombieSpeed);
	}

	public void Run()
	{
        bool isRunning = true;
		
        Console.Clear();
		Console.WriteLine("=== ZOMBIE SURVIVOR ===");
		Console.WriteLine("Press 'Q' to exit.\n");

		int startLeft = Console.CursorLeft;
		int startTop = Console.CursorTop;

		while (isRunning && !game.IsGameOver)
		{
			Console.SetCursorPosition(startLeft, startTop);
			if (game.Grid != null)
			{
				Console.WriteLine(game.Grid.ToString());
			}

			if (game.exitSpawned)
			{
				Console.WriteLine(">>>>> KEY FOUND! Get to the exit portal <<<<<");
			}
			else
			{
				Console.WriteLine("                                             ");
			}

			MoveDirection? p1direction = null;
            MoveDirection? p2direction = null;
            bool p1Shoot = false;
            bool p2Shoot = false;

            if (Console.KeyAvailable)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
				while (Console.KeyAvailable) Console.ReadKey(true);

				switch (keyInfo.Key)
				{
					case ConsoleKey.W:
                        p1direction = MoveDirection.Up;
						break;
                    case ConsoleKey.UpArrow:
						p2direction = MoveDirection.Up;
						break;
					case ConsoleKey.S:
                        p1direction = MoveDirection.Down;
                        break;
                    case ConsoleKey.DownArrow:
                        p2direction = MoveDirection.Down;
                        break;
					case ConsoleKey.A:
                        p1direction = MoveDirection.Left;
                        break;
                    case ConsoleKey.LeftArrow:
                        p2direction = MoveDirection.Left;
                        break;
					case ConsoleKey.D:
                        p1direction = MoveDirection.Right;
                        break;
                    case ConsoleKey.RightArrow:
                        p2direction = MoveDirection.Right;
                        break;
					case ConsoleKey.Q:
						isRunning = false;
						break;
					case ConsoleKey.Spacebar:
						p1Shoot = true;
						break;
					case ConsoleKey.Enter:
						p2Shoot = true;
						break;
                }
			}
			game.Update(p1direction, p2direction, p1Shoot, p2Shoot);

			Thread.Sleep(sleepDelay);
		}

		Console.Clear();
		Console.WriteLine("=== ZOMBIE SURVIVOR ===");

		if (game != null && game.Grid != null)
		{
			Console.WriteLine(game.Grid.ToString());
		}
		if (game.IsGameWon)
		{
			Console.WriteLine("=============================================");
			Console.WriteLine("             *** YOU ESCAPED! ***            ");
			Console.WriteLine("   You survived the swarm and found the exit! ");
			Console.WriteLine($"                Final Score: {game.CurrentScore}");
			Console.WriteLine("=============================================");
		}
		else
		{
			Console.WriteLine("=============================================");
			Console.WriteLine("                GAME OVER                    ");
			Console.WriteLine("       The zombies finally got you...        ");
			Console.WriteLine($"           Final Score: {game.CurrentScore} ");
			Console.WriteLine("=============================================");
		}

        SaveManager save = new SaveManager();
        SaveData data = save.Load();

        if (game.CurrentScore > data.HighScore.Score)
		{
			Console.WriteLine("\n***** NEW HIGH SCORE! *****");
			Console.WriteLine("Enter your name: ");
			string playerName = Console.ReadLine();
			data.HighScore.Score = game.CurrentScore;
			data.HighScore.PlayerName = playerName;
			save.Save(data);
		}
	
		Console.WriteLine("Thank you for playing!");
		Console.ReadLine();
	}
}