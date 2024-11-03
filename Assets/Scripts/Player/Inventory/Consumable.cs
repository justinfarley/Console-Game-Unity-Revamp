using System;
using System.Threading.Tasks;
using UnityEngine;
using static ACG.Colors;

public class Consumable : Item
{
    public Consumable(string name, Tuple<int, int> worth) : base(name, worth)
    {
        Name = $"<color={ConsumableNameColor}>{Name}</color>";
    }
    public virtual string Use()
    {
        Inventory.RemoveItem(this);
        return $"Used {Name}!";
    }
    public static Consumable Create(string name, Tuple<int, int> worth) => new Consumable(name, worth);
}
