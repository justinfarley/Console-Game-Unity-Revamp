using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Enemy : IDamageable
{
    public int Health { get; set; }
    public int BaseHealth { get; set; }
    //TODO: Maybe defense?
    public Weapon Weapon { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ExperienceValue { get; set; }
    public float Speed { get; set; }

    public Enemy(string name, int health, string description, int experienceValue, float speed, params Weapon[] weapons)
    {
        Health = health;
        BaseHealth = health;
        Weapon = weapons[Random.Range(0, weapons.Length)];
        Name = name;
        Description = description;
        ExperienceValue = experienceValue;
        Speed = speed;
    }
    public static Enemy Create(string name, int health, string description, int experienceValue, float speed, params Weapon[] weapons)
        => new(name, health, description, experienceValue, speed, weapons);

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
            Die();
    }
    public void Die()
    {
        Debug.Log($"The {Name} died.");
    }
}
