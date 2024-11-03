using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using static ACG.Colors;

public static class Commands
{

    public static async Task<string> GetCommandOutput(Command command, Command.CommandType[] validTypes = null, params string[] args)
    {
        if(validTypes != null && !validTypes.Contains(command.type)) return "<color=red>Command Not Found!";

        switch (command.type)
        {
            case Command.CommandType.Move: return Move(args);
            case Command.CommandType.Help: return Help(args);
            case Command.CommandType.Clear: return Clear(args);
            case Command.CommandType.Save: return Save(args);
            case Command.CommandType.Load: return Load(args);
            case Command.CommandType.See: return See(args);
            case Command.CommandType.Equip: return Equip(args);
            case Command.CommandType.Run: return Run(args);
            case Command.CommandType.Fight: return await Fight(args);
            case Command.CommandType.Use: return await Use(args);
            case Command.CommandType.Battle: return await Battle(args);
            case Command.CommandType.DELETE_SAVE: return await DeleteSave(args);
            default: return "<color=red>Command Not Found!";
        };
    }

    private static async Task<string> Fight(string[] args)
    {
        if (BattleSystem.CurrentEnemy == null) return "Something went wrong.";

        var attackResult = Player.Weapon.DealDamage(BattleSystem.CurrentEnemy);


        ACG.SpawnBattleUI(ConsoleController.Controller.transform).Display();

        await Awaitable.WaitForSecondsAsync(0.25f);

        return $"You attacked the {BattleSystem.CurrentEnemy.DisplayName} with your {Player.Weapon.DisplayName}!\n" +
                                         (attackResult.WasCrit
                                            ? $"<color=yellow>It's a Critical Hit!</color> You dealt <color=yellow>{attackResult.Damage}</color> Damage!\n"
                                            : $"You dealt <color=red>{attackResult.Damage}</color> Damage!\n"
                                         ) 
                                         + $"{BattleSystem.CurrentEnemy.DisplayName}'s Health: <color=green>{attackResult.OtherHealthBefore}</color> -> <color=red>{attackResult.OtherHealthAfter}</color>";
    
    }

    private static string Run(string[] args)
    {
        //TODO: implemetn
        return "Ran away ong";
    }

    private static async Task<string> Battle(string[] args)
    {
        await BattleSystem.StartBattle(ACG.Enemies.Orc); //TODO: change im testing with an Orc
        return "Battle Over!";
    }

    // TODO: arg1 should be page (eventually)
    public static string Help(params string[] args)
    {
        if(args.Length <= 1)
            return "Here is a list of commands: \n" + Command.GetListOfCommands();

        string arg1 = args[1];

        if (int.TryParse(arg1, out int pageNumber))
            return $"Now showing page {pageNumber}:";
        else if (ConsoleController.GetCommandType(arg1) != Command.CommandType.NOT_FOUND)
            return Command.GetUsage(ConsoleController.GetCommandType(arg1));
        else 
            return "Here is a list of commands: \n" + Command.GetListOfCommands();
    }

    public static string Move(params string[] args) => "Moved!";
    public static string Clear(params string[] args)    
    {
        ACG.DestroyAllChildren(ConsoleController.Controller.transform);
        ACG.SpawnCommandLine(ConsoleController.Controller.transform);
        return string.Empty;
    }
    //arg1 should be the UserName
    public static string Save(params string[] args)
    {
        string username = null;
        if (args.Length > 1)
        {
            username = args[1].ToUpper();
            if(username != null)
            {
                if (username.Length <= 15)
                    Player.UpdateUserName(username);
                else
                    return "Please use a shorter name!";
            }
        }

        if (SaveLoad.Save())
            return $"<color=green>Saved Successfully{(username != null ? (" as <color=yellow>" + username + "<color=green>'s Profile") : string.Empty)}.";
        else
            return "<color=red>An Error Occured.";
    }
    //arg1 is username
    public static string See(params string[] args)
    {
        if (args.Length <= 1)
            return "Incorrect Usage. For help, use \"help see\"";
        else if (args[1].ToLower().Equals("weapon"))
            return Player.WeaponStatsString();
        else if (args[1].ToLower().Equals("player"))
            return Player.PlayerStatsString();
        else if (Regex.IsMatch(args[1].ToLower(), "inve?n?t?o?r?y?"))
            return $"Current Items: \n{Player.Inventory.StringList()}";

        return "Incorrect Usage. For help, use \"help see\"";
    }
    public static string Load(params string[] args)
    {
        if (SaveLoad.Load() == null)
            return "No save data exists. Try saving first!";
        else
            return $"Loaded <color=yellow>{Player.UserName}</color>'s Profile Successfully.";
    }
    public static string Equip(params string[] args)
    {
        if (args.Length <= 1) return "Incorrect Usage. For help, use \"help equip\"";

        if (Inventory.Has<Weapon>(args[1]))
        {
            Player.EquipWeapon(Inventory.Get<Weapon>(args[1]));
            return $"<color=green>Successfully Equipped your <color={WeaponNameColor}>{Player.Weapon.Name}";
        }
        else
        {
            return $"You don't have a(n) \"{args[1]}\" weapon in your inventory!";
        }
    }
    private static async Task<string> Use(string[] args)
    {
        if (args.Length <= 1) return "Invalid Usage. For help, use \"help use\"";

        if (Inventory.Has<Consumable>(args[1]))
        {
            var consumable = Inventory.Get<Consumable>(args[1]);
            await Player.UseConsumable(consumable, Player.Instance);
            return $"Successfully Used {consumable.DisplayName}";
        }
        return $"You don't have a(n) {args[1]} in your inventory!";
    }
    public static async Task<string> DeleteSave(params string[] args)
    {
        bool confirmed = await ACG.DisplayWithConfirmation("Are you sure you want to delete all saved data?\nType CONFIRM to verify.");

        if (confirmed)
        {
            Player.ResetPlayerStats();
            SaveLoad.DeleteData();
            return "<color=green>Successfully Deleted.";
        }
        else
            return "<color=red>Cancelled. Nothing was done.";
    }
    public class CommandExecutionCallback
    {
        public Command Command;
        public string Output;
        public bool SpawnCommandLineOnFinish;

        public CommandExecutionCallback(Command command, string output, bool spawnCommandLineOnFinish)
        {
            this.Command = command;
            this.Output = output;
            SpawnCommandLineOnFinish = spawnCommandLineOnFinish;
        }

        public static CommandExecutionCallback New(Command command, string output, bool spawnCL) => new CommandExecutionCallback(command, output, spawnCL);
    }
}
