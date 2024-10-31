using System;
using System.Linq;
using UnityEngine;

public static class ACG
{
    public static string BasePath => PlayerStats.UserName == null
                                     ? "<color=red>UNKNOWN</color>~SERVER://"
                                     : $"<color=yellow>{PlayerStats.UserName}</color>~SERVER://";

    public static string FullPath => BasePath + "> ";

    public static T LoadResource<T>(params string[] pathSegments) where T : UnityEngine.Object
    {
        string path = System.IO.Path.Combine(pathSegments);
        return Resources.Load<T>(path);
    }

    public static GameObject SpawnCommandLine(Transform parent) => SpawnPrefab(Paths.Prefabs.CommandLine, parent);
    public static GameObject SpawnOutputBox(Transform parent) => SpawnPrefab(Paths.Prefabs.OutputBox, parent);
    public static GameObject SpawnPrefab(string prefab, Transform parent) => GameObject.Instantiate(LoadResource<GameObject>(Paths.PrefabsPath, prefab), parent);

    public static void DestroyAllChildren(Transform obj)
    {
        for(int i = 0; i < obj.childCount; i++)
            UnityEngine.Object.Destroy(obj.GetChild(i).gameObject);
    }


    public static class Names
    {
        public const string SaveCommand = "Save";
        public const string Delete_SaveCommand = "DELETE_SAVE";

        public readonly static string[] ColoredNames = new string[]
        {
            Delete_SaveCommand, SaveCommand
        };

        public static string AddColor(string cmd) => ColoredNames.ToList().Find(x => x.Contains(cmd)) ?? cmd;
    }
    public static class Paths
    {
        public const string PrefabsPath = "Prefabs";
        public static class Prefabs 
        {
            public const string OutputBox = "OutputBox";
            public const string CommandLine = "CommandLine";
        }
    }

}
