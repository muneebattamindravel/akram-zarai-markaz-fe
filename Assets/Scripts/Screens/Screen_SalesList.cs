using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using TMPro;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

public class Screen_SalesList : OSA<BaseParamsWithPrefab, SalesListViewHolder>
{
    public GameObject contentRoot;
    public List<Sale> sales;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;

    public TMP_Text text_totalSale, text_totalProfit;
    float totalSale = 0.00f, totalProfit = 0.00f;

    public SimpleDataHelper<Sale> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Sale>(this);
        base.Start();
    }

    protected override SalesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new SalesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(SalesListViewHolder newOrRecycled)
    {
        Sale sale = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = sale.id.ToString();

        newOrRecycled.returnApplied.gameObject.SetActive(false);
        if (sale.returnApplied)
            newOrRecycled.returnApplied.gameObject.SetActive(true);

        newOrRecycled.totalAmount.text = sale.totalAmount.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.saleDate.text = sale.saleDate.ToString(Constants.DateDisplayFormat);
        newOrRecycled.profitAmount.text = sale.profitAmount.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.receivedAmount.text = sale.receivedAmount.ToCommaSeparatedNumbers() + Constants.Currency;

        newOrRecycled.bookNumber.text = sale.bookNumber;
        newOrRecycled.billNumber.text = sale.billNumber;

        if (sale.profitAmount >= 0)
            newOrRecycled.profitAmount.color = Constants.PositiveColor;
        else
            newOrRecycled.profitAmount.color = Constants.NegativeColor;

        newOrRecycled.customerName.text = "";
        if (sale.contact != null)
            newOrRecycled.customerName.text = sale.contact.name;

        newOrRecycled.delete.onClicked.RemoveAllListeners();
        newOrRecycled.delete.onClicked.AddListener(() => {
            DeleteSaleCompletely(sale.id);
        });

        newOrRecycled.view.onClicked.RemoveAllListeners();
        newOrRecycled.view.onClicked.AddListener(() => {
            ViewSale(sale.id);
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        totalSale = 0.00f;
        totalProfit = 0.00f;

        foreach (Sale s in sales)
        {
            totalSale += s.totalAmount;
            totalProfit += s.profitAmount;
        }

        text_totalProfit.text = totalProfit.ToCommaSeparatedNumbers();
        text_totalSale.text = totalSale.ToCommaSeparatedNumbers();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, sales.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    public void OnEnable()
    {
        SalesManager.onSaleAdded += GetSales;
        SalesManager.onSaleUpdated += GetSales;
        SalesManager.onReceivedSalePayment += GetSales;
        dateFilterPicker.onDateSelected += GetSales;

        dateFilterPicker.gameObject.SetActive(true);
        InitializeColumnsHeaders();
        Preloader.Instance.ShowWindowed();
        GetSales();
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
                FieldInfo fieldInfo = typeof(Sale).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    sales = sales.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    sales = sales.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    sales = sales.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Sale item in sales) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Sale).GetField(header.dataField);
                foreach (Sale filtered in sales.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    private void OnDisable()
    {
        SalesManager.onSaleAdded -= GetSales;
        SalesManager.onSaleUpdated -= GetSales;
        SalesManager.onReceivedSalePayment += GetSales;
        this.dateFilterPicker.onDateSelected -= GetSales;

        sales = null;
        GC.Collect();
    }

    void GetSales()
    {
        if (dateFilterPicker.GetDateRange().from != DateTime.MinValue && dateFilterPicker.GetDateRange().to != DateTime.MinValue)
        {
            Debug.Log("Getting Sales for range = " + dateFilterPicker.GetDateRange().from + "," + dateFilterPicker.GetDateRange().to);
            Preloader.Instance.ShowWindowed();
            SalesManager.Instance.GetSales(dateFilterPicker.GetDateRange(), (response) => {
                sales = response.data;
                sales = sales.OrderByDescending(p => p.saleDate).ThenByDescending(p => p.id).ToList();
                columnHeaders[1].SetState(ColumnState.DESCENDING);
                PopulateData();

            }, null);
        }
    }

    void ReceiveSalePayment(int saleId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.ReceiveSalePayment);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ReceiveSalePayment>().ShowView(saleId);
    }

    void ViewSale(int saleId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Sale_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Sale_View_Add>().ShowView(saleId);
    }

    void DeleteSaleCompletely(int saleId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Sale,
            Constants.DELETE, Constants.CANCEL,
            () => {
                //Confirmed
                Preloader.Instance.ShowFull();
                SalesManager.Instance.DeleteSale(saleId, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.SaleDeleted);
                    GetSales();
                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
                });
            },
            () => {
                //Cancelled
            }
        );
    }

    public void Button_ReloadClicked()
    {
        GetSales();
    }
}

public class SalesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, saleDate, bookNumber, billNumber, totalAmount, profitAmount, customerName, receivedAmount;
    public MRButton view, delete;
    public TMP_Text returnApplied;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("saleDate", out saleDate);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
        root.GetComponentAtPath("totalAmount", out totalAmount);
        root.GetComponentAtPath("profitAmount", out profitAmount);
        root.GetComponentAtPath("customerName", out customerName);
        root.GetComponentAtPath("receivedAmount", out receivedAmount);
        root.GetComponentAtPath("Button_View", out view);
        root.GetComponentAtPath("Button_Delete", out delete);
        root.GetComponentAtPath("ReturnApplied", out returnApplied);
    }
}
