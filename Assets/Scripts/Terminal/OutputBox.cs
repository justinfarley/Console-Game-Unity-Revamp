using Febucci.UI;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    private TypewriterByCharacter typewriter;
    private static ConsoleController console;
    private TMP_Text tmp;
    private string _output;
    private bool _isConfirmation = false;

    public void ShowOutput(string output, bool isConfirmation = false)
    {
        _isConfirmation = isConfirmation;
        _output = output;
        typewriter = GetComponent<TypewriterByCharacter>();
        tmp = GetComponent<TMP_Text>();
        typewriter.onCharacterVisible.AddListener(CharacterShown);
        typewriter.onTextShowed.AddListener(() => OnTextShown(_isConfirmation));
        typewriter.ShowText(_output);
    }

    private void CharacterShown(char ch)
    {
        if (!tmp.isTextOverflowing) return;

        string fittingText;
        string overflowingText;

        int charIndex = typewriter.TextAnimator.textFull.IndexOf(ch);

        if (charIndex == tmp.firstOverflowCharacterIndex || ch == '\n')
        {
            fittingText = typewriter.TextAnimator.textFull;
            overflowingText = typewriter.TextAnimator.textFull.Substring(charIndex + 1);
            typewriter.onCharacterVisible.RemoveAllListeners();
            typewriter.onTextShowed.RemoveAllListeners();
            typewriter.StopShowingText();
            tmp.text = fittingText;
            console = console != null ? console : transform.parent.GetComponent<ConsoleController>();

            ConsoleController.SpawnOutputBox(overflowingText, console.transform, _isConfirmation);

            StartCoroutine(DestroyBuffer());
        }
    }
    private void OnTextShown(bool isConfirmation)
    {
        CommandLine cl = ACG.SpawnCommandLine(transform.parent).GetComponent<CommandLine>();
        cl.IsConfirmationPrompt = isConfirmation;
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
