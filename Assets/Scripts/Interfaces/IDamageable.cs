using UnityEngine;

public interface IDamageable
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public TookDamageData TakeDamage(int damage);

    public static void AddHealth(IDamageable damageable, int amount)
    {
        damageable.Health += amount;
        if (damageable.Health > damageable.MaxHealth)
            damageable.Health = damageable.MaxHealth;
    }
}
public class TookDamageData
{
    public int HealthBefore;
    public int HealthAfter;

    public TookDamageData(int healthBefore, int healthAfter)
    {
        HealthBefore = healthBefore;
        HealthAfter = healthAfter;
    }
}
