using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public delegate void KeyboardEvent();
public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    EventSystem system;
    void Start()
    {
        system = EventSystem.current;
    }

    public static KeyboardEvent enterPressed, tabPressed;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (tabPressed != null)
            {
                MRSoundManager.Instance.Play(SoundType.BUTTON_CLICK);
                tabPressed.Invoke();
            }

            //if (system.currentSelectedGameObject && system.currentSelectedGameObject.GetComponent<Selectable>() != null)
            //{
            //    Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            //    if (next == null) next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            //    if (next == null) next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight();
            //    if (next == null) next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft();

            //    if (next != null)
            //    {
            //        InputField inputfield = next.GetComponent<InputField>();
            //        if (inputfield != null)
            //            inputfield.OnPointerClick(new PointerEventData(system));

            //        system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            //    }
            //}
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (enterPressed != null)
            {
                MRSoundManager.Instance.Play(SoundType.BUTTON_CLICK);
                enterPressed.Invoke();
            }                
        }
    }
}
