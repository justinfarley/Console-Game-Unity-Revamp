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
    public bool isComplete = false;
    public static Action OnOutputBoxTextShown = null;
    private RectTransform rectTransform;
    private float yIncrease;
    public async Task ShowOutput(string output, ACG.OutputType outputType = ACG.OutputType.Default, bool spawnCLineOnComplete = true)
    {
        _output = output;
        rectTransform = transform.GetComponent<RectTransform>();
        if (outputType == ACG.OutputType.Prompt)
            CommandLine.OnPromptAnswered += s => isComplete = true;
        else if (outputType == ACG.OutputType.Confirmation)
            CommandLine.OnConfirmationAnswered += s => isComplete = true;
        else if(outputType == ACG.OutputType.Default)
            OnOutputBoxTextShown += () => isComplete = true;

        yIncrease = rectTransform.sizeDelta.y;

        var taskCompletionSource = new TaskCompletionSource<bool>();

        outputQueue.Enqueue(taskCompletionSource);

        while (outputQueue.Peek() != taskCompletionSource)
            await Awaitable.NextFrameAsync();

        typewriter = GetComponent<TypewriterByCharacter>();
        tmp = GetComponent<TMP_Text>();

        typewriter.onCharacterVisible.AddListener(CharacterShown);
        typewriter.onTextShowed.AddListener(() => StartCoroutine(DestroyBuffer()));
        typewriter.onTextShowed.AddListener(() => OnOutputBoxTextShown?.Invoke());
        if (spawnCLineOnComplete)
            typewriter.onTextShowed.AddListener(() => SpawnCommandLine(outputType));

        typewriter.ShowText(_output);

        await WaitUntilTextIsShown();

        taskCompletionSource.SetResult(true);
        outputQueue.Dequeue();
    }

    private async Task WaitUntilTextIsShown()
    {
        while (!isComplete)
            await Awaitable.NextFrameAsync();
    }

    private void CharacterShown(char ch)
    {
        if (ch != '\n') return;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + yIncrease);
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
