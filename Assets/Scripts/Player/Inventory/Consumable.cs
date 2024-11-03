using System;
using System.Threading.Tasks;
using UnityEngine;
using static ACG.Colors;

public class Consumable : Item
{
    public string DisplayName => $"<color={ConsumableNameColor}>{Name}</color>";
    public Consumable(string name, Tuple<int, int> worth) : base(name, worth)
    {
    }
    public virtual UseData Use(IDamageable damageable)
    {
        Inventory.RemoveItem(this);

        return new UseData();
    }
    public static Consumable Create(string name, Tuple<int, int> worth) => new Consumable(name, worth);
}
public class UseData
{

}