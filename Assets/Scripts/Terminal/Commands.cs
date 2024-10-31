using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Commands
{
    public static async Task RunCommand(Command command)
    {
        string output = await GetCommandOutput(command, command.args);
        ConsoleController.OnCommandExecuted?.Invoke(CommandExecutionCallback.New(command,output));
    }
    public static async Task<string> GetCommandOutput(Command command, params string[] args) =>
        command.type switch
        {
            Command.CommandType.Move => Move(args),
            Command.CommandType.Help => Help(args),
            Command.CommandType.Clear => Clear(args),
            Command.CommandType.Save => Save(args),
            Command.CommandType.Load => Load(args),
            Command.CommandType.Stats => Stats(args),
            Command.CommandType.DELETE_SAVE => await DeleteSave(args),
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
            username = args[1].ToUpper();
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
    public static string Stats(params string[] args) => PlayerStats.PlayerStatsString();
    public static string Load(params string[] args)
    {
        if (SaveLoad.Load() == null)
            return "No save data exists. Try saving first!";
        else
            return $"Loaded <color=yellow>{PlayerStats.UserName}</color>'s Profile Successfully.";
    }
    public static async Task<string> DeleteSave(params string[] args)
    {
        OutputBox outputBox = ACG.SpawnOutputBox(ConsoleController.Controller.transform).GetComponent<OutputBox>();

        bool gotConfirmation = false;
        bool isConfirmed = false;

        void Confirmation(bool val)
        {
            gotConfirmation = true;
            isConfirmed = val;
        }

        CommandLine.OnConfirmationPromptAnswered += Confirmation;

        outputBox.ShowOutput("Are you sure you want to delete all saved data?\nType CONFIRM to verify.", true);

        while (!gotConfirmation)
            await Awaitable.EndOfFrameAsync();

        CommandLine.OnConfirmationPromptAnswered -= Confirmation;

        if (isConfirmed)
        {
            PlayerStats.ResetPlayerStats();
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

        public CommandExecutionCallback(Command command, string output)
        {
            this.Command = command;
            this.Output = output;
        }

        public static CommandExecutionCallback New(Command command, string output) => new CommandExecutionCallback(command, output);
    }
}
