using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    public static List<string> CommandNames => Enum.GetNames(typeof(CommandType)).Select(x => x.ToLower()).ToList();
    public enum CommandType
    {
        NOT_FOUND,
        Move,
        Help,
        Clear,
        Save,
        Load,
        DELETE_SAVE,
        See,
        Equip,
    }

    public CommandType type;
    public string name;
    public string[] args;

    public Command(string input)
    {
        args = input.Trim().Split(" ");
        type = ConsoleController.GetCommandType(args[0]);
        name = type.ToString();
    }

    public static string GetUsage(CommandType type) =>
        type switch
        {
            CommandType.Help =>
                "Usage: \n\"help <page_number>\": Shows specified help page\n\"help <command>\": Shows command usage\n\"help\": Shows a few starter commands.",
            CommandType.See =>
                "Usage: \n\"see player\": Shows current player stats. (Must be on a saved file)\n\"see weapon\": Shows current weapon stats.\n\"see inv\": Shows inventory",
            CommandType.DELETE_SAVE =>
                "Usage: \n\"DELETE_SAVE\" to delete current save progress.",
            CommandType.Save =>
                "Usage: \n\"save\": Saves the current profile\n\"save <username>\": Saves profile with a specified username.",
            CommandType.Move =>
                "Usage: \n\"move <direction> <number_of_squares>\": Moves a number of squares in that direction.",
            CommandType.Clear =>
                "Usage: \n\"clear\": Clears the console.",
            CommandType.Load =>
                "Usage: \n\"load\": Loads the saved profile if one exists.",
            CommandType.Equip =>
                "Usage: \n\"equip <weapon_name>\": Equips the given weapon from your inventory.",
            _ => $"Something went wrong. The Case was probably not added for command type {type}",
        };

    public static string GetListOfCommands()
    {
        var validCommands = ACG.ValidCommands.GetValidCommands(ConsoleController.State);
        string[] names = Enum.GetNames(typeof(CommandType))
                             .Where(x => !x.Contains("NOT_FOUND"))
                             .Where(x => validCommands.Select(x => x.ToString().ToLower()).ToList().Contains(x.ToLower()))
                             .OrderBy(x => x).Select(x => $"- {ACG.Names.AddColor(x)}")
                             .ToArray();
        string result = "<color=#808080>";
        foreach (var name in names)
            result += (name + "\n<color=#808080>");

        result += "You can also use \"help <command>\" for additional usage info. Example: \"help see\"";
        return result;
    }
}
