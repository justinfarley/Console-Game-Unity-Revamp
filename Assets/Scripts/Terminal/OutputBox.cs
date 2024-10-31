using Febucci.UI;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    private TypewriterByCharacter typewriter;
    private static GameObject commandLinePrefab;
    private static ConsoleController console;
    private TMP_Text tmp;

    public void ShowOutput(string output)
    {
        typewriter = GetComponent<TypewriterByCharacter>();
        tmp = typewriter.GetComponent<TMP_Text>();
        typewriter.onCharacterVisible.AddListener(CharacterShown);
        typewriter.onTextShowed.AddListener(() => ACG.SpawnCommandLine(transform.parent));
        typewriter.ShowText(output);
        
    }

    private void CharacterShown(char ch)
    {
        if (!tmp.isTextOverflowing) return;

        string fittingText;
        string overflowingText;

        if (typewriter.TextAnimator.textFull.IndexOf(ch) == tmp.firstOverflowCharacterIndex - 1 || ch == '\n')
        {
            fittingText = typewriter.TextAnimator.textFull;
            overflowingText = typewriter.TextAnimator.textFull.Substring(ch == '\n' 
                                                                                    ? typewriter.TextAnimator.textFull.IndexOf(ch) + 1 
                                                                                    : tmp.firstOverflowCharacterIndex);
            typewriter.onCharacterVisible.AddListener(CharacterShown);
            typewriter.onTextShowed.RemoveAllListeners();
            typewriter.StopShowingText();
            tmp.text = fittingText;
            console = console != null ? console : transform.parent.GetComponent<ConsoleController>();

            ConsoleController.SpawnOutputBox(overflowingText, console.transform);

            var ta = typewriter.GetComponent<TextAnimator_TMP>();
            Destroy(typewriter);
            Destroy(ta);
            Destroy(this);
        }
    }
    private void OnDestroy()
    {
        typewriter.onTextShowed.RemoveAllListeners();
    }
}
