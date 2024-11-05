using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.Dates;
using System;
using System.Globalization;
using UnityEngine.UI;

public class Screen_Purchases_View_Add : MonoBehaviour
{
    public TMP_InputField input_invoiceNumber, input_costPrice,
        input_initialQuantity, input_totalAmount, input_batchNumber, input_notes, input_purchaseNotes;
    public MRDatePicker datepicker_invoiceDate, datepicker_expiryDate;
    public TMP_Dropdown dropdown_company, dropdown_supplier, dropdown_purchaseType;
    public TMP_Text text_totalPurchaseAmount, text_selectedProductName, text_title;
    List<Contact> suppliers;
    List<Product> products;
    List<Product> productsFiltered;
    List<ProductStock> addedproductstocks;
    float totalPurchaseAmount;
    ViewMode mode;
    Contact selectedSupplier;
    Product selectedProduct;

    public GameObject productsDroplistContentRoot;
    public GameObject productDroplistItem;

    public MRDroplist droplist_products;

    public GameObject productStocksContentRoot;
    public GameObject productStocksItem;

    List<Company> companies;
    Company selectedCompany;

    public GameObject buttonSave, buttonAdd;

    int purchaseId;
    Purchase purchase;

    private void OnEnable()
    {
        //KeyboardManager.enterPressed += OnEnterPressed;
    }

    private void OnDisable()
    {
        //KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public void ShowView()
    {
        mode = ViewMode.ADD;

        block = false;

        input_invoiceNumber.text = "";
        input_costPrice.text = "";
        input_initialQuantity.text = "";
        input_totalAmount.text = "";
        input_batchNumber.text = "";
        input_notes.text = "";
        totalPurchaseAmount = 0;
        input_notes.text = "";
        buttonSave.SetActive(true);

        TMP_Dropdown.OptionData defaultOption = dropdown_supplier.options[0];
        dropdown_supplier.options.Clear();
        dropdown_supplier.options.Add(defaultOption);
        dropdown_supplier.value = 0;

        defaultOption = dropdown_company.options[0];
        dropdown_company.options.Clear();
        dropdown_company.options.Add(defaultOption);
        dropdown_company.value = 0;

        text_selectedProductName.text = "";

        selectedSupplier = null;
        text_totalPurchaseAmount.text = totalPurchaseAmount.ToString() + Constants.Currency;

        input_totalAmount.interactable = false;
        addedproductstocks = new List<ProductStock>();
        droplist_products.gameObject.SetActive(true);

        dropdown_company.interactable = true;
        dropdown_supplier.interactable = true;
        dropdown_purchaseType.interactable = true;
        input_invoiceNumber.interactable = true;
        input_notes.interactable = true;
        dropdown_purchaseType.interactable = true;
        datepicker_invoiceDate.enabled = true;

        buttonAdd.SetActive(true);

        input_costPrice.interactable = true;
        input_initialQuantity.interactable = true;
        input_totalAmount.interactable = true;
        input_batchNumber.interactable = true;
        datepicker_expiryDate.enabled = true;
        input_notes.interactable = true;

        text_title.text = Constants.Add + " " + Constants.Purchase;

        ClearContentPurchaseProductStocks();


        GetData();
    }

    public void ShowView(int purchaseId, bool viewOnly = false)
    {
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            text_title.text = Constants.View + " " + Constants.Purchase;

            this.purchaseId = purchaseId;

            dropdown_company.interactable = false;
            dropdown_supplier.interactable = false;
            dropdown_purchaseType.interactable = false;
            input_invoiceNumber.interactable = false;
            input_notes.interactable = false;
            dropdown_purchaseType.interactable = false;
            datepicker_invoiceDate.enabled = false;
            buttonAdd.SetActive(false);

            input_costPrice.interactable = false;
            input_initialQuantity.interactable = false;
            input_totalAmount.interactable = false;
            input_batchNumber.interactable = false;
            datepicker_expiryDate.enabled = false;
            input_notes.interactable = false;
            input_purchaseNotes.interactable = false;
            droplist_products.enabled = false;


            buttonSave.SetActive(false);
        }
        else
        {
            mode = ViewMode.EDIT;
            droplist_products.enabled = false;
        }

        GetAndDisplayPurchase();
    }

    void GetAndDisplayPurchase()
    {
        if (mode == ViewMode.VIEW)
        {
            ProductsManager.Instance.GetProductStocksByPurchaseId(this.purchaseId, (res) => {
                this.addedproductstocks = res.data;
                PurchasesManager.Instance.GetPurchase(this.purchaseId, (res) => {

                    this.purchase = res.data;
                    this.selectedCompany = res.data.company;

                    dropdown_company.captionText.text = res.data.company.name;
                    dropdown_supplier.captionText.text = res.data.contact.name;
                    input_invoiceNumber.text = res.data.invoiceNumber;
                    input_notes.text = res.data.notes;
                    dropdown_purchaseType.captionText.text = res.data.purchaseType.ToString();
                    datepicker_invoiceDate.SelectedDate = res.data.invoiceDate;

                    PopulateAddedStocks();

                }, null);
            }, null);
        }
    }

