namespace zombie_survival;

public interface IDamageable
{
    bool IsDead {  get; }

    void Die();
}