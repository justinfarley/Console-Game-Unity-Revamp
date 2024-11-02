using UnityEngine;
using RedLobsterStudios.Util;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Febucci.UI.Core;

public class ConsoleController : MonoBehaviour
{
    public enum ConsoleState
    {
        Default,
        Battle,
        ItemPedestal,

    }
    public static ConsoleState State = ConsoleState.Default;
    public static Action<Commands.CommandExecutionCallback> OnCommandExecuted = null;
    private static GameObject outputBoxPrefab;
    public static LinkedList<string> CommandList = new LinkedList<string>();
    public static LinkedListNode<string> CurrentCommand = null;
    public static ConsoleController Controller { get; set; } = null;
    protected async void Awake()
    {
        if (Controller == null)
            Controller = this;

        OnCommandExecuted += RunCommand;
        outputBoxPrefab = outputBoxPrefab != null 
            ? outputBoxPrefab 
            : ACG.LoadResource<GameObject>(ACG.Paths.PrefabsPath, ACG.Paths.Prefabs.OutputBox);

        ACG.DestroyAllChildren(Controller.transform);

        string input = await ACG.DisplayWithPrompt("Hello Traveler! Enter your name here:");

        await Commands.RunCommand(new Command($"save {input}"), false);

        string worldSize = await ACG.DisplayWithPrompt("How large would you like to make your world, Small(1), Medium(2), Large(3), or XLarge(4)?", true);
        int size;
        while (!int.TryParse(worldSize, out size))
            worldSize = await ACG.DisplayWithPrompt("That wasn't a valid pick... Try again:", true);

        ACG.SpawnCommandLine(Controller.transform);
        Debug.Log(size);
    }
    private void RunCommand(Commands.CommandExecutionCallback cb)
    {
        if(cb.Command.type == Command.CommandType.Clear)
            return;
        //instantiate output box    
        SpawnOutputBox(cb.Output, transform, ACG.OutputType.Default,cb.SpawnCommandLineOnFinish);
    }

    public static Command.CommandType GetCommandType(string consoleInput)
    {
        string result = AutoComplete(Command.CommandNames, consoleInput);

        return Enum.TryParse(result, true, out Command.CommandType type) ? type : Command.CommandType.NOT_FOUND;
    }
    public static string AutoComplete(List<string> commandNames, string input)
    {
        input = input.Trim().ToLower();
        string result = commandNames.FindAll(cName => Matches(cName, input)).FirstOrDefault();
        return result ?? input;
    }
    public static bool Matches(string testAgainst, string input) =>
        testAgainst.ToLower().StartsWith(input) ||
        testAgainst.ToLower().Equals(input) ||
        testAgainst.ToLower().Contains(input);
    public static void SpawnOutputBox(string output, Transform parent, ACG.OutputType outputType = ACG.OutputType.Default, bool spawnCLOnDone = true)
    {
        OutputBox outputBox = ACG.SpawnOutputBox(parent).GetComponent<OutputBox>();

        if (string.IsNullOrEmpty(output)) return;

        outputBox.ShowOutput(output, outputType, spawnCLOnDone);
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
