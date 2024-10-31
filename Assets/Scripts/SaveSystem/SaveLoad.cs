using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public static class SaveLoad
{
    public static UnityAction OnSaveGame;
    public static UnityAction<SaveData> OnLoadGame;

    private static string directory = "/SaveData/";
    private static string fileName = "ACG.ERR";

    public static bool Save()
    {
        OnSaveGame?.Invoke();

        SaveData data = new SaveData();

        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
        });

        File.WriteAllText(dir + fileName, json);

        Debug.Log("Saving Game");

        return true;
    }

    public static SaveData Load()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;

        if(File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
            });

            List<UniqueID> uniqueIDs = Object.FindObjectsByType<UniqueID>(FindObjectsSortMode.None).ToList();

            foreach(var kvp in data.idToSaveableItems)
                uniqueIDs.Find(x => x.ID.Equals(kvp.Key)).GetComponent<ISaveable>().LoadData(kvp.Value);

            OnLoadGame?.Invoke(data);
            return data;
        }
        else
        {
            Debug.Log("File doesn't exist");
        }
        return null;
    }
    //Deletes the file
    public static void DeleteData()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

}
