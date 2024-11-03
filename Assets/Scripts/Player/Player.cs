using RedLobsterStudios.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ACG.Colors;

[RequireComponent(typeof(UniqueID))]
public class Player : MonoSingleton<Player>, ISaveable, IDamageable
{
    public static Inventory Inventory { get; private set; }
    public static Weapon Weapon { get; private set; } = ACG.Weapons.Stick;
    public int Health { get; set; } = 10;
    public int MaxHealth { get; set; } = 10;
    public static float BaseCritChance { get; private set; } = 5f;
    public static float CritChance => Mathf.Clamp(BaseCritChance + Weapon.CritChance, 0f, 100f);
    public static string UserName { get; private set; }
    public const float BaseSpeed = 1f;
    public static float Speed => BaseSpeed + Weapon.SpeedFactor;
    //MAYBE: defense

    protected override void Awake()
    {
        base.Awake();
        Inventory = new Inventory(new List<Item>() { ACG.Weapons.Branch, ACG.Items.MediumPotion }); //TODO: REMOVE, JUST FOR NOW);
    }

    public static void UpdateUserName(string userName) => UserName = userName;

    public Data GetData()
    {
        return PlayerStatsSaveData.New(Inventory, Weapon, Health, BaseCritChance, UserName);
    }

    public void LoadData(Data data)
    {
        Inventory = ((PlayerStatsSaveData)data).Inventory;
        Health = ((PlayerStatsSaveData)data).Health;
        MaxHealth = ((PlayerStatsSaveData)data).MaxHealth;
        BaseCritChance = ((PlayerStatsSaveData)data).BaseCritChance;
        UserName = ((PlayerStatsSaveData)data).UserName;
    }

    public static string PlayerStatsString()
    {
        if (UserName == null) return "No data!";

        string result = string.Empty;

        result += $"<color=yellow>{UserName}</color>'s Stats:\n";
        result += $"Health: {Instance.Health}\n";
        if (Weapon == null) 
            return result;
        result += $"Current Weapon: {Weapon.DisplayName}\n";
        result += $"Crit Chance: {BaseCritChance}% (base) + {Weapon.CritChance}% ({Weapon.DisplayName}) = <color={CritChanceColor()}>{CritChance}%\n";
        return result;
    }
    public static string WeaponStatsString()
    {
        if (Weapon == null) return "You don't have a weapon equipped!";

        string result = string.Empty;
        result += $"{Weapon.DisplayName}'s Stats:\n";
        result += $"Damage: {Weapon.DamageRange.Item1} - {Weapon.DamageRange.Item2}\n";
        result += $"Worth: {Weapon.Worth.Item1} - {Weapon.Worth.Item2}\n";
        result += $"Crit Chance: {Weapon.CritChance}%({Weapon.DisplayName})\n";
        result += $"Crit Multiplier: <color={(Weapon.CritMultiplier < 2f ? "yellow" : "green")}>{Weapon.CritMultiplier}x\n";
        result += $"Durability: {Weapon.Durability}/{Weapon.BaseDurability}";
        return result;
    }

    public static void ResetPlayerStats()
    {
        Inventory = new Inventory(new List<Item>() { ACG.Weapons.Branch, ACG.Items.MediumPotion }); //TODO: REMOVE, JUST FOR NOW););
        Instance.Health = 10; //TODO: generic base value change later
        Instance.MaxHealth = 10; //TODO: generic base value change later
        BaseCritChance = 5;
        UserName = null;
    }
    public static void EquipWeapon(Weapon weapon)
    {
        Weapon = weapon;
    }
    public static async Task UseConsumable(Consumable consumable, IDamageable damageable)
    {
        await Awaitable.NextFrameAsync();
        consumable.Use(damageable);
    }

    public TookDamageData TakeDamage(int damage)
    {
        int beforeHealth = Health;
        
        Health -= damage;
        
        int afterHealth = Health;

        if (Health <= 0)
            print("ded");

        return new TookDamageData(beforeHealth, afterHealth);
    }
    private static string CritChanceColor() =>
        CritChance switch
        {
            >= 50f => "green",
            >= 35f => "orange",
            >= 10f => "yellow",
            < 10f => "red",
            _ => "Something went horribly wrong.",
        };

    [Serializable]
    public class PlayerStatsSaveData : Data
    {
        public Inventory Inventory;
        public Weapon Weapon;
        public int Health;
        public int MaxHealth;
        public float BaseCritChance;
        public string UserName;

        public PlayerStatsSaveData(Inventory inventory, Weapon weapon, int health, float baseCritChance, string userName)
        {
            Inventory = inventory;
            Weapon = weapon;
            Health = health;
            MaxHealth = health;
            BaseCritChance = baseCritChance;
            UserName = userName;
        }
        public static PlayerStatsSaveData New(Inventory inventory, Weapon weapon, int health, float baseCritChance, string userName) => new PlayerStatsSaveData(inventory, weapon, health, baseCritChance, userName);
    }
}
