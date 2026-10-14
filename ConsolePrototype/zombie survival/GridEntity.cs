namespace zombie_survival;

public abstract class GridEntity
{
    public GridPoint Location { get; private set; }

    protected GridEntity(int x, int y)
    {
        Location = new GridPoint(x, y);
    }

    public bool IsAt(int x, int y)
    {
        return Location.X == x && Location.Y == y;
    }

    public void SetLocation(GridPoint location)
    {
        this.Location = location;
    }
}