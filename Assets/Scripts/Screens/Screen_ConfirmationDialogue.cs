using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using TMPro;
using System;

public class Screen_ConfirmationDialogue : MonoBehaviour
{
    public TMP_Text text_message, text_yes, text_no;
    Action onConfirmed, onCancelled;

    public void ShowView(string message, string yesText, string noText, Action onConfirm, Action onCancel)
    {
        onConfirmed = onConfirm;
        onCancelled = onCancel;
        text_message.text = message;
        text_yes.text = yesText;
        text_no.text = noText;
    }

    public void Button_YesClicked()
    {
        if (onConfirmed != null) onConfirmed.Invoke();
        GUIManager.Instance.Back();
    }

    public void Button_CloseClicked() {
        if (onCancelled != null) onCancelled.Invoke();
        GUIManager.Instance.Back();
    }
}
