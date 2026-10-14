namespace zombie_survival
{
    public class SkeletonKey : GridEntity, ICollectable
    {
        public SkeletonKey(int x, int y) : base(x, y) { }

        public void Collected(Player player)
        {
            player.CollectKey();
        }
    }
}