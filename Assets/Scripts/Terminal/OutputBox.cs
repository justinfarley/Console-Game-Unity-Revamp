using Febucci.UI;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    private TypewriterByCharacter typewriter;
    private static ConsoleController console;
    private TMP_Text tmp;
    private string _output;
    private ACG.OutputType outputType = ACG.OutputType.Default;

    public async Task ShowOutput(string output, ACG.OutputType outputType = ACG.OutputType.Default, bool spawnCLineOnComplete = true)
    {
        this.outputType = outputType;
        _output = output;
        typewriter = GetComponent<TypewriterByCharacter>();
        tmp = GetComponent<TMP_Text>();
        typewriter.onCharacterVisible.AddListener(CharacterShown);
        if(spawnCLineOnComplete)
            typewriter.onTextShowed.AddListener(() => SpawnCommandLine(this.outputType));
        typewriter.ShowText(_output);

        bool isFinished = false;
        void Done() => isFinished = true;

        typewriter.onTextShowed.AddListener(Done);

        while (!isFinished)
            await Awaitable.EndOfFrameAsync();

        typewriter.onTextShowed.RemoveListener(Done);

    }

    private void CharacterShown(char ch)
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
            console = console != null ? console : transform.parent.GetComponent<ConsoleController>();

            _ = ConsoleController.SpawnOutputBox(overflowingText, console.transform, outputType);

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
        typewriter.onCharacterVisible.RemoveAllListeners();
        typewriter.onTextShowed.RemoveAllListeners();
    }
}
