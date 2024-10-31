using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Item> Items = new List<Item>() { new Item("TEST"), new Item("TEST2") };

    public string StringList()
    {
        string result = string.Empty;
        int itemsPerLine = 5;

        for (int i = 0; i < Items.Count; i++)
        {
            result += Items[i].Name + ( i % itemsPerLine == 0 && i <= (Items.Count - 2) ? ",\n" : ", ");
        }
        result = result.Trim();
        return result;
    }
}
