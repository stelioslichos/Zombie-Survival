namespace zombie_survival
{
    public class Player : GridEntity, IDamageable
    {
        public Player(int x, int y) : base(x, y) { }

        public int Ammo { get; private set; } = 3;
        public int Kills { get; private set; } = 0;
        public bool IsDead { get; private set; } = false;
        public bool HasKey { get; private set; } = false;
        public bool HasEscaped { get; private set; } = false;

        public MoveDirection FacingDirection { get; set; } = MoveDirection.Right;

        public void CollectKey()
        {
            HasKey = true;
        }

        public void Escape()
        {
            HasEscaped = true;
        }

        public void Die()
        {
            IsDead = true;
        }

        public void AddAmmo(int amount)
        {
            Ammo += amount;
        }

        public void AddKill()
        {
            Kills++;
        }

        public bool Shoot()
        {
            if (Ammo > 0)
            {
                Ammo--;
                return true;
            }
            return false;
        }
    }
}