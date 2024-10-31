using System.Collections.Generic;
using UnityEngine;

public static class Commands
{
    public static void RunCommand(Command command)
    {
        ConsoleController.OnCommandExecuted?.Invoke(CommandExecutionCallback.New(command,GetCommandOutput(command, command.args)));
    }
    public static string GetCommandOutput(Command command, params string[] args) =>
        command.type switch
        {
            Command.CommandType.Move => Move(args),
            Command.CommandType.Help => Help(args),
            Command.CommandType.Clear => Clear(args),
            Command.CommandType.Save => Save(args),
            Command.CommandType.Load => Load(args),
            _ => "<color=red>Command Not Found!",
        };
    // TODO: arg1 should be page (eventually)
    public static string Help(params string[] args) => "Here is a list of commands: \n" + Command.GetListOfCommands();
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
            username = args[1];
            if(username != null)
            {
                if (username.Length <= 15)
                    PlayerStats.UpdateUserName(username);
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
    public static string Load(params string[] args)
    {
        SaveLoad.Load();
        return $"Loaded <color=yellow>{PlayerStats.UserName}</color>'s Profile Successfully.";
    }
    public class CommandExecutionCallback
    {
        public Command Command;
        public string Output;

        public CommandExecutionCallback(Command command, string output)
        {
            this.Command = command;
            this.Output = output;
        }

        public static CommandExecutionCallback New(Command command, string output) => new CommandExecutionCallback(command, output);
    }
}
