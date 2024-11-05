using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PurchasesManager : MonoBehaviour
{
    public static PurchasesManager Instance;
    public delegate void PurchasesEvent();

    public static PurchasesEvent onPurchaseAdded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string PURCHASES_ROUTE = "purchases";

    public void AddPurchase(Purchase purchase, ResponseAction<Purchase> successAction, ResponseAction<Purchase> failAction = null)
    {
        APIManager.Instance.Post<Purchase>(PURCHASES_ROUTE, purchase, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetPurchase(int purchaseId, ResponseAction<Purchase> successAction, ResponseAction<Purchase> failAction = null)
    {
        APIManager.Instance.Get<Purchase>(PURCHASES_ROUTE + "/" + purchaseId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetPurchases(ResponseAction<List<Purchase>> successAction, ResponseAction<List<Purchase>> failAction = null)
    {
        APIManager.Instance.Get<List<Purchase>>(PURCHASES_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdatePurchase(Purchase purchase, int purchaseId, ResponseAction<Purchase> successAction, ResponseAction<Purchase> failAction = null)
    {
        APIManager.Instance.Patch<Purchase>(PURCHASES_ROUTE + "/" + purchaseId, purchase, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeletePurchase(int purchaseId, ResponseAction<Purchase> successAction, ResponseAction<Purchase> failAction = null)
    {
        APIManager.Instance.Delete<Purchase>(PURCHASES_ROUTE + "/" + purchaseId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }
}
