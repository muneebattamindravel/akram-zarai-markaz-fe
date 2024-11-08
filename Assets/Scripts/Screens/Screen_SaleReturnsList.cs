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

public class Screen_SaleReturnReturnsList : OSA<BaseParamsWithPrefab, SaleReturnReturnsListViewHolder>
{
    public GameObject contentRoot;
    public List<SaleReturn> saleReturnReturns;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;
    public SimpleDataHelper<SaleReturn> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<SaleReturn>(this);
        base.Start();
    }

    protected override SaleReturnReturnsListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new SaleReturnReturnsListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(SaleReturnReturnsListViewHolder newOrRecycled)
    {
        SaleReturn saleReturn = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = saleReturn.id.ToString();
        newOrRecycled.returnDate.text = saleReturn.returnDate.ToString(Constants.DateDisplayFormat);
        newOrRecycled.bookNumber.text = saleReturn.bookNumber;
        newOrRecycled.billNumber.text = saleReturn.billNumber;
        newOrRecycled.productName.text = saleReturn.product.name;
        newOrRecycled.returnQuantity.text = saleReturn.quantity.ToString();
        newOrRecycled.returnAmount.text = saleReturn.returnAmount.ToCommaSeparatedNumbers() + Constants.Currency;

        newOrRecycled.view.onClicked.RemoveAllListeners();
        newOrRecycled.view.onClicked.AddListener(() => {
            ViewSaleReturn(saleReturn.id);
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, saleReturnReturns.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    public void OnEnable()
    {
        dateFilterPicker.onDateSelected += GetSaleReturnReturns;

        dateFilterPicker.gameObject.SetActive(true);
        InitializeColumnsHeaders();
        Preloader.Instance.ShowWindowed();
        GetSaleReturnReturns();
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
                FieldInfo fieldInfo = typeof(SaleReturn).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    saleReturnReturns = saleReturnReturns.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    saleReturnReturns = saleReturnReturns.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    saleReturnReturns = saleReturnReturns.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (SaleReturn item in saleReturnReturns) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(SaleReturn).GetField(header.dataField);
                foreach (SaleReturn filtered in saleReturnReturns.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    private void OnDisable()
    {
        this.dateFilterPicker.onDateSelected -= GetSaleReturnReturns;

        saleReturnReturns = null;
        GC.Collect();
    }

    void GetSaleReturnReturns()
    {
        if (dateFilterPicker.GetDateRange().from != DateTime.MinValue && dateFilterPicker.GetDateRange().to != DateTime.MinValue)
        {
            Debug.Log("Getting SaleReturn Returns for range = " + dateFilterPicker.GetDateRange().from + "," + dateFilterPicker.GetDateRange().to);
            Preloader.Instance.ShowWindowed();
            SalesManager.Instance.GetSaleReturns(dateFilterPicker.GetDateRange(), (response) => {
                saleReturnReturns = response.data;
                saleReturnReturns = saleReturnReturns.OrderByDescending(p => p.returnDate).ThenByDescending(p => p.id).ToList();
                columnHeaders[1].SetState(ColumnState.DESCENDING);
                PopulateData();

            }, null);
        }
    }

    void ViewSaleReturn(int saleReturnId)
    {
        // GUIManager.Instance.OpenScreenExplicitly(MRScreenName.SaleReturn_View_Add);
        // GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_SaleReturn_View_Add>().ShowView(saleReturnId);
    }

    public void Button_ReloadClicked()
    {
        GetSaleReturnReturns();
    }
}

public class SaleReturnReturnsListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, returnDate, bookNumber, billNumber, productName, returnQuantity, returnAmount;
    public MRButton view, delete;
    public TMP_Text returnApplied;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("returnDate", out returnDate);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
        root.GetComponentAtPath("productName", out productName);
        root.GetComponentAtPath("returnQuantity", out returnQuantity);
        root.GetComponentAtPath("returnAmount", out returnAmount);
        root.GetComponentAtPath("Button_View", out view);
    }
}
