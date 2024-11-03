using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Enemy : IDamageable
{
    public enum AttackType
    {
        HeldItem,
        Weapon
    }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    //TODO: Maybe defense?
    public Weapon Weapon { get; set; }
    public Consumable HeldItem { get; set; }
    public string Name { get; set; }
    public string DisplayName
    {
        get
        {
            return $"<color={ACG.Colors.EnemyNameColor}>{Name}</color>";
        }
    }
    public string Description { get; set; }
    public int ExperienceValue { get; set; }
    public float Speed { get; set; }

    public Enemy(string name, int health, string description, int experienceValue, float speed, Consumable heldItem, params Weapon[] weapons)
    {
        Health = health;
        MaxHealth = health;
        Weapon = weapons[Random.Range(0, weapons.Length)];
        Name = name;
        Description = description;
        ExperienceValue = experienceValue;
        Speed = speed;
        HeldItem = heldItem;
    }
    public static Enemy Create(string name, int health, string description, int experienceValue, float speed, Consumable heldItem, params Weapon[] weapons)
        => new(name, health, description, experienceValue, speed, heldItem, weapons);

    public TookDamageData TakeDamage(int damage)
    {
        int beforeHealth = Health;

        Health -= damage;

        int afterHealth = Health;

        if (Health <= 0)
            Die();

        return new TookDamageData(beforeHealth, afterHealth);
    }

    public AttackType ChooseAttack()
    {
        if(HeldItem is Potion)
        {
            float healthPercentage = GetHealthPercentage();

            if (healthPercentage <= 35f)
                return AttackType.HeldItem;
            else
                return AttackType.Weapon;
        }


        //Weapon by default
        return AttackType.Weapon;
    }

    private float GetHealthPercentage() => ((float)Health / MaxHealth) * 100f;

    public void Die()
    {
        Health = 0;
        Debug.Log($"The {Name} died.");
    }
}
