using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Developed By Muneeb Atta Khan - CoFounder Mindravel Interactive
// Developed for Sirlin Games Interview

public enum ButtonState { REGULAR, HOVERED, PRESSED };

public class HTMLButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    [SerializeField]
    public ButtonClickedEvent onClicked = new ButtonClickedEvent();
    
    ButtonState currentState = ButtonState.REGULAR;

    void ChangeButtonState(ButtonState newState)
    {
        currentState = newState;
        Debug.Log("Current State = " + currentState);
        if (currentState == ButtonState.HOVERED) GetComponent<Image>().color = Color.grey;
        else if (currentState == ButtonState.PRESSED) GetComponent<Image>().color = Color.black;
        else if (currentState == ButtonState.REGULAR) GetComponent<Image>().color = Color.white;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == ButtonState.REGULAR)
        {
            ChangeButtonState(ButtonState.HOVERED);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (currentState != ButtonState.PRESSED)
        {
            ChangeButtonState(ButtonState.REGULAR);
        }
    }

    string thisObjectName;
    int thisObjectDepth;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (currentState != ButtonState.PRESSED)
        {
            ChangeButtonState(ButtonState.PRESSED);
            thisObjectName = eventData.pointerCurrentRaycast.gameObject.name;
            thisObjectDepth = eventData.pointerCurrentRaycast.depth;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData != null && eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (thisObjectName == eventData.pointerCurrentRaycast.gameObject.name && thisObjectDepth == eventData.pointerCurrentRaycast.depth)
            {
                onClicked.Invoke();
                Debug.Log("On Cick Event");
                ChangeButtonState(ButtonState.HOVERED);
            }
            else
            {
                ChangeButtonState(ButtonState.REGULAR);
            }
        }
        else
        {
            ChangeButtonState(ButtonState.REGULAR);
        }
    }
}