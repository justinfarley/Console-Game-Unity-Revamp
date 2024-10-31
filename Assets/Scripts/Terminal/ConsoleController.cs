using UnityEngine;
using RedLobsterStudios.Util;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

public class ConsoleController : MonoBehaviour
{
    public static Action<Commands.CommandExecutionCallback> OnCommandExecuted = null;
    private static GameObject outputBoxPrefab;
    public static LinkedList<string> CommandList = new LinkedList<string>();
    public static LinkedListNode<string> CurrentCommand = null;
    public static ConsoleController Controller { get; set; } = null;
    protected void Awake()
    {
        if (Controller == null)
            Controller = this;

        OnCommandExecuted += RunCommand;
        outputBoxPrefab = outputBoxPrefab != null 
            ? outputBoxPrefab 
            : ACG.LoadResource<GameObject>(ACG.Paths.PrefabsPath, ACG.Paths.Prefabs.OutputBox);
    }
    private void RunCommand(Commands.CommandExecutionCallback cb)
    {
        if(cb.Command.type == Command.CommandType.Clear)
            return;
        //instantiate output box
        SpawnOutputBox(cb.Output, transform);
    }

    public static Command.CommandType GetCommandType(string consoleInput)
    {
        string result = AutoComplete(Command.CommandNames, consoleInput);

        return Enum.TryParse(result, true, out Command.CommandType type) ? type : Command.CommandType.NOT_FOUND;
    }
    public static string AutoComplete(List<string> commandNames, string input)
    {
        input = input.ToLower();
        string result = commandNames.Find(cName => cName.ToLower().StartsWith(input) ||
                                                cName.ToLower().Equals(input) ||
                                                cName.ToLower().Contains(input));
        return result;
    }
    public static void SpawnOutputBox(string output, Transform parent)
    {
        OutputBox outputBox = ACG.SpawnOutputBox(parent).GetComponent<OutputBox>();

        if (string.IsNullOrEmpty(output)) return;

        outputBox.ShowOutput(output);
    }

    public static string GetPrevCommand()
    {
        if (CurrentCommand == null)
        {
            if(CommandList.Count <= 0)
                return string.Empty;
            CurrentCommand = CommandList.Last;
            return CurrentCommand.Value ?? string.Empty;
        }

        if (CurrentCommand.Previous == null)
            return CurrentCommand.Value ?? string.Empty;

        CurrentCommand = CurrentCommand.Previous;

        return CurrentCommand.Value ?? string.Empty;
    }
    public static string GetNextCommand()
    {
        if (CurrentCommand == null)
            return string.Empty;

        if(CurrentCommand == CommandList.Last)
        {
            CurrentCommand = null;
            return string.Empty;
        }

        CurrentCommand = CurrentCommand.Next;

        return CurrentCommand?.Value ?? string.Empty;
    }
}
