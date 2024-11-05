using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NavigationPanel : MonoBehaviour
{
    public static NavigationPanel Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public Color unselectedTextColor;
    public List<NavigationPanelItem> items;

    public GameObject adminPanel, operatorPanel;
    public GameObject adminContentRoot, operatorContentRoot;
    public GameObject buttonAdmin, buttonOperator;

    private void Start()
    {
        adminPanel.SetActive(false);
        operatorPanel.SetActive(false);

        items = new List<NavigationPanelItem>();
        foreach (Transform t in adminContentRoot.transform)
            items.Add(t.gameObject.GetComponent<NavigationPanelItem>());
        foreach (Transform t in operatorContentRoot.transform)
            items.Add(t.gameObject.GetComponent<NavigationPanelItem>());

        OperatorView();

        SlideOut();
        ResetItems();

        StartCoroutine(PrintMemoryUsed());
    }

    public void Button_AdminViewClicked()
    {
        AdminView();
    }

    public void Button_OperatorViewClicked()
    {
        OperatorView();
    }

    void AdminView()
    {
        adminPanel.SetActive(true);
        operatorPanel.SetActive(false);
        buttonAdmin.transform.Find("Image_Highlight").gameObject.SetActive(true);
        buttonOperator.transform.Find("Image_Highlight").gameObject.SetActive(false);
    }

    void OperatorView()
    {
        adminPanel.SetActive(false);
        operatorPanel.SetActive(true);
        buttonAdmin.transform.Find("Image_Highlight").gameObject.SetActive(false);
        buttonOperator.transform.Find("Image_Highlight").gameObject.SetActive(true);
    }

    public TMP_Text text_memory;
    IEnumerator PrintMemoryUsed()
    {
        float mbsUsed = System.GC.GetTotalMemory(false) / 1024 / 1024;
        text_memory.text = mbsUsed.ToString() + " MBs";
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(PrintMemoryUsed());
    }

    public GameObject container;
    public void SlideIn()
    {
        container.SetActive(true);
    }
    public void SlideOut()
    {
        container.SetActive(false);
    }

    public void NavigateTo(MRScreenName screen)
    {
        ResetItems();
        GUIManager.Instance.OpenScreenExplicitly(screen);
        SelectItem(items.Find(p => p.screenToOpen == screen).gameObject);
        TopBar.Instance.FetchTopBarData();
    }

    void ResetItems()
    {
        foreach(NavigationPanelItem item in items)
        {
            item.gameObject.transform.Find("Image_Highlight").gameObject.SetActive(false);
            item.gameObject.transform.Find("Text_Title").GetComponent<TMP_Text>().color = unselectedTextColor;
        }
    }

    void SelectItem(GameObject item)
    {
        item.transform.Find("Image_Highlight").gameObject.SetActive(true);
        item.transform.Find("Text_Title").GetComponent<TMP_Text>().color = Color.white;
    }

    public void Button_CloseClicked()
    {
        Application.Quit();
    }
}
