using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class TabKeyHandler : MonoBehaviour
{
    public Selectable tabDestination;

    private void OnEnable()
    {
        KeyboardManager.tabPressed += OnTabPressed;
    }
    private void OnDisable()
    {
        KeyboardManager.tabPressed -= OnTabPressed;
    }

    public void OnTabPressed()
    {
        Debug.Log("CURRENT = " + EventSystem.current.currentSelectedGameObject.name);
        if (EventSystem.current.currentSelectedGameObject.name == this.gameObject.name)
            EventSystem.current.SetSelectedGameObject(tabDestination.gameObject, new BaseEventData(EventSystem.current));
    }
}
