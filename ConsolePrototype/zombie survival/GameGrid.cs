using System;
using System.Data;
using System.Collections.Generic;

namespace zombie_survival;

public enum MoveDirection
{
    Left, Right, Up, Down
}

public class GameGrid
{
    internal int X {  get; set; }
    internal int Y { get; set; }

    private Random rng = new Random();
    public Player Player { get; }
    public Player Player2 { get; }
    public List<Zombie> Zombies { get; }
    public List<ICollectable> Collectables { get; }

    public GameGrid(int x, int y, bool twoPlayer)
    {
        this.X = x;
        this.Y = y;

        Player = new Player(this.X - 1, 0);
        Zombies = new List<Zombie>();
        Zombies.Add(new Zombie(0, this.Y - 1));
        Collectables = new List<ICollectable>();
        
        if (twoPlayer == true)
        {
            Player2 = new Player(this.X - 1, this.Y - 1);
        }
    }

    // constructor for square game grid.
    public GameGrid(int x, bool twoPlayer) : this(x, x, twoPlayer) { }

    public virtual GridPoint GetLeft(GridPoint p)
    {
        if (p.Y == 0) return p;
        return new GridPoint(p.X, p.Y - 1);
    }

    public virtual GridPoint GetRight(GridPoint p)
    {
        if (p.Y == Y - 1) return p;
        return new GridPoint(p.X, p.Y + 1);
    }

    public virtual GridPoint GetUp(GridPoint p)
    {
        if (p.X == 0) return p;
        return new GridPoint(p.X - 1, p.Y);
    }

    public virtual GridPoint GetDown(GridPoint p)
    {
        if (p.X == X - 1) return p;
        return new GridPoint(p.X + 1, p.Y);
    }

    public List<GridPoint> GetBlastArea(GridPoint origin, int radius)
    {
        List<GridPoint> blastArea = new List<GridPoint>();

        GridPoint currentUp = origin;
        GridPoint currentDown = origin;
        GridPoint currentLeft = origin;
        GridPoint currentRight = origin;

        for (int i = 0; i < radius; i++)
        {
            currentUp = GetUp(currentUp);
            if (!blastArea.Exists(p => p.X == currentUp.X && p.Y == currentUp.Y)) blastArea.Add(currentUp);

            currentDown = GetDown(currentDown);
            if (!blastArea.Exists(p => p.X == currentDown.X && p.Y == currentDown.Y)) blastArea.Add(currentDown);

            currentLeft = GetLeft(currentLeft);
            if (!blastArea.Exists(p => p.X == currentLeft.X && p.Y == currentLeft.Y)) blastArea.Add(currentLeft);

            currentRight = GetRight(currentRight);
            if (!blastArea.Exists(p => p.X == currentRight.X && p.Y == currentRight.Y)) blastArea.Add(currentRight);
        }

        return blastArea;
    }

    public bool IsZombieAt(int x, int y)
    {
        foreach (Zombie z in Zombies)
        {
            if (z.IsAt(x, y))
            {
                return true;
            }
        }
        return false;
    }

    public void MakeMove(GridEntity entity, MoveDirection direction)
    {
        GridPoint currentLocation = entity.Location;
        GridPoint newLocation = currentLocation;

        switch (direction)
        {
            case MoveDirection.Left:
                newLocation = GetLeft(currentLocation);
                break;
            case MoveDirection.Right:
                newLocation = GetRight(currentLocation);
                break;
            case MoveDirection.Up:
                newLocation = GetUp(currentLocation);
                break;
            case MoveDirection.Down:
                newLocation = GetDown(currentLocation);
                break;
        }
        entity.SetLocation(newLocation);
    }

    public void MoveZombie()
    {
        foreach (Zombie z in Zombies)
        {
            int zX = z.Location.X;
            int zY = z.Location.Y;

            Player target = null;
            int shortestDistance = int.MaxValue;

            if (!Player.IsDead)
            {
                target = Player;
                shortestDistance = Math.Abs(zX - Player.Location.X) + Math.Abs(zY - Player.Location.Y);
            }

            if (Player2 != null && !Player2.IsDead)
            {
                int p2Distance = Math.Abs(zX - Player2.Location.X) + Math.Abs(zY - Player2.Location.Y);
                if (target == null || p2Distance < shortestDistance)
                {
                    target = Player2;
                }
            }

            if (target == null) return;

            int pX = target.Location.X;
            int pY = target.Location.Y;


            MoveDirection? direction = null;

            bool moveToX = (zX != pX);
            bool moveToY = (zY != pY);

            if (moveToX && moveToY)
            {
                if (rng.Next(0, 2) == 0)
                {
                    moveToY = false;
                }
                else
                {
                    moveToX = false;
                }
            }
            if (moveToX)
            {
                if (zX < pX)
                {
                    direction = MoveDirection.Down;
                }
                else
                {
                    direction = MoveDirection.Up;
                }
            }
            else if (moveToY)
            {
                if (zY < pY)
                {
                    direction = MoveDirection.Right;
                }
                else
                {
                    direction = MoveDirection.Left;
                }
            }
            if (direction.HasValue)
            {
                MakeMove(z, direction.Value);
            }
        }
    }

    public int FireShotgun(Player player)
    {
        int points = 0;
        var blastArea = GetBlastArea(player.Location, 4);

        for (int i = Zombies.Count - 1; i >= 0; i--)
        {
            Zombie z = Zombies[i];
            if (blastArea.Exists(p => p.X == z.Location.X && p.Y == z.Location.Y))
            {
                Zombies.RemoveAt(i);
                player.AddKill();
                points += 50;
            }
        }
        return points;
    }

    public void CheckCollectables(Player player)
    {
        if (player == null || player.IsDead)
        {
            return;
        }

        for (int i = Collectables.Count -1; i >=0; i--)
        {
            if (Collectables[i] is GridEntity entity && entity.IsAt(player.Location.X, player.Location.Y))
            {
                Collectables[i].Collected(player);
                Collectables.RemoveAt(i);
                break;
            }
        }
    }

    public override string ToString()
    {
        string gridString = "";
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                if (Player.IsAt(i, j))
                {
                    gridString += $"{Player} ";
                }
                else if (Player2 != null && Player2.IsAt(i, j))
                {
                    gridString += $"2 ";
                }
                else if (IsZombieAt(i, j))
                {
                    gridString += $"Z ";
                }
                else
                {
                    ICollectable foundItem = null;
                    foreach (var item in Collectables)
                    {
                        if (item is GridEntity entity && entity.IsAt(i, j))
                        {
                            if (!(item is SkeletonKey))
                            {
                                foundItem = item;
                                break;
                            }
                        }
                    }
                    if (foundItem != null)
                    {
                        gridString += $"{foundItem} ";
                    }
                    else
                    {
                        gridString += ". ";
                    }
                }
            }
            gridString += "\n";
        }
        return gridString;
    }
}