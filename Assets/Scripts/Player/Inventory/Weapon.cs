using System;
using UnityEngine;
using static ACG.Colors;

public class Weapon : Item
{
    public Tuple<int, int> DamageRange;
    public float CritChance;
    public float CritMultiplier;
    public int Durability;
    public int BaseDurability;
    public float SpeedFactor;
    public Weapon(string name, int minDamage, int maxDamage, float critChance, int durability, float critMultiplier, Tuple<int, int> worth, float speedFactor) : base(name, worth)
    {
        DamageRange = new(minDamage, maxDamage);
        CritChance = critChance;
        Durability = durability;
        BaseDurability = durability;
        CritMultiplier = critMultiplier;
        SpeedFactor = speedFactor;
        Name = $"<color={WeaponNameColor}>{Name}</color>";
    }
    public void DealDamage(IDamageable damageableObj)
    {
        int dealtDamage = UnityEngine.Random.Range(DamageRange.Item1, DamageRange.Item2 + 1);

        if (IsCrit())
            dealtDamage = Mathf.FloorToInt(dealtDamage * CritMultiplier);

        damageableObj.TakeDamage(dealtDamage);
        Durability--;
    }
    private static bool IsCrit() => UnityEngine.Random.Range(0, 101) <= Mathf.FloorToInt(PlayerStats.CritChance);
    public static Weapon Create(string name, int minDamage, int maxDamage, float critChance, int durability, float critMultiplier, Tuple<int, int> worth, float speedFactor)
        => new Weapon(name, minDamage, maxDamage, critChance, durability, critMultiplier, worth, speedFactor);
}
