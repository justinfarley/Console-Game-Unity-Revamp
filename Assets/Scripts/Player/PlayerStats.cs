using RedLobsterStudios.Util;
using System;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class PlayerStats : MonoSingleton<PlayerStats>, ISaveable
{
    public static Inventory Inventory { get; private set; }
    //public Weapon Weapon { get; private set; }
    public static int Health { get; private set; }
    public static float BaseCritChance { get; private set; } = 0.05f;
    public static string UserName { get; private set; }

    public static void UpdateUserName(string userName) => UserName = userName;

    public Data GetData()
    {
        return PlayerStatsSaveData.New(Inventory, Health, BaseCritChance, UserName);
    }

    public void LoadData(Data data)
    {
        Inventory = ((PlayerStatsSaveData)data).Inventory;
        Health = ((PlayerStatsSaveData)data).Health;
        BaseCritChance = ((PlayerStatsSaveData)data).BaseCritChance;
        UserName = ((PlayerStatsSaveData)data).UserName;
    }

    //public float CritChance => BaseCritChance + Weapon.CritChance;
    //MAYBE: defense


    [Serializable]
    public class PlayerStatsSaveData : Data
    {
        public Inventory Inventory;
        public int Health;
        public float BaseCritChance;
        public string UserName;

        public PlayerStatsSaveData(Inventory inventory, int health, float baseCritChance, string userName)
        {
            Inventory = inventory;
            Health = health;
            BaseCritChance = baseCritChance;
            UserName = userName;
        }
        public static PlayerStatsSaveData New(Inventory inventory, int health, float baseCritChance, string userName) => new PlayerStatsSaveData(inventory, health, baseCritChance, userName);
    }
}
