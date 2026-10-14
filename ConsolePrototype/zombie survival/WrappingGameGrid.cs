namespace zombie_survival;

public class WrappingGameGrid : GameGrid
{
    public WrappingGameGrid(int x, int y, bool twoPlayer) : base(x, y, twoPlayer) { }

    public override GridPoint GetLeft(GridPoint p)
    {
        return new GridPoint(p.X, (p.Y + Y - 1) % Y);
    }
    public override GridPoint GetRight(GridPoint p)
    {
        return new GridPoint(p.X, (p.Y + 1) % Y);
    }
    public override GridPoint GetUp(GridPoint p)
    {
        return new GridPoint((p.X + X - 1) % X, p.Y);
    }
    public override GridPoint GetDown(GridPoint p)
    {
        return new GridPoint((p.X + 1) % X, p.Y);
    }
}