    void GetData()
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies((response) => {
            companies = response.data;
            
            ProductsManager.Instance.GetProducts(
            (response) =>
            {
                products = response.data;
                productsFiltered = products;

                ContactsManager.Instance.GetAllSuppliers(
                    (response) =>
                    {
                        suppliers = response.data;

                        List<string> companyNames = new List<string>();
                        List<string> supplierNames = new List<string>();
                        foreach (Company company in companies)
                            companyNames.Add(company.name);
                        foreach (Contact supplier in suppliers)
                            supplierNames.Add(supplier.name);

                        ProductsDropList();

                        dropdown_company.AddOptions(companyNames);
                        dropdown_company.onValueChanged.AddListener((changedValue) => {
                            selectedCompany = companies.Find(p => p.name == dropdown_company.options[changedValue].text);
                            if (selectedCompany != null)
                                productsFiltered = products.FindAll(p => p.company.name == selectedCompany.name);
                            else
                                productsFiltered = products;

                            droplist_products.PopulateItems();
                        });

                        dropdown_supplier.AddOptions(supplierNames);
                        dropdown_supplier.onValueChanged.AddListener((changedValue) => {
                            selectedSupplier = suppliers.Find(p => p.name == dropdown_supplier.options[changedValue].text);
                        });

                        dropdown_purchaseType.value = 0;

                        Preloader.Instance.HideFull();
                    },
                    (response) =>
                    {
                        GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                    }
                    );
            },
            (response) =>
            {
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            }
            );

        }, null);
    }

    void ProductsDropList()
    {
        droplist_products.onPopulateItems.AddListener(() =>
        {
            droplist_products.ClearContentDroplist();
            foreach (Product product in productsFiltered.FindAll(p => p.IsEnabledOnGrid))
            {
                GameObject item = droplist_products.GetPooledItem();
                if (item != null)
                {
                    item.transform.Find("Text_Name").GetComponent<TMP_Text>().text = product.name;
                    item.transform.Find("Text_Company").GetComponent<TMP_Text>().text = product.company.name;
                    item.transform.Find("Text_Label_SalePrice").GetComponent<TMP_Text>().text = product.salePrice + Constants.Currency;
                    item.transform.Find("Text_Label_AvailableStock").GetComponent<TMP_Text>().text = product.currentStock + " " + product.unit.name;

                    item.GetComponent<MRButton>().onClicked.RemoveAllListeners();
                    item.GetComponent<MRButton>().onClicked.AddListener(() =>
                    {
                        SelectProduct(product);
                    });

                    item.SetActive(true);
                }
                else
                {
                    GUIManager.Instance.ShowToast(Constants.Error, Constants.PoolError, false);
                }
            }
        });

        droplist_products.onInputEndEdit.AddListener((string inputFieldText) =>
        {
            foreach (Product product in products) product.IsEnabledOnGrid = true;
            foreach (Product filtered in products.FindAll(p => !p.name.ToLower().Contains(inputFieldText)))
                filtered.IsEnabledOnGrid = false;

            droplist_products.PopulateItems();
        });
    }

    void ClearContentPurchaseProductStocks()
    {
        foreach (Transform t in productStocksContentRoot.transform) Destroy(t.gameObject);
    }

    public void SelectProduct(Product product)
    {
        selectedProduct = product;
        text_selectedProductName.text = selectedProduct.name;
        droplist_products.InputDeSelected();
    }

    public void SupplierSelected()
    {
        selectedSupplier = suppliers.Find(p => p.name == dropdown_supplier.options[dropdown_supplier.value].text);
    }

    public void Button_AddClicked()
    {
        if (selectedProduct == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectProduct, false);
            return;
        }
        if (string.IsNullOrEmpty(input_costPrice.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.CostPriceEmpty, false);
            return;
        }
        if (string.IsNullOrEmpty(input_initialQuantity.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.InitialQuantityEmpty, false);
            return;
        }
        if (string.IsNullOrEmpty(input_batchNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.BatchNumberEmpty, false);
            return;
        }
        if ( datepicker_expiryDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ExpiryDateEmpty, false);
            return;
        }
        
        ProductStock productStock = new ProductStock();
        productStock.batchNumber = input_batchNumber.text;
        productStock.costPrice = float.Parse(input_costPrice.text);
        productStock.initialQuantity = int.Parse(input_initialQuantity.text);
        productStock.quantity = int.Parse(input_initialQuantity.text);
        productStock.notes = input_notes.text;
        productStock.productId = selectedProduct.id;
        productStock.product = selectedProduct;

        addedproductstocks.Add(productStock);
        PopulateAddedStocks();

        input_costPrice.text = "";
        input_initialQuantity.text = "";
        input_totalAmount.text = "";
        input_batchNumber.text = "";
        input_notes.text = "";

        text_selectedProductName.text = "";
        selectedProduct = null;
    }

    void PopulateAddedStocks()
    {
        ClearContentPurchaseProductStocks();
        float totalStockQuantity = 0.00f;
        totalPurchaseAmount = 0.00f;
        foreach (ProductStock productStock in addedproductstocks.FindAll(p => p.IsEnabledOnGrid))
        {
            GameObject item = GameObject.Instantiate(productStocksItem, productStocksContentRoot.transform);
            item.transform.parent = productStocksContentRoot.transform;
            item.transform.localScale = Vector3.one;

            item.transform.Find("productName").GetComponent<TMP_Text>().text = productStock.product.name;
            item.transform.Find("batchNumber").GetComponent<TMP_Text>().text = productStock.batchNumber;
            item.transform.Find("costPrice").GetComponent<TMP_Text>().text = productStock.costPrice + Constants.Currency;
            item.transform.Find("notes").GetComponent<TMP_Text>().text = productStock.notes.ToString();

            if (mode == ViewMode.VIEW)
            {
                item.transform.Find("quantity").GetComponent<TMP_Text>().text = productStock.initialQuantity + " " + productStock.product.unit.name;
                item.transform.Find("totalCost").GetComponent<TMP_Text>().text = (productStock.initialQuantity * productStock.costPrice) + Constants.Currency;
            }
            else
            {
                item.transform.Find("quantity").GetComponent<TMP_Text>().text = productStock.quantity + " " + productStock.product.unit.name;
                item.transform.Find("totalCost").GetComponent<TMP_Text>().text = (productStock.quantity * productStock.costPrice) + Constants.Currency;
            }

            if (mode == ViewMode.VIEW) {
                item.transform.Find("Button_Delete").gameObject.SetActive(false);
                item.transform.Find("Button_ReturnStock").gameObject.SetActive(true);

                item.transform.Find("Button_ReturnStock").GetComponent<MRButton>().onClicked.AddListener(() =>
                {
                    ReturnStock(productStock);
                });
            }
            else
            {
                item.transform.Find("Button_ReturnStock").gameObject.SetActive(false);
                item.transform.Find("Button_Delete").gameObject.SetActive(true);

                item.transform.Find("Button_Delete").GetComponent<MRButton>().onClicked.AddListener(() =>
                {
                    addedproductstocks.Remove(productStock);
                    PopulateAddedStocks();
                });
            }

            if (mode == ViewMode.VIEW)
            {
                totalStockQuantity += productStock.initialQuantity;
                totalPurchaseAmount += (productStock.costPrice * productStock.initialQuantity);
            }
            else
            {
                totalStockQuantity += productStock.quantity;
                totalPurchaseAmount += (productStock.costPrice * productStock.quantity);
            }
        }

        text_totalPurchaseAmount.text = totalPurchaseAmount + Constants.Currency;
    }

    void ReturnStock(ProductStock productStock)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.ProductStock_Return);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ProductStock_Return>().ShowView(this.purchase, this.selectedCompany, productStock);
    }

    public bool block = false;
    public void Button_SaveClicked()
    {
        if (selectedCompany == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectCompanyForPurchase, false);
            return;
        }

        if (selectedSupplier == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectSupplier, false);
            return;
        }

        if (string.IsNullOrEmpty(input_invoiceNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.InvoiceNumberEmpty, false);
            return;
        }
        if ( datepicker_invoiceDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.InvoiceDateEmpty, false);
            return;
        }
        if (totalPurchaseAmount <= 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.TotalPurchaseAmountZero, false);
            return;
        }

        if (block) return;
        block = true;

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            Purchase purchase = new Purchase();
            purchase.companyId = selectedCompany.id;
            purchase.contactId = selectedSupplier.id;
            purchase.invoiceNumber = input_invoiceNumber.text;
            purchase.purchaseType = (PurchaseType) dropdown_purchaseType.value;
            purchase.invoiceDate = datepicker_invoiceDate.SelectedDate;
            input_purchaseNotes.interactable = true;
            purchase.notes = input_purchaseNotes.text;
            purchase.totalAmount = totalPurchaseAmount;
            purchase.purchasedproductstocks = addedproductstocks;
            droplist_products.enabled = true;

            PurchasesManager.Instance.AddPurchase(purchase,
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.PurchaseAdded);
                    if (PurchasesManager.onPurchaseAdded != null) PurchasesManager.onPurchaseAdded();
                    GUIManager.Instance.Back();
                },
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
        }
    }

    public void CalculateTotalAmount()
    {
        if (string.IsNullOrEmpty(input_costPrice.text) || string.IsNullOrEmpty(input_initialQuantity.text))
        {
            input_totalAmount.text = "0" + Constants.Currency;
            return;
        }

        float totalAmount = (float.Parse(input_costPrice.text) * float.Parse(input_initialQuantity.text));
        input_totalAmount.text = totalAmount + Constants.Currency;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
