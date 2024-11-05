using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Preloader : MonoBehaviour
{
    public static Preloader Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public GameObject windowed, full;
    public void ShowWindowed()
    {
        windowed.SetActive(true);
    }

    public void HideWindowed()
    {
        windowed.SetActive(false);
    }

    public void ShowFull()
    {
        full.SetActive(true);
    }

    public void HideFull()
    {
        full.SetActive(false);
    }
}
