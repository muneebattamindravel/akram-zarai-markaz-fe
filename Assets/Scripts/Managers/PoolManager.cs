using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public enum PoolType
    {
        StockReportList,
        ProductStockList,
        StockBookList,
        ProductStockItem,
    };

    List<GameObject> productStockListItemPool = new List<GameObject>();
    List<GameObject> stockBookListItemPool = new List<GameObject>();
    List<GameObject> productStockItemPool = new List<GameObject>();

    public GameObject productStockListItemPrefab;
    public GameObject stockBookListItemPrefab;
    public GameObject productStockItemPrefab;

    private void Start()
    {
        CreatePoolObjects(productStockListItemPool, productStockListItemPrefab, 50);
        CreatePoolObjects(stockBookListItemPool, stockBookListItemPrefab, 100);
        CreatePoolObjects(productStockItemPool, productStockItemPrefab, 50);
    }

    void CreatePoolObjects(List<GameObject> poolList, GameObject itemPrefab, int poolCount)
    {
        GameObject newObject;
        for (int i = 0; i < poolCount; i++)
        {
            newObject = Instantiate(itemPrefab);
            newObject.SetActive(false);
            poolList.Add(newObject);
            newObject.transform.parent = this.transform;
        }
    }

    public GameObject GetPooledItem(PoolType type)
    {
        if (type == PoolType.ProductStockList) return GetPooledItem(productStockListItemPool, productStockListItemPrefab);
        else if (type == PoolType.StockBookList) return GetPooledItem(stockBookListItemPool, stockBookListItemPrefab);
        else if (type == PoolType.ProductStockItem) return GetPooledItem(productStockItemPool, productStockItemPrefab);

        return null;
    }

    GameObject GetPooledItem(List<GameObject> poolList, GameObject itemPrefab)
    {
        int i = 0;
        for (; i < poolList.Count; i++)
        {
            if (!poolList[i].activeSelf)
                return poolList[i];
        }

        CreatePoolObjects(poolList, itemPrefab, 5);
        return poolList[i];
    }
}
