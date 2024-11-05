using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System.Linq;

public class Screen_ProfitReport : OSA<BaseParamsWithPrefab, ProfitReportListViewHolder>
{
    public MRDateFilterPicker dateFilterPicker;
    public GameObject buttonCompaniesHighlight, buttonCategoriesHighlight, buttonProductsHighlight;
    public TMP_Text text_columnHeaderName, text_totalSales, text_totalGrossProfit, text_totalExpenses, text_totalNetProfit;

    public GameObject contentRoot;
    public List<Sale> sales;
    public List<SaleProfit> saleProfits;
    public List<Expense> expenses;
    Dictionary<string, ReportValues> companiesReport; 
    Dictionary<string, ReportValues> categoriesReport;
    Dictionary<string, ReportValues> productsReport;

    class ReportValues
    {
        public float sale;
        public float profit;
    }

    public class PrintValues
    {
        public int id;
        public string name;
        public float totalSale;
        public float totalProfit;
        public float percentage;
    };

    public SimpleDataHelper<PrintValues> Data { get; private set; }
    public List<PrintValues> printValues;
    protected override void Start()
    {
        Data = new SimpleDataHelper<PrintValues>(this);
        base.Start();
    }

    protected override ProfitReportListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new ProfitReportListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(ProfitReportListViewHolder newOrRecycled)
    {
        PrintValues reportItem = Data[newOrRecycled.ItemIndex];
        newOrRecycled.id.GetComponent<TMP_Text>().text = reportItem.id.ToString();
        newOrRecycled.name.GetComponent<TMP_Text>().text = reportItem.name;

        newOrRecycled.totalSale.GetComponent<TMP_Text>().text = reportItem.totalSale.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.totalProfit.GetComponent<TMP_Text>().text = reportItem.totalProfit.ToCommaSeparatedNumbers() + Constants.Currency;

        if (reportItem.totalProfit > 0)
            newOrRecycled.totalProfit.GetComponent<TMP_Text>().color = Constants.PositiveColor;
        else
            newOrRecycled.totalProfit.GetComponent<TMP_Text>().color = Constants.NegativeColor;

        newOrRecycled.percentage.GetComponent<TMP_Text>().text = ((reportItem.totalProfit * 1.0f / reportItem.totalSale * 1.0f) * 100).ToString("#.##") + " %";
    }


    private void OnEnable()
    {
        dateFilterPicker.gameObject.SetActive(true);

        SalesManager.onSaleAdded += GetData;
        SalesManager.onSaleUpdated += GetData;
        this.dateFilterPicker.onDateSelected += GetData;

        companiesReport = new Dictionary<string, ReportValues>();
        categoriesReport = new Dictionary<string, ReportValues>();
        productsReport = new Dictionary<string, ReportValues>();

        ResetHighlight();
        GetData();
    }

    void ResetHighlight()
    {
        buttonCompaniesHighlight.SetActive(false);
        buttonCategoriesHighlight.SetActive(false);
        buttonProductsHighlight.SetActive(false);
    }

    public void ShowProfitByCompanies()
    {
        ResetHighlight();
        buttonCompaniesHighlight.SetActive(true);
        text_columnHeaderName.text = Constants.Company;

        PopulateData(Constants.Company);
    }               

    public void ShowProfitByCategories()
    {
        ResetHighlight();
        buttonCategoriesHighlight.SetActive(true);
        text_columnHeaderName.text = Constants.Category;

        PopulateData(Constants.Category);
    }

    public void ShowProfitByProducts()
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
            values.totalProfit = reportEntry.Value.profit;
            values.totalSale = reportEntry.Value.sale;
            this.printValues.Add(values);
            index++;
        }

        this.printValues = this.printValues.OrderByDescending(p => p.totalProfit).ToList();

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

        //float totalSales = 0, totalGrossProfit = 0, totalExpenses = 0, totalNetProfit = 0;
        double totalSales = 0, totalGrossProfit = 0, totalExpenses = 0, totalNetProfit = 0;
        List<int> usedSales = new List<int>();

        foreach(Expense expense in expenses)
            totalExpenses += expense.amount;

        foreach (Sale sale in sales)
            totalSales += sale.totalAmount;

        foreach (SaleProfit saleProfit in saleProfits)
        {
            totalGrossProfit += saleProfit.amount;
            float saleProfitSaleAmount = saleProfit.sale.totalAmount;
            bool add = true;
            if (usedSales.Contains(saleProfit.sale.id))
                add = false;

            if (companiesReport.ContainsKey(saleProfit.saleItem.product.company.name))
            {
                ReportValues val = null;
                companiesReport.TryGetValue(saleProfit.saleItem.product.company.name, out val);
                if (add)
                    val.sale += saleProfit.sale.totalAmount;
                val.profit += saleProfit.amount;
                companiesReport[saleProfit.saleItem.product.company.name] = val;
            }
            else
            {
                ReportValues val = new ReportValues();
                if (add)
                    val.sale = saleProfit.sale.totalAmount;
                val.profit = saleProfit.amount;
                companiesReport.Add(saleProfit.saleItem.product.company.name, val);
            }

            if (categoriesReport.ContainsKey(saleProfit.saleItem.product.category.name))
            {
                ReportValues val = null;
                categoriesReport.TryGetValue(saleProfit.saleItem.product.category.name, out val);
                if (add)
                    val.sale += saleProfit.sale.totalAmount;
                val.profit += saleProfit.amount;
                categoriesReport[saleProfit.saleItem.product.category.name] = val;
            }
            else
            {
                ReportValues val = new ReportValues();
                if (add)
                    val.sale = saleProfit.sale.totalAmount;
                val.profit = saleProfit.amount;
                categoriesReport.Add(saleProfit.saleItem.product.category.name, val);
            }

            if (productsReport.ContainsKey(saleProfit.saleItem.product.name))
            {
                ReportValues val = null;
                productsReport.TryGetValue(saleProfit.saleItem.product.name, out val);
                if (add)
                    val.sale += saleProfit.sale.totalAmount;
                val.profit += saleProfit.amount;
                productsReport[saleProfit.saleItem.product.name] = val;
            }
            else
            {
                ReportValues val = new ReportValues();
                if (add)
                    val.sale = saleProfit.sale.totalAmount;
                val.profit = saleProfit.amount;
                productsReport.Add(saleProfit.saleItem.product.name, val);
            }

            usedSales.Add(saleProfit.sale.id);
        }

        totalNetProfit = totalGrossProfit - totalExpenses;

        if (totalNetProfit < 0)
            text_totalNetProfit.color = Constants.NegativeColor;
        else
            text_totalNetProfit.color = Constants.PositiveColor;

        text_totalSales.text = String.Format("{0:0.00}", totalSales); 
        text_totalGrossProfit.text = String.Format("{0:0.00}", totalGrossProfit); 
        text_totalExpenses.text = String.Format("{0:0.00}", totalExpenses); 
        text_totalNetProfit.text = String.Format("{0:0.00}", totalNetProfit); 
    }

    private void OnDisable()
    {
        SalesManager.onSaleAdded -= GetData;
        SalesManager.onSaleUpdated -= GetData;
        this.dateFilterPicker.onDateSelected -= GetData;

        printValues = null;
        categoriesReport = null;
        productsReport = null;
        companiesReport = null;
        GC.Collect();
    }

    void GetData()
    {
        Preloader.Instance.ShowWindowed();
        MRDateRange range = dateFilterPicker.GetDateRange();
        SalesManager.Instance.GetSales(dateFilterPicker.GetDateRange(), (response) => {
            sales = response.data;
            ProfitsManager.Instance.GetSaleProfits(range, (response) => {
                saleProfits = response.data;
                ExpensesManager.Instance.GetExpenses(range, (response) => {
                    expenses = response.data;
                    CalculateTotals();
                    ShowProfitByCompanies();
                }, null);
            });
        }, null);
    }
}

public class ProfitReportListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, totalSale, totalProfit, percentage;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("totalSale", out totalSale);
        root.GetComponentAtPath("totalProfit", out totalProfit);
        root.GetComponentAtPath("percentage", out percentage);
    }
}