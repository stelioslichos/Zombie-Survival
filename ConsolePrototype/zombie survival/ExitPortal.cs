namespace zombie_survival;

public class ExitPortal : GridEntity, ICollectable
{
    public ExitPortal(int x, int y) : base(x, y) { }

    public void Collected(Player player)
    {
        if (player.HasKey)
        {
            player.Escape();
        }
    }

    public override string ToString()
    {
        return "E";
    }
}