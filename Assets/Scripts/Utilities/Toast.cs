using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Toast : MonoBehaviour
{
    public TMP_Text text_title, text_message;
    public GameObject container, tick, cross;
    public void ShowToast(string title, string message, bool isPositive = true)
    {
        tick.SetActive(false); cross.SetActive(false);
        if (isPositive) tick.SetActive(true);
        else cross.SetActive(true);

        text_title.text = title; text_message.text = message;

        container.transform.DOLocalMoveX(650f, 0.3f);

        GetComponent<CanvasGroup>().DOFade(0f, 1f).SetDelay(3f).OnComplete(() => {
            if (gameObject) Destroy(gameObject);
        });
    }

    public void OnClicked()
    {
        Destroy(gameObject);
    }
}
