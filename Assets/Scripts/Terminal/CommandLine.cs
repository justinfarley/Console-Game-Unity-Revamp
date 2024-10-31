using Newtonsoft.Json.Schema;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CommandLine : MonoBehaviour
{
    private TMP_InputField field;
    private int minCaretPosition = ACG.FullPath.Length;
    private string previousText;
    private int lastValidCaretPosition;
    private TMP_SelectionCaret caret = null;
    private RectTransform fieldRect;
    private void Start()
    {
        field = gameObject.GetComponent<TMP_InputField>();
        field.text = ACG.FullPath;
        previousText = field.text;
        lastValidCaretPosition = Mathf.Max(minCaretPosition, field.caretPosition);
        field.onValueChanged.AddListener(HandleTextChange);
        field.ActivateInputField();
        fieldRect = field.GetComponent<RectTransform>();

        StartCoroutine(InitCaret());
    }
    private void Update()
    {
        CheckShortcuts();
    }
    void LateUpdate()
    {
        if (!field.isFocused)
        {
            field.caretPosition = Mathf.Max(minCaretPosition, field.text.Length);
            field.ActivateInputField();
        }
        if (field.caretPosition < minCaretPosition)
            field.caretPosition = minCaretPosition;
    }
    private void HandleTextChange(string newText)
    {
        if (field.caretPosition < minCaretPosition)
        {

            if (!field.text.StartsWith(ACG.FullPath))
            {
                field.text = Regex.Replace(field.text, $"^{Regex.Escape(ACG.FullPath)}+", "");
                field.text = ACG.FullPath + Regex.Replace(previousText, ".*~SERVER://.*>", "");
            }
            field.caretPosition = lastValidCaretPosition;
        }
        else
        {
            previousText = newText;
            lastValidCaretPosition = field.caretPosition;
        }
        StartCoroutine(InitCaret());
    }
    private void CheckShortcuts()
    {
        Shortcuts.DetectSubmitCommand(field, this, caret);
        Shortcuts.DetectGetPreviousCommand(field, () => field.caretPosition = field.text.Length);
        Shortcuts.DetectGetNextCommand(field, () => field.caretPosition = field.text.Length);
        Shortcuts.DetectTabAutoComplete(field, () => field.caretPosition = field.text.Length);
    }
    private IEnumerator InitCaret()
    {
        while (caret == null)
        {
            caret = transform.parent.GetComponentInChildren<TMP_SelectionCaret>();
            yield return null;
        }

        caret.rectTransform.anchorMax = fieldRect.anchorMax;
        caret.rectTransform.anchorMin = fieldRect.anchorMin;
        caret.rectTransform.pivot = fieldRect.pivot;
        caret.rectTransform.anchoredPosition = new Vector2(fieldRect.anchoredPosition.x, fieldRect.anchoredPosition.y);
    }
    void OnDestroy()
    {
        field.onValueChanged.RemoveListener(HandleTextChange);
    }
}
