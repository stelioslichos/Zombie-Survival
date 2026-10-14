namespace zombie_survival;

public class Zombie : GridEntity, IDamageable
{
    public Zombie(int x, int y) : base(x, y) { }

    public bool IsDead { get; private set; } = false;

    public void Die()
    {
        IsDead = true;
    }
    public override string ToString()
    {
        return "Z";
    }
}