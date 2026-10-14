using System;

namespace zombie_survival;

public class Game
{
    public GameGrid Grid { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsGameWon { get; private set; }
    public bool keySpawned;
    public bool exitSpawned;
    private int tickCounter = 0;
    private int zombieSpeed;
    private Random rng = new Random();
    public int CurrentScore { get; set; }

    public Game(GameGrid grid, int zombieSpeed)
    {
        Grid = grid;
        this.zombieSpeed = zombieSpeed;
        IsGameOver = false;
        IsGameWon = false;
        keySpawned = false;
        exitSpawned = false;
    }

    public void Update(MoveDirection? p1direction, MoveDirection? p2direction, bool p1Shoot, bool p2Shoot)
    {
        if (IsGameOver) return;

        int totalKills = Grid.Player.Kills;

        if (Grid.Player2 != null)
        {
            totalKills += Grid.Player2.Kills;
        }

        if (totalKills >= 50 && !keySpawned)
        {
            Grid.Collectables.Add(new SkeletonKey(rng.Next(Grid.X), rng.Next(Grid.Y)));
            keySpawned = true;
        }

        tickCounter++;

        if (tickCounter % 100 == 0)
        {
            CurrentScore += 1;
        }

        if (tickCounter % 125 == 0)
        {
            Grid.Zombies.Add(new Zombie(0, Grid.Y - 1));
        }

        if  (tickCounter % 200 == 0)
        {
            Grid.Collectables.Add(new AmmoCrate(rng.Next(Grid.X), rng.Next(Grid.Y)));
        }

        if (p1Shoot && !Grid.Player.IsDead && Grid.Player.Shoot())
        {
            CurrentScore += Grid.FireShotgun(Grid.Player);
        }

        if (p2Shoot && Grid.Player2 != null && !Grid.Player2.IsDead && Grid.Player2.Shoot())
        {
            CurrentScore += Grid.FireShotgun(Grid.Player2);
        }

        if (!Grid.Player.IsDead && p1direction.HasValue)
        {
            Grid.MakeMove(Grid.Player, p1direction.Value);
            Grid.CheckCollectables(Grid.Player);
        }
        if (Grid.Player2 !=null && !Grid.Player2.IsDead && p2direction.HasValue)
        {
            Grid.MakeMove(Grid.Player2, p2direction.Value);
            Grid.CheckCollectables(Grid.Player2);
        }

        bool p1Caught = !Grid.Player.IsDead && Grid.IsZombieAt(Grid.Player.Location.X, Grid.Player.Location.Y);
        bool p2Caught = Grid.Player2 != null && !Grid.Player2.IsDead && Grid.IsZombieAt(Grid.Player2.Location.X, Grid.Player2.Location.Y);

        if (p1Caught) Grid.Player.Die();
        if (p2Caught) Grid.Player2.Die();

        if (Grid.Player.IsDead && (Grid.Player2 == null || Grid.Player2.IsDead))
        {
            IsGameOver = true;
            return;
        }

        if (tickCounter % zombieSpeed == 0)
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

        bool keyFound = Grid.Player.HasKey || (Grid.Player2 != null && Grid.Player2.HasKey);

        if (keyFound && !exitSpawned)
        {
            Grid.Collectables.Add(new ExitPortal(Grid.X / 2, Grid.Y / 2));
            exitSpawned = true;
        }

        bool p1Escaped = Grid.Player.HasEscaped;
        bool p2Escaped = (Grid.Player2 == null) || Grid.Player2.HasEscaped;

        if (p1Escaped && p2Escaped)
        {
            IsGameWon = true;
            IsGameOver = true;
        }
    }
}