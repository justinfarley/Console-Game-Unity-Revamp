using System;
using UnityEngine;

public class Item
{
    public string Name;
    public Tuple<int, int> Worth;

    public Item(string name, Tuple<int, int> worth)
    {
        Name = name;
        Worth = worth;
    }
}
