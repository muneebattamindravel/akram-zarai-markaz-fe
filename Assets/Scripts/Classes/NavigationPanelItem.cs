using UnityEngine;
using UnityEngine.UI;

public class NavigationPanelItem : MonoBehaviour {
    public MRScreenName screenToOpen;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            MRSoundManager.Instance.Play(SoundType.BUTTON_CLICK);
            NavigationPanel.Instance.NavigateTo(screenToOpen);
        });
    }
}