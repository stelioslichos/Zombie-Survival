namespace zombie_survival
{
    public class ExitPortal : GridEntity, ICollectable
    {
        public ExitPortal(int x, int y) : base(x, y) { }

        public void Collected(Player player)
        {
                player.Escape();
        }
    }
}