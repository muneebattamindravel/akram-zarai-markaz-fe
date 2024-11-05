using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

public class Screen_PurchasesList : OSA<BaseParamsWithPrefab, PurchasesListViewHolder>
{
    public GameObject contentRoot;
    public List<Purchase> purchases;
    public List<ColumnHeader> columnHeaders;
    public TMP_Text text_totalPurchaseAmount;
    float totalPurchaseAmount = 0;

    public SimpleDataHelper<Purchase> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Purchase>(this);
        base.Start();
    }

    protected override PurchasesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new PurchasesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(PurchasesListViewHolder newOrRecycled)
    {
        Purchase purchase = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = purchase.id.ToString();
        newOrRecycled.invoiceNumber.text = purchase.invoiceNumber.ToString();
        newOrRecycled.invoiceDate.text = purchase.invoiceDate.ToString(Constants.DateDisplayFormat);
        newOrRecycled.companyName.text = purchase.company.name;
        newOrRecycled.purchaseType.text = purchase.purchaseType.ToString();
        newOrRecycled.contactName.text = purchase.contact.name;
        newOrRecycled.totalAmount.text = purchase.totalAmount.ToCommaSeparatedNumbers() + Constants.Currency;

        newOrRecycled.view.onClicked.RemoveAllListeners();
        newOrRecycled.view.onClicked.AddListener(() => {
            ViewPurchase(purchase.id);
        });

        newOrRecycled.delete.onClicked.RemoveAllListeners();
        newOrRecycled.delete.onClicked.AddListener(() => {
            DeletePurchaseCompletely(purchase.id);
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        totalPurchaseAmount = 0f;
        foreach (Purchase p in purchases)
            totalPurchaseAmount += p.totalAmount;

        text_totalPurchaseAmount.text = totalPurchaseAmount.ToCommaSeparatedNumbers();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, purchases.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    private void OnEnable()
    {
        GetPurchases();
        PurchasesManager.onPurchaseAdded += GetPurchases;
        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        PurchasesManager.onPurchaseAdded -= GetPurchases;
        purchases = null;
        GC.Collect();
    }

    public void InitializeColumnsHeaders()
    {
        foreach (ColumnHeader header in columnHeaders)
        {
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.RemoveAllListeners();
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.AddListener(() => {
                foreach (ColumnHeader hdr in columnHeaders)
                    if (hdr != header)
                        hdr.ResetState();

                ColumnState nextState = header.SetNextState();
                FieldInfo fieldInfo = typeof(Purchase).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    purchases = purchases.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    purchases = purchases.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    purchases = purchases.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Purchase item in purchases) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Purchase).GetField(header.dataField);
                foreach (Purchase filtered in purchases.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetPurchases()
    {
        Preloader.Instance.ShowWindowed();
        PurchasesManager.Instance.GetPurchases((response) => {
            purchases = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        });
    }

    void ViewPurchase(int purchaseId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Purchases_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Purchases_View_Add>().ShowView(purchaseId, true);
    }

    void DeletePurchaseCompletely(int purchaseId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Purchase,
            Constants.DELETE, Constants.CANCEL,
            () =>
            {
                //Confirmed
                Preloader.Instance.ShowFull();

                PurchasesManager.Instance.DeletePurchase(purchaseId, (res) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.PurchaseDeleted);
                    GetPurchases();
                },
                (res) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Error, res.message.message, false);
                });
            }, () => {
                //Cancelled
            }
        );
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Purchases_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Purchases_View_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetPurchases();
    }
}

public class PurchasesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, invoiceNumber, invoiceDate, companyName, contactName, totalAmount, purchaseType, notes;
    public MRButton view, delete;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("invoiceNumber", out invoiceNumber);
        root.GetComponentAtPath("invoiceDate", out invoiceDate);
        root.GetComponentAtPath("companyName", out companyName);
        root.GetComponentAtPath("contactName", out contactName);
        root.GetComponentAtPath("totalAmount", out totalAmount);
        root.GetComponentAtPath("purchaseType", out purchaseType);
        root.GetComponentAtPath("notes", out notes);
        root.GetComponentAtPath("Button_View", out view);
        root.GetComponentAtPath("Button_Delete", out delete);
    }
}
