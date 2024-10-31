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
    }

    public CommandType type;
    public string name;
    public string[] args;

    public Command(string input)
    {
        args = input.Split(" ");
        type = ConsoleController.GetCommandType(args[0]);
        name = type.ToString();
    }
    public static string GetListOfCommands()
    {
        string[] names = Enum.GetNames(typeof(CommandType)).Where(x => !x.Contains("NOT_FOUND")).OrderBy(x => x).Select(x => $"- {ACG.Names.AddColor(x)}").ToArray();
        string result = "<color=#D3D3D3>";
        foreach (var name in names)
            result += (name + "\n");
        return result;
    }
}
