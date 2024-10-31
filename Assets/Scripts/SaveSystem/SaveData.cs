using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SaveData
{
    public Dictionary<string, Data> idToSaveableItems { get; set; } = new Dictionary<string, Data>();

    public SaveData()
    {
        UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None)
        .ToList()
        .Where(x => ((GameObject)x).GetComponent<ISaveable>() != null)
        .ToList()
        .ForEach(saveable =>
        {

            string id = ((GameObject)saveable).GetComponent<UniqueID>().ID;
            idToSaveableItems.Add(id, ((GameObject)saveable).GetComponent<ISaveable>().GetData());
        });
    }
}
