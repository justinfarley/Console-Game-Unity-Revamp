using System;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public static class Shortcuts
{
    public static bool DetectSubmitCommand(TMP_InputField field, CommandLine cmdLine, TMP_SelectionCaret caret)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return false;

        if (cmdLine.IsConfirmationPrompt)
        {
            bool confirmed = cmdLine.RawInput.Equals("CONFIRM");
            CommandLine.OnConfirmationPromptAnswered?.Invoke(confirmed);
            Cleanup(field, cmdLine, caret);
            return true;
        }

        string input = field.text.Replace(ACG.FullPath, "");
        Command command = new(input);
        _ = Commands.RunCommand(command);

        if(!string.IsNullOrEmpty(input)) 
            ConsoleController.CommandList.AddLast(input);
        ConsoleController.CurrentCommand = null;

        //Cleanup
        Cleanup(field, cmdLine, caret);
        return true;
    }
    public static void DetectGetPreviousCommand(TMP_InputField field, Action endAction = null)
    {
        if (!Input.GetKeyDown(KeyCode.UpArrow)) return;
        field.text = $"{ACG.FullPath}{ConsoleController.GetPrevCommand()}";
        endAction?.Invoke();
    }
    public static void DetectGetNextCommand(TMP_InputField field, Action endAction = null)
    {
        if (!Input.GetKeyDown(KeyCode.DownArrow)) return;
        field.text = $"{ACG.FullPath}{ConsoleController.GetNextCommand()}";
        endAction?.Invoke();
    }
    public static void DetectTabAutoComplete(TMP_InputField field, Action endAction = null)
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;
        string input = field.text.Replace(ACG.FullPath, "");
        field.text = $"{ACG.FullPath}{ConsoleController.AutoComplete(Command.CommandNames, input)}";
        endAction?.Invoke();
    }

    #region Private API
    private static void Cleanup(TMP_InputField field, CommandLine cmdLine, TMP_SelectionCaret caret)
    {
        field.enabled = false;
        UnityEngine.Object.Destroy(caret.gameObject);
        UnityEngine.Object.Destroy(cmdLine);
    }

    #endregion
}
