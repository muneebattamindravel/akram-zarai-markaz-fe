
using UnityEngine.Events;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MRDroplist : MonoBehaviour
{
    GameObject droplist;
    TMP_InputField inputfield;
    Transform contentRoot;
    public UnityEvent onPopulateItems, onDroplistClosed;
    public UnityEvent<string> onInputEndEdit;
    public GameObject droplistItemPrefab;
    GameObject buttonCloseDroplist;

    public int poolCount = 150;
    List<GameObject> pooledObjects;

    static bool droplistAvailable = true;

    public bool isEnabled = true;

    private void Awake()
    {
        buttonCloseDroplist = transform.GetChild(0).gameObject;
        droplist = transform.GetChild(1).gameObject;
        droplist.SetActive(false);
        buttonCloseDroplist.GetComponent<Button>().onClick.AddListener(InputDeSelected);
        inputfield = transform.GetChild(2).GetComponent<TMP_InputField>();
        inputfield.GetComponent<TMP_InputField>().onEndEdit.AddListener(InputEndEdit);
        inputfield.GetComponent<TMP_InputField>().onSelect.AddListener(InputSelected);
        contentRoot = droplist.transform.Find("Viewport").transform.Find("Content");
        ClearContentDroplist();
        buttonCloseDroplist.SetActive(false);
    }

    private void Start()
    {
        if (!isEnabled)
            return;

        pooledObjects = new List<GameObject>();
        CreatePoolObjects(poolCount);
    }

    void CreatePoolObjects(int count)
    {
        if (!isEnabled)
            return;

        GameObject newObject;
        for (int i = 0; i < count; i++)
        {
            newObject = Instantiate(droplistItemPrefab);
            newObject.SetActive(false);
            newObject.transform.parent = contentRoot.transform;
            newObject.transform.localScale = Vector3.one;
            pooledObjects.Add(newObject);
        }
    }

    public GameObject GetPooledItem()
    {

        int i = 0;
        for (; i < poolCount; i++)
        {
            if (!pooledObjects[i].activeSelf)
                return pooledObjects[i];
        }

        CreatePoolObjects(5);
        return pooledObjects[i];
    }

    public void InputSelected(string value)
    {
        if (!isEnabled)
            return;

        if (droplistAvailable)
        {
            droplistAvailable = false;
            PopulateItems();
            droplist.SetActive(true);
            inputfield.text = "";
            buttonCloseDroplist.SetActive(true);
        }
    }

    public void InputDeSelected()
    {
        if (!isEnabled)
            return;

        droplist.SetActive(false);
        inputfield.text = "";
        buttonCloseDroplist.SetActive(false);
        droplistAvailable = true;
        if (onDroplistClosed != null) onDroplistClosed.Invoke();
    }

    public void InputEndEdit(string value)
    {
        if (!isEnabled)
            return;

        if (onInputEndEdit != null) onInputEndEdit.Invoke(value);
    }

    public void ClearContentDroplist()
    {
        if (!isEnabled)
            return;

        foreach (Transform t in contentRoot) t.gameObject.SetActive(false);
    }

    public void PopulateItems()
    {
        if (!isEnabled)
            return;

        if (onPopulateItems != null) onPopulateItems.Invoke();
    }
}
