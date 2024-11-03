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
    public string DisplayName
    {
        get => $"<color={WeaponNameColor}>{Name}</color>";
    }
    public Weapon(string name, int minDamage, int maxDamage, float critChance, int durability, float critMultiplier, Tuple<int, int> worth, float speedFactor) : base(name, worth)
    {
        DamageRange = new(minDamage, maxDamage);
        CritChance = critChance;
        Durability = durability;
        BaseDurability = durability;
        CritMultiplier = critMultiplier;
        SpeedFactor = speedFactor;
    }
    public AttackData DealDamage(IDamageable damageableObj)
    {
        int dealtDamage = UnityEngine.Random.Range(DamageRange.Item1, DamageRange.Item2 + 1);

        bool wasCrit = IsCrit();

        if (wasCrit)
            dealtDamage = Mathf.FloorToInt(dealtDamage * CritMultiplier);

        TookDamageData otherData = damageableObj.TakeDamage(dealtDamage);
        Durability--;

        return new AttackData(dealtDamage, opposingHealthBeforeAttack: otherData.HealthBefore, otherData.HealthAfter, Durability, wasCrit);
    }
    private static bool IsCrit() => UnityEngine.Random.Range(0, 101) <= Mathf.FloorToInt(Player.CritChance);
    public static Weapon Create(string name, int minDamage, int maxDamage, float critChance, int durability, float critMultiplier, Tuple<int, int> worth, float speedFactor)
        => new Weapon(name, minDamage, maxDamage, critChance, durability, critMultiplier, worth, speedFactor);

    public class AttackData
    {
        public int Damage;
        public int OtherHealthBefore;
        public int OtherHealthAfter;
        public int NewDurability;
        public bool WasCrit;



        public AttackData(int damage, int opposingHealthBeforeAttack, int opposingHealthAfterAttack, int newDurability, bool wasCrit)
        {
            Damage = damage;
            OtherHealthBefore = opposingHealthBeforeAttack;
            OtherHealthAfter = opposingHealthAfterAttack;
            NewDurability = newDurability;
            this.WasCrit = wasCrit;
        }
    }

}
