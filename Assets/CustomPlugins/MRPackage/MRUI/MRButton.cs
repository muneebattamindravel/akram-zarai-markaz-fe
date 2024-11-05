using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MRButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float fadeTime = 0.3f;
    public float onHoverAlpha = 0.6f;
    public float onClickAlpha = 0.5f;

    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    [SerializeField]
    public ButtonClickedEvent onClicked = new ButtonClickedEvent();

    private CanvasGroup canvasGroup;

    bool _interactible = true;
    public bool interactible
    {
        get { return _interactible; }
        set {
            _interactible = value;
            //if (value)
            //    StartCoroutine(FadeIn(canvasGroup, 1.0f, fadeTime));
            //else
            //    StartCoroutine(FadeOut(canvasGroup, onHoverAlpha, fadeTime));
        }
    }

    private void Awake()
    {
        if (GetComponent<CanvasGroup>() == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        else
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactible) return;
        StopAllCoroutines();
        StartCoroutine(FadeOut(canvasGroup, onHoverAlpha, fadeTime));
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!interactible) return;
        StopAllCoroutines();
        StartCoroutine(FadeIn(canvasGroup, 1.0f, fadeTime));
    }

    string thisObjectName;
    int thisObjectDepth;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!interactible) return;
        thisObjectName = eventData.pointerCurrentRaycast.gameObject.name;
        thisObjectDepth = eventData.pointerCurrentRaycast.depth;
        canvasGroup.alpha = onClickAlpha;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!interactible) return;
        canvasGroup.alpha = 1.0f;

        if (eventData != null && eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (thisObjectName == eventData.pointerCurrentRaycast.gameObject.name && thisObjectDepth == eventData.pointerCurrentRaycast.depth)
            {
                onClicked.Invoke();
                if (MRSoundManager.Instance)
                    MRSoundManager.Instance.Play(SoundType.BUTTON_CLICK);
            }
        }
    }

    IEnumerator FadeIn(CanvasGroup group, float alpha, float duration)
    {
        var time = 0.0f;
        var originalAlpha = group.alpha;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
            yield return new WaitForEndOfFrame();
        }

        group.alpha = alpha;
    }

    IEnumerator FadeOut(CanvasGroup group, float alpha, float duration)
    {
        var time = 0.0f;
        var originalAlpha = group.alpha;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
            yield return new WaitForEndOfFrame();
        }

        group.alpha = alpha;
    }
}