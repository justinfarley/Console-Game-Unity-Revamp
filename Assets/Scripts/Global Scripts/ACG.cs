using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public static class ACG
{

    public enum OutputType
    {
        Default,
        Confirmation,
        Prompt
    }

    public static string BasePath => Player.UserName == null
                                     ? "<color=red>UNKNOWN</color>~SERVER://"
                                     : $"<color=yellow>{Player.UserName}</color>~SERVER://";

    public static string FullPath => BasePath + ExtraPath + "> ";

    public static string ExtraPath { get; set; }

    public static readonly string[] BattlePrompts = new string[]
    {
        "What would you like to do?", "What will you do?", "Do something that will help you win:",
    };

    public static string AddToPath(string path)
    {
        ExtraPath += path;
        return FullPath;
    }
    public static string ResetPath()
    {
        ExtraPath = string.Empty;
        return FullPath;
    }
    public static string RemoveFromPath(string pathToRemove)
    {
        ExtraPath = ExtraPath.Replace(pathToRemove, string.Empty);
        return FullPath;
    }
    public static async Task Display(string fullDisplayString, bool spawnCommandLineOnCompletion = false)
    {
        HardCleanup();
        await Awaitable.NextFrameAsync();
        await SpawnOutputBox(ConsoleController.Controller.transform).GetComponent<OutputBox>().ShowOutput(fullDisplayString, OutputType.Default, spawnCommandLineOnCompletion);
    }
    public static async Task<bool> DisplayWithConfirmation(string fullDisplayString)
    {
        HardCleanup();
        await Awaitable.NextFrameAsync();
        _ = SpawnOutputBox(ConsoleController.Controller.transform)
            .GetComponent<OutputBox>()
            .ShowOutput(fullDisplayString, OutputType.Confirmation);

        bool finished = false;
        bool confirmed = false;

        void Confirmation(bool val)
        {
            finished = true;
            confirmed = val;
        }

        CommandLine.OnConfirmationAnswered += Confirmation;

        while (!finished)
            await Awaitable.NextFrameAsync();

        CommandLine.OnConfirmationAnswered -= Confirmation;

        return confirmed;
    }
    public static async Task<string> DisplayWithPrompt(string fullDisplayString, bool spawnCLOnComplete = true)
    {
        HardCleanup();
        await Awaitable.NextFrameAsync();

        bool finished = false;
        string result = "REPLACE ME";

        void Action(string res)
        { 
            finished = true;
            result = res;
        }

        CommandLine.OnPromptAnswered += Action;

        await SpawnOutputBox(ConsoleController.Controller.transform).GetComponent<OutputBox>().ShowOutput(fullDisplayString, OutputType.Prompt, spawnCLOnComplete);
        
        while(!finished)
            await Awaitable.NextFrameAsync();

        CommandLine.OnPromptAnswered -= Action;

        return result;
    }

    public static void HardCleanup()
    {
        List<CommandLine> commandLines = ConsoleController.Controller.GetComponentsInChildren<CommandLine>().ToList();

        foreach (var result in commandLines)
        {
            if(result.Caret != null)
                GameObject.Destroy(result.Caret.gameObject);

            if (result.Field == null)
                result.Field = result.GetComponent<TMP_InputField>();

            result.Field.enabled = false;
            GameObject.Destroy(result);
        }
    }

    public static bool CoinFlip(int numChoices = 2) => UnityEngine.Random.Range(0, numChoices) == 0;
    public static int NumBetween(Tuple<int, int> tuple) => UnityEngine.Random.Range(tuple.Item1, tuple.Item2 + 1);
    public static T PickRandom<T>(T[] items) => items[UnityEngine.Random.Range(0, items.Length)]; 
    public static T LoadResource<T>(params string[] pathSegments) where T : UnityEngine.Object
    {
        string path = System.IO.Path.Combine(pathSegments);
        return Resources.Load<T>(path);
    }

    public static bool PlayerGoesFirst(Enemy enemy) => Player.Speed >= enemy.Speed;
    public static GameObject SpawnCommandLine(Transform parent) => SpawnPrefab(Paths.Prefabs.CommandLine, parent);
    public static GameObject SpawnOutputBox(Transform parent) => SpawnPrefab(Paths.Prefabs.OutputBox, parent);
    public static BattleUIDisplay SpawnBattleUI(Transform parent)
    {
        DestroyAllChildren<BattleUIDisplay>(ConsoleController.Controller.transform);
        return SpawnPrefab(Paths.Prefabs.BattleUI, parent).GetComponent<BattleUIDisplay>();
    }
    public static GameObject SpawnPrefab(string prefab, Transform parent) => GameObject.Instantiate(LoadResource<GameObject>(Paths.PrefabsPath, prefab), parent);

    public static void DestroyAllChildren(Transform obj)
    {
        for(int i = 0; i < obj.childCount; i++)
            UnityEngine.Object.Destroy(obj.GetChild(i).gameObject);
    }
    public static void DestroyAllChildren<T>(Transform obj) where T : Component
    {
        for (int i = 0; i < obj.childCount; i++)
            if(obj.GetChild(i).GetComponentInChildren<T>() != null)
                UnityEngine.Object.Destroy(obj.GetChild(i).gameObject);
    }

    public static class Names
    {
        public const string SaveCommand = "Save";
        public const string Delete_SaveCommand = "<color=red>DELETE_SAVE</color>";

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
            public const string BattleUI = "BattleUI";
        }
    }
    public static class FakePaths
    {
        public const string ConfirmationPath = ".../";
        public const string PromptPath = ".../";
    }

    public static class Weapons 
    {
        //Level 0
        public static readonly Weapon Stick = Weapon.Create("Stick", 1, 1, 5f, 10, 2f, new(1,1), 3);
        public static readonly Weapon Branch = Weapon.Create("Branch", 1, 2, 7.5f, 15, 2f, new(1,2), 2.5f);
        public static readonly Weapon WoodenSword = Weapon.Create("Wooden Sword", 1, 3, 10f, 25, 2.5f, new(3,3), 2f);
        public static readonly Weapon[] Level_0_Weapons = new Weapon[] { Stick, Branch, WoodenSword };

        //Level 1
        public static readonly Weapon BowAndArrow = Weapon.Create("Bow and Arrow", 0, 6, 50f, 30, 2f, new(2,4), 1.5f);
        public static readonly Weapon BrassKnuckles = Weapon.Create("Brass Knuckles", 3, 5, 25f, 30, 2f, new(4, 5), 5f);
        public static readonly Weapon FrozenSausage = Weapon.Create("Frozen Sausage", 4, 5, 55f, 10, 1.25f, new(0, 1), 3f);
        public static readonly Weapon IronHammer = Weapon.Create("Iron Hammer", 4, 7, 5f, 50, 3f, new(4, 5), -0.5f);
        public static readonly Weapon[] Level_1_Weapons = new Weapon[] { BowAndArrow, BrassKnuckles, FrozenSausage, IronHammer };

        //Level 2
        public static readonly Weapon Brick = Weapon.Create("Brick", 5, 8, 5f, 100, 1.5f, new(1, 1), -2f);
        public static readonly Weapon Longbow = Weapon.Create("Longbow", 0, 10, 50f, 50, 2f, new(3, 8), 1f);
        public static readonly Weapon PlatedAxe = Weapon.Create("Plated Axe", 5, 8, 10f, 100, 2.5f, new(5, 10), -1f);
        public static readonly Weapon RustedSpear = Weapon.Create("Rusted Spear", 5, 6, 40f, 10, 3f, new(2, 8), -0.5f);
        public static readonly Weapon[] Level_2_Weapons = new Weapon[] { Brick, Longbow, PlatedAxe, RustedSpear };

        //Level 3
        public static readonly Weapon Crossbow = Weapon.Create("Crossbow", 2, 15, 50f, 75, 3f, new(5, 10), -0.5f);
        public static readonly Weapon Gambler = Weapon.Create("Gambler", 5, 20, 75f, 75, 2f, new(10, 10), -100f);
        public static readonly Weapon WoodenBat = Weapon.Create("Wooden Bat", 6, 12, 20f, 50, 3f, new(8, 9), 1.5f);
        public static readonly Weapon[] Level_3_Weapons = new Weapon[] { Crossbow, Gambler, WoodenBat };

        //Level 4
        public static readonly Weapon Club = Weapon.Create("Club", 10, 15, 20f, 60, 2.5f, new(7, 10), -1f);
        public static readonly Weapon SharpKey = Weapon.Create("Sharp Key", 7, 9, 99f, 55, 2.25f, new(10, 15), 2f);
        public static readonly Weapon ToyGun = Weapon.Create("Toy Gun", 8, 20, 15f, 50, 1.25f, new(6, 10), 1f);
        public static readonly Weapon[] Level_4_Weapons = new Weapon[] { Club, SharpKey, ToyGun };

        //Level 5
        public static readonly Weapon MetalBat = Weapon.Create("Metal Bat", 15, 25, 40f, 200, 2f, new(10, 20), 2f);
        public static readonly Weapon SharkGun = Weapon.Create("Shark Gun", 15, 25, 20f, 200, 3f, new(30, 30), 0f);
        public static readonly Weapon[] Level_5_Weapons = new Weapon[] { MetalBat, SharkGun };


        public static readonly Weapon[] ALL_WEAPONS = Level_0_Weapons
                                                     .Concat(Level_1_Weapons)
                                                     .Concat(Level_2_Weapons)
                                                     .Concat(Level_3_Weapons)
                                                     .Concat(Level_4_Weapons)                                    
                                                     .Concat(Level_5_Weapons)                                    
                                                     .ToArray();
    }
    public static class Items
    {
        //Potions
        public static readonly Potion SmallPotion = Potion.Create("Small Potion", new(1, 3), PotionType.Small);
        public static readonly Potion MediumPotion = Potion.Create("Medium Potion", new(3, 7), PotionType.Medium);
        public static readonly Potion LargePotion = Potion.Create("Large Potion", new(8, 14), PotionType.Large);
        public static readonly Potion ExtraLargePotion = Potion.Create("Extra Large Potion", new(12, 18), PotionType.ExtraLarge);
        public static readonly Potion MegaPotion = Potion.Create("Mega Potion", new(15, 25), PotionType.Mega);
        public static readonly Potion UltraPotion = Potion.Create("Ultra Potion", new(30, 45), PotionType.Ultra);
        public static readonly Potion GargantuanPotion = Potion.Create("Gargantuan Potion", new(50, 100), PotionType.Gargantuan);

    }
    public static class Enemies
    {
        public static readonly Enemy Goblin = Enemy.Create("Goblin", 5, "A small, but aggressive, Goblin.", 2, 1,null,Weapons.Stick, Weapons.Branch);
        public static readonly Enemy Orc = Enemy.Create("Orc", 10, "A wild Orc furiously swinging his sword around.", 5, 3, null,Weapons.WoodenSword);
        public static readonly Enemy ViciousPlant = Enemy.Create("ViciousPlant", 25, "This overgrown green fungus has big fists that it uses to catch prey.", 12, 2, Items.SmallPotion, Weapons.BrassKnuckles);
        public static readonly Enemy Clown = Enemy.Create("Clown", 50, "It's a Clown. It appears to have anger issues.", 25, 3, Items.SmallPotion, Weapons.WoodenBat);
        public static readonly Enemy Giant = Enemy.Create("Giant", 100, "This enormous human easily intimidates you with its strength.", 50, 1, Items.MediumPotion,Weapons.Club);
        public static readonly Enemy Vampire = Enemy.Create("Vampire", 150, "This enormous human easily intimidates you with its strength.", 50, 1, Items.LargePotion,Weapons.MetalBat);
    }
    public static class Colors 
    {
        public const string WeaponNameColor = "#1ec197";
        public const string EnemyNameColor = "red";
        public const string ConsumableNameColor = "#1e81c1";
    }
    public static class ValidCommands
    {
        private static readonly Command.CommandType[] AlwaysIncluded = new Command.CommandType[] { Command.CommandType.Help, Command.CommandType.Clear };
        public static readonly Command.CommandType[] DefaultCommands = AlwaysIncluded.Concat(new Command.CommandType[] { Command.CommandType.Save, Command.CommandType.Load, Command.CommandType.DELETE_SAVE, Command.CommandType.See, Command.CommandType.Equip, Command.CommandType.Battle, Command.CommandType.Use }).ToArray();
        public static readonly Command.CommandType[] BattleCommands = AlwaysIncluded.Concat(new Command.CommandType[] { Command.CommandType.Fight, Command.CommandType.Run, Command.CommandType.Use }).ToArray();

        private static Dictionary<ConsoleController.ConsoleState, Command.CommandType[]> ValidCommandsDict = new Dictionary<ConsoleController.ConsoleState, Command.CommandType[]>()
        {
            { ConsoleController.ConsoleState.Default, DefaultCommands },
            { ConsoleController.ConsoleState.Battle, BattleCommands},
        };
        public static Command.CommandType[] GetValidCommands(ConsoleController.ConsoleState state) => ValidCommandsDict[state] ?? AlwaysIncluded;
    }
}
