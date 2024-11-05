using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System.Linq;

public class Screen_StockReport : OSA<BaseParamsWithPrefab, StockReportListViewHolder>
{
    public GameObject buttonCompaniesHighlight, buttonCategoriesHighlight, buttonProductsHighlight;
    public TMP_Text text_columnHeaderName, text_totalStockAmount;
    public GameObject contentRoot;
    public List<Product> products;
    Dictionary<string, ReportValues> companiesReport = new Dictionary<string, ReportValues>();
    Dictionary<string, ReportValues> categoriesReport = new Dictionary<string, ReportValues>();
    Dictionary<string, ReportValues> productsReport = new Dictionary<string, ReportValues>();


    public SimpleDataHelper<PrintValues> Data { get; private set; }
    public List<PrintValues> printValues;
    protected override void Start()
    {
        Data = new SimpleDataHelper<PrintValues>(this);
        base.Start();
    }

    protected override StockReportListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new StockReportListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(StockReportListViewHolder newOrRecycled)
    {
        PrintValues account = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.GetComponent<TMP_Text>().text = account.id.ToString();
        PrintValues reportItem = Data[newOrRecycled.ItemIndex];
        newOrRecycled.id.GetComponent<TMP_Text>().text = reportItem.id.ToString();
        newOrRecycled.name.GetComponent<TMP_Text>().text = reportItem.name;

        newOrRecycled.stockAmount.GetComponent<TMP_Text>().text = reportItem.stockAmount.ToCommaSeparatedNumbers().ToString();
        newOrRecycled.stockValue.GetComponent<TMP_Text>().text = reportItem.stockValue.ToCommaSeparatedNumbers() + Constants.Currency;

        if (reportItem.stockValue > 0)
            newOrRecycled.stockValue.GetComponent<TMP_Text>().color = Constants.PositiveColor;
        else
            newOrRecycled.stockValue.GetComponent<TMP_Text>().color = Constants.NegativeColor;
    }

    class ReportValues
    {
        public float stockAmount;
        public float stockValue;
    }

    public class PrintValues
    {
        public int id;
        public string name;
        public float stockAmount;
        public float stockValue;
    };


    private void OnEnable()
    {
        SalesManager.onSaleAdded += GetProducts;
        SalesManager.onSaleUpdated += GetProducts;

        ResetHighlight();
        GetProducts();
    }

    void ResetHighlight()
    {
        buttonCompaniesHighlight.SetActive(false);
        buttonCategoriesHighlight.SetActive(false);
        buttonProductsHighlight.SetActive(false);
    }

    public void ShowStockByCompanies()
    {
        ResetHighlight();
        buttonCompaniesHighlight.SetActive(true);
        text_columnHeaderName.text = Constants.Company;

        PopulateData(Constants.Company);
    }               

    public void ShowStockByCategories()
    {
        ResetHighlight();
        buttonCategoriesHighlight.SetActive(true);
        text_columnHeaderName.text = Constants.Category;

        PopulateData(Constants.Category);
    }

    public void ShowStockByProducts()
    {
        ResetHighlight();
        buttonProductsHighlight.SetActive(true);
        text_columnHeaderName.text = Constants.Product;

        PopulateData(Constants.Product);
    }

    void PopulateData(string reportType)
    {
        Preloader.Instance.ShowWindowed();

        Dictionary<string, ReportValues> report = companiesReport;
        if (reportType == Constants.Category)
            report = categoriesReport;
        else if (reportType == Constants.Company)
            report = companiesReport;
        else if (reportType == Constants.Product)
            report = productsReport;

        int index = 1;
        this.printValues = new List<PrintValues>();
        foreach (KeyValuePair<string, ReportValues> reportEntry in report)
        {
            PrintValues values = new PrintValues();
            values.id = index;
            values.name = reportEntry.Key;
            values.stockAmount = reportEntry.Value.stockAmount;
            values.stockValue = reportEntry.Value.stockValue;
            this.printValues.Add(values);
            index++;
        }

        this.printValues = this.printValues.OrderByDescending(p => p.stockValue).ToList();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, printValues);

        Preloader.Instance.HideWindowed();
    }

    void CalculateTotals()
    {
        companiesReport.Clear();
        categoriesReport.Clear();
        productsReport.Clear();

        float totalStockAmount = 0;

        foreach(Product product in products)
        {
            totalStockAmount += product.currentStockAmount;

            if (companiesReport.ContainsKey(product.company.name))
            {
                ReportValues val = null;
                companiesReport.TryGetValue(product.company.name, out val);
                val.stockAmount += product.currentStock;
                val.stockValue += product.currentStockAmount;
                companiesReport[product.company.name] = val;
            }
            else
            {
                ReportValues val = new ReportValues();
                val.stockAmount = product.currentStock;
                val.stockValue = product.currentStockAmount;
                companiesReport.Add(product.company.name, val);
            }

            if (categoriesReport.ContainsKey(product.category.name))
            {
                ReportValues val = null;
                categoriesReport.TryGetValue(product.category.name, out val);
                val.stockAmount = product.currentStock;
                val.stockValue = product.currentStockAmount;
                categoriesReport[product.category.name] = val;
            }
            else
            {
                ReportValues val = new ReportValues();
                val.stockAmount = product.currentStock;
                val.stockValue = product.currentStockAmount;
                categoriesReport.Add(product.category.name, val);
            }

            if (productsReport.ContainsKey(product.name))
            {
                ReportValues val = null;
                productsReport.TryGetValue(product.name, out val);
                val.stockAmount = product.currentStock;
                val.stockValue = product.currentStockAmount;
                productsReport[product.name] = val;
            }
            else
            {
                ReportValues val = new ReportValues();
                val.stockAmount = product.currentStock;
                val.stockValue = product.currentStockAmount;
                productsReport.Add(product.name, val);
            }
        }

        text_totalStockAmount.text = totalStockAmount + Constants.Currency;
    }

    private void OnDisable()
    {
        SalesManager.onSaleAdded -= GetProducts;
        SalesManager.onSaleUpdated -= GetProducts;
    }

    void GetProducts()
    {
        Preloader.Instance.ShowWindowed();
        ProductsManager.Instance.GetProducts((response) => {
            products = response.data;
            CalculateTotals();
            ShowStockByProducts();
        });
    }
}

public class StockReportListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, stockAmount, stockValue;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("stockAmount", out stockAmount);
        root.GetComponentAtPath("stockValue", out stockValue);
    }
}