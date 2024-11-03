using Febucci.UI;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    private static Queue<TaskCompletionSource<bool>> outputQueue = new Queue<TaskCompletionSource<bool>>();
    private TypewriterByCharacter typewriter;
    private TMP_Text tmp;
    private string _output;
    private ACG.OutputType outputType = ACG.OutputType.Default;
    public bool isTextComplete = false;

    public async Task ShowOutput(string output, ACG.OutputType outputType = ACG.OutputType.Default, bool spawnCLineOnComplete = true)
    {
        try
        {

            var taskCompletionSource = new TaskCompletionSource<bool>();
            outputQueue.Enqueue(taskCompletionSource);

            while (outputQueue.Peek() != taskCompletionSource)
                await Awaitable.NextFrameAsync();

            this.outputType = outputType;
            _output = (output.IndexOf('\n') == -1 ? output : output.Substring(0, output.IndexOf('\n')));
            typewriter = GetComponent<TypewriterByCharacter>();
            tmp = GetComponent<TMP_Text>();
            typewriter.onCharacterVisible.AddListener(CharacterShown);

            if(spawnCLineOnComplete)
                typewriter.onTextShowed.AddListener(() => SpawnCommandLine(this.outputType));

            typewriter.ShowText(_output);

            await WaitUntilTextIsShown();

            taskCompletionSource.SetResult(true);
            outputQueue.Dequeue();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private async Task WaitUntilTextIsShown()
    {
        void Complete() => isTextComplete = true;

        typewriter.onTextShowed.AddListener(Complete);

        while (!isTextComplete)
            await Awaitable.NextFrameAsync();

        typewriter.onTextShowed.RemoveListener(Complete);
    }

    private async void CharacterShown(char ch)
    {
        if (!tmp.isTextOverflowing) return;

        string fittingText;
        string overflowingText;

        int charIndex = typewriter.TextAnimator.textFull.IndexOf(ch);

        if (ch == '\n')
        {
            fittingText = typewriter.TextAnimator.textFull;
            overflowingText = typewriter.TextAnimator.textFull.Substring(charIndex + 1);

            typewriter.onCharacterVisible.RemoveAllListeners();
            typewriter.onTextShowed.RemoveAllListeners();
            typewriter.StopShowingText();
            tmp.text = fittingText;
            isTextComplete = true;
            await ConsoleController.SpawnOutputBox(overflowingText, ConsoleController.Controller.transform, outputType, true);

            StartCoroutine(DestroyBuffer());
        }
    }
    private void SpawnCommandLine(ACG.OutputType outputType)
    {
        CommandLine cl = ACG.SpawnCommandLine(transform.parent).GetComponent<CommandLine>();
        cl.OutputType = outputType;
    }

    private IEnumerator DestroyBuffer()
    {
        yield return new WaitForSeconds(0.1f);
        var ta = typewriter.GetComponent<TextAnimator_TMP>();
        Destroy(typewriter);
        Destroy(ta);
        tmp.text = _output;
        Destroy(this);
    }
    private void OnDestroy()
    {
        typewriter.onCharacterVisible?.RemoveAllListeners();
        typewriter.onTextShowed?.RemoveAllListeners();
    }
}
