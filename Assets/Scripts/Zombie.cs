namespace zombie_survival
{
    public class Zombie : GridEntity, IDamageable
    {
        public Zombie(int x, int y) : base(x, y) { }

        public bool IsDead { get; private set; } = false;
        public MoveDirection FacingDirection { get; set; } = MoveDirection.Left;

        public void Die()
        {
            IsDead = true;
        }
    }
}