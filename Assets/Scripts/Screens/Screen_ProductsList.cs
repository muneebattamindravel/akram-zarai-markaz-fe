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

public class Screen_ProductsList : OSA<BaseParamsWithPrefab, ProductsListViewHolder>
{
    public GameObject contentRoot;
    public List<Product> products, productsFiltered;
    public List<ColumnHeader> columnHeaders;
    public TMP_Text text_totalStockAmount;
    float totalStockAmount = 0;
    public TMP_Dropdown dropdown_company;
    List<Company> companies;
    Company selectedCompany;

    public SimpleDataHelper<Product> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Product>(this);
        base.Start();
    }

    protected override ProductsListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new ProductsListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(ProductsListViewHolder newOrRecycled)
    {
        Product product = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = product.id.ToString();
        newOrRecycled.name.text = product.name.ToString();
        newOrRecycled.salePrice.text = product.salePrice.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.currentStock.text = product.currentStock.ToCommaSeparatedNumbers() + " " + product.unit.name;
        newOrRecycled.currentStockAmount.text = product.currentStockAmount.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.company.text = product.company.name.ToString();
        newOrRecycled.category.text = product.category.name.ToString();

        newOrRecycled.name.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.name.GetComponent<MRButton>().onClicked.AddListener(() => {
            ViewProduct(product.id);
        });

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            ViewProduct(product.id);
        });

        newOrRecycled.viewStockBook.onClicked.RemoveAllListeners();
        newOrRecycled.viewStockBook.onClicked.AddListener(() => {
            ViewStockBook(product.id);
        });

    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        totalStockAmount = 0;
        foreach (Product product in productsFiltered.FindAll(p => p.IsEnabledOnGrid))
            totalStockAmount += product.currentStockAmount;
        text_totalStockAmount.text = totalStockAmount.ToCommaSeparatedNumbers() + Constants.Currency;

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, productsFiltered.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    private void OnEnable()
    {
        GetData();
        ProductsManager.onProductAdded += GetData;
        ProductsManager.onProductUpdated += GetData;
        SalesManager.onSaleAdded += GetData;
        totalStockAmount = 0;

        TMP_Dropdown.OptionData defaultOption = dropdown_company.options[0];
        dropdown_company.options.Clear();
        dropdown_company.options.Add(defaultOption);
        dropdown_company.value = 0;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        ProductsManager.onProductAdded -= GetData;
        ProductsManager.onProductUpdated -= GetData;
        SalesManager.onSaleAdded -= GetData;

        products = null;
        GC.Collect();
    }

    public void InitializeColumnsHeaders()
    {
        foreach (ColumnHeader header in columnHeaders)
        {
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Product item in products) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Product).GetField(header.dataField);
                foreach (Product filtered in products.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetData()
    {
        Preloader.Instance.ShowWindowed();
        ProductsManager.Instance.GetProducts((response) => {
            products = response.data;
            productsFiltered = products;

            CompaniesManager.Instance.GetCompanies((response) => {
                companies = response.data;

                List<string> companyNames = new List<string>();
                foreach (Company company in companies)
                    companyNames.Add(company.name);

                dropdown_company.AddOptions(companyNames);
                dropdown_company.onValueChanged.AddListener((changedValue) => {
                    selectedCompany = companies.Find(p => p.name == dropdown_company.options[changedValue].text);
                    if (selectedCompany != null)
                        productsFiltered = products.FindAll(p => p.company.name == selectedCompany.name);
                    else
                        productsFiltered = products;

                    PopulateData();
                });

                columnHeaders[0].ResetState();
                products = products.OrderBy(p => p.id).ToList();

                PopulateData();

            }, null);

            
        }, (response) => { GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false); });
    }

    void ViewStockBook(int productId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.StockBook);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_StockBook>().ShowView(productId);
    }

    void ViewProduct(int productId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Products_View);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Products_View>().ShowView(productId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Products_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Products_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetData();
    }
}

public class ProductsListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, salePrice, currentStock, currentStockAmount, company, category;
    public MRButton edit, viewStockBook;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("salePrice", out salePrice);
        root.GetComponentAtPath("currentStock", out currentStock);
        root.GetComponentAtPath("currentStockAmount", out currentStockAmount);
        root.GetComponentAtPath("company", out company);
        root.GetComponentAtPath("category", out category);
        root.GetComponentAtPath("Button_Edit", out edit);
        root.GetComponentAtPath("Button_ViewStockBook", out viewStockBook);
    }
}