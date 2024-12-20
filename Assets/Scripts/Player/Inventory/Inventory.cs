using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    public static List<Item> Items = null;

    public Inventory(List<Item> items)
    {
        Items = items;
    }

    public string StringList()
    {
        string result = string.Empty;
        int itemsPerLine = 5;
        for (int i = 0; i < Items.Count; i++)
            result += Items[i].Name + ( i % itemsPerLine == 0 && i <= (Items.Count - 2) ? ",\n" : ", ");
        result = result.Trim();
        return result;
    }
    public static bool Has<T>(string itemName) => Items.Where(x => x is T).Any(x => ConsoleController.Matches(x.Name.ToLower(), itemName.ToLower()));
    public static bool Has(string itemName) => Items.Any(x => ConsoleController.Matches(x.Name.ToLower(), itemName.ToLower()));
    public static T Get<T>(string itemName) where T : class => Items.Where(x => x is T).ToList().Find(x => ConsoleController.Matches(x.Name.ToLower(), itemName.ToLower())) as T;
    public static void AddItem(Item item) => Items = Items.Concat(new List<Item>() { item }).ToList();
    public static void RemoveItem(Item item) => Items.Remove(item);
}
