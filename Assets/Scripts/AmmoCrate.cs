namespace zombie_survival
{
    public class AmmoCrate : GridEntity, ICollectable
    {
        public AmmoCrate(int x, int y) : base(x, y) { }

        public void Collected(Player player)
        {
            player.AddAmmo(3);
        }

        public override string ToString()
        {
            return "A";
        }
    }
}