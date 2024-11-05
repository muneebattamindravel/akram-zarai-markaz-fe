using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_Sale_View_Add : MonoBehaviour
{
    //Variables
    List<Product> products = new List<Product>();
    List<Contact> customers;
    List<SaleItem> saleItems;
    Contact selectedContact = null;
    ViewMode mode;
    List<Account> onlineAccounts;
    List<string> onlineAccountsStrings;

    //UI
    public MRDroplist droplist_products, droplist_customers;
    public GameObject saleItemsContentRoot;
    public GameObject saleItemPrefab, isReturnObject;
    public TMP_Text text_totalItems, text_totalSaleAmount, text_netSaleAmount;

    //public DatePicker datepicker_saleDate;
    public MRDatePicker datepicker_saleDate;

    public TMP_InputField input_bookNumber, input_billNumber, input_discount, input_paymentReceived;
    public TMP_Text text_title;
    public GameObject buttonSale, buttonSadSale;
    public TMP_Dropdown dropdown_paymentType, dropdown_onlineAccount;
    public GameObject selectedCustomerPrefab, quickSelection;
    GameObject selectedCustomerObject;
    public GameObject viewSalePaymentsButton;
    public TMP_Text text_label_productsCount;
    public GameObject text_productsCount;
    public GameObject buttonReturnSaleItems;
    public TMP_InputField input_notes;

    int saleId;
    Sale sale;

    float discount = 0f;
    float netSaleAmount = 0f;
    float paymentReceived = 0f;

    private void OnEnable()
    {
        saleLock = false;

        isReturnObject.SetActive(false);

        input_billNumber.text = "";
        input_bookNumber.text = "";
        input_discount.text = "";
        input_notes.text = "";
        input_paymentReceived.text = "";
        discount = 0f;
        paymentReceived = 0f;
        netSaleAmount = 0f;
        buttonSale.SetActive(true);
        buttonSadSale.SetActive(false);
        dropdown_onlineAccount.gameObject.SetActive(false);
        dropdown_paymentType.value = 0;
        viewSalePaymentsButton.SetActive(false);
        dropdown_paymentType.gameObject.SetActive(true);
        dropdown_paymentType.enabled = true;

        input_discount.enabled = true;
        input_bookNumber.enabled = true;
        input_billNumber.enabled = true;
        input_paymentReceived.enabled = true;
        datepicker_saleDate.enabled = true;
        droplist_customers.gameObject.SetActive(true);
        droplist_products.gameObject.SetActive(true);

        buttonReturnSaleItems.SetActive(false);

        if (selectedCustomerObject != null)
            Destroy(selectedCustomerObject);
    }

    public void ShowView()
    {
        isReturnObject.SetActive(false);

        mode = ViewMode.ADD;
        text_title.text = Constants.Add + " " + Constants.NewSale;

        text_productsCount.gameObject.SetActive(true);
        text_label_productsCount.gameObject.SetActive(true);

        viewTitleContainer.SetActive(false);

        Initialize();
        GetData();
        ClearSaleItemsContentRoot();
    }

    public TMP_Text text_saleId;
    public GameObject viewTitleContainer;

    public void ShowView(int saleId, bool viewOnly = false)
    {
        isReturnObject.SetActive(false);

        viewTitleContainer.SetActive(true);
        text_saleId.text = saleId.ToString();

        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            text_title.text = Constants.View + " " + Constants.ViewSale;

            text_productsCount.gameObject.SetActive(false);
            text_label_productsCount.gameObject.SetActive(false);

            buttonReturnSaleItems.SetActive(true);
            input_notes.interactable = false;
        }
        else
        {
            mode = ViewMode.EDIT;
            text_title.text = Constants.Edit + " " + Constants.ViewSale;

            text_productsCount.gameObject.SetActive(false);
            text_label_productsCount.gameObject.SetActive(false);
            input_notes.interactable = true;

            buttonReturnSaleItems.SetActive(true);
        }

        this.saleId = saleId;

        GetSaleDetails();
    }

    void GetSaleDetails()
    {
        SalesManager.Instance.GetSale(saleId,
            (response) => {
                sale = response.data;

                isReturnObject.SetActive(this.sale.returnApplied);

                PopulateViewSaleDetails();
            },
            (response) => {
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
            });
    }

    void PopulateViewSaleDetails()
    {
        ClearSaleItemsContentRoot();

        netSaleAmount = sale.totalAmount;
        paymentReceived = sale.receivedAmount;
        input_paymentReceived.text = paymentReceived.ToString();
        input_paymentReceived.enabled = false;

        datepicker_saleDate.SelectedDate = sale.saleDate;
        datepicker_saleDate.enabled = false;
        droplist_customers.gameObject.SetActive(false);
        droplist_products.gameObject.SetActive(false);

        if (sale.contact != null)
        {
            selectedCustomerObject = GameObject.Instantiate(selectedCustomerPrefab, quickSelection.transform);
            selectedCustomerObject.transform.SetAsFirstSibling();
            selectedCustomerObject.name = sale.contact.name;
            selectedCustomerObject.transform.Find("Text_Name").GetComponent<TMP_Text>().text = sale.contact.name;
            selectedCustomerObject.transform.Find("Button_Delete").gameObject.SetActive(false);
        }

        text_totalItems.text = sale.saleitems.Count.ToString();
        text_totalSaleAmount.text = sale.totalAmount.ToString();
        input_notes.text = sale.notes;
        input_discount.text = sale.discount.ToString();
        input_discount.enabled = false;
        text_netSaleAmount.text = (sale.totalAmount - sale.discount) + Constants.Currency;
        input_bookNumber.text = sale.bookNumber;
        input_billNumber.text = sale.billNumber;
        input_bookNumber.enabled = false;
        input_billNumber.enabled = false;
        dropdown_paymentType.gameObject.SetActive(false);
        dropdown_onlineAccount.gameObject.SetActive(false);

        buttonSale.SetActive(false);
        buttonSadSale.SetActive(false);
        dropdown_paymentType.enabled = false;
        viewSalePaymentsButton.SetActive(true);

        foreach (SaleItem item in sale.saleitems)
        {
            GameObject saleItem = GameObject.Instantiate(saleItemPrefab, saleItemsContentRoot.transform);
            saleItem.transform.Find("Product").transform.Find("productName").GetComponent<TMP_Text>().text = item.product.name;
            saleItem.transform.Find("Product").transform.Find("Text_StockAvailable").gameObject.SetActive(false);
            saleItem.transform.Find("Product").transform.Find("stockAvailable").gameObject.SetActive(false);

            saleItem.transform.Find("Quantity").transform.Find("Button_Plus").gameObject.SetActive(false);
            saleItem.transform.Find("Quantity").transform.Find("Button_Minus").gameObject.SetActive(false);
            saleItem.transform.Find("Quantity").transform.Find("InputField - Quantity").GetComponent<TMP_InputField>().text = item.quantity.ToString();
            saleItem.transform.Find("Quantity").transform.Find("InputField - Quantity").GetComponent<TMP_InputField>().interactable = false;

            saleItem.transform.Find("Price").transform.Find("InputField - Price").GetComponent<TMP_InputField>().text = item.salePrice.ToString();
            saleItem.transform.Find("Price").transform.Find("InputField - Price").GetComponent<TMP_InputField>().interactable = false;

            saleItem.transform.Find("SubTotal").transform.Find("subTotal").GetComponent<TMP_Text>().text = (item.salePrice * item.quantity) + Constants.Currency;
            saleItem.transform.Find("Button_Delete").gameObject.SetActive(false);

            saleItem.transform.Find("Button_Return").gameObject.SetActive(false);
            saleItem.transform.Find("Button_NotReturn").gameObject.SetActive(false);

            ImageUtils.Instance.LoadImage(saleItem.transform.Find("Product").transform.Find("Image_ProductImage").GetComponent<RawImage>(), Application.persistentDataPath + "/" + item.product.imageURL);
        }
    }

    void Initialize()
    {
        //variables
        saleItems = new List<SaleItem>();
        selectedContact = null;

        //ui
        datepicker_saleDate.SelectedDate = DateTime.Now;

        UpdateSaleUI();
        ProductsDropList();
        CustomersDropList();
    }

    void UpdateSaleUI()
    {
        if (mode == ViewMode.EDIT)
            return;

        text_totalItems.text = saleItems.Count.ToString();
        text_totalSaleAmount.text = CalculateTotalSaleAmount() + Constants.Currency;

        netSaleAmount = CalculateTotalSaleAmount() - discount;
        text_netSaleAmount.text = netSaleAmount + Constants.Currency;
    }

    public void OnDiscountValueChanged()
    {
        if (mode == ViewMode.EDIT)
            return;

        discount = 0f;
        if (!string.IsNullOrEmpty(input_discount.text))
        {
            discount = float.Parse(input_discount.text, CultureInfo.InvariantCulture.NumberFormat);
            if (discount > CalculateTotalSaleAmount())
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.DiscoutGreater, false);
                discount = 0f;
                input_discount.text = "";
            }
        }

        netSaleAmount = CalculateTotalSaleAmount() - discount;
        UpdateSaleUI();
    }

    public void OnPaymentReceivedValueChanged()
    {
        buttonSale.SetActive(true);
        buttonSadSale.SetActive(false);

        if (!string.IsNullOrEmpty(input_paymentReceived.text))
        {
            paymentReceived = float.Parse(input_paymentReceived.text, CultureInfo.InvariantCulture.NumberFormat);

            if (paymentReceived < netSaleAmount)
                buttonSadSale.SetActive(true);
            else if (paymentReceived > netSaleAmount)
            {
                input_paymentReceived.text = "";
                paymentReceived = 0;
                GUIManager.Instance.ShowToast(Constants.Failed, Constants.ReceivedAmountGreater, false);
            }
        }
    }

    float CalculateTotalSaleAmount()
    {
        if (mode == ViewMode.EDIT)
            return 0;

        float totalAmount = 0.00f;
        foreach (SaleItem saleItem in saleItems)
        {
            totalAmount += (saleItem.quantity * saleItem.salePrice);
        }
        return totalAmount;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    void GetData()
    {
        

        Preloader.Instance.ShowFull();
        ContactsManager.Instance.GetAllCustomers(
                    (response) =>
                    {
                        customers = response.data;
                        List<string> customerNames = new List<string>();
                        customerNames.Add(Constants.None);
                        foreach (Contact contact in customers) customerNames.Add(contact.name);

                        AccountsManager.Instance.GetAccounts(
                            (response) =>
                            {
                                onlineAccounts = response.data;
                                onlineAccounts = onlineAccounts.FindAll(p => p.type == AccountType.Online.ToString());
                                onlineAccountsStrings = new List<string>();
                                foreach (Account account in onlineAccounts) onlineAccountsStrings.Add(account.name + "\n" + account.bankName + " - " + account.bankAccountNumber);
                                dropdown_onlineAccount.ClearOptions();
                                dropdown_onlineAccount.AddOptions(onlineAccountsStrings);
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
    }

    public void OnPaymentTypeChanged()
    {
        if (dropdown_paymentType.value == 0)
            dropdown_onlineAccount.gameObject.SetActive(false);
        else
            dropdown_onlineAccount.gameObject.SetActive(true);
    }

    void ClearSaleItemsContentRoot()
    {
        foreach (Transform t in saleItemsContentRoot.transform) Destroy(t.gameObject);
    }

    public void SelectCustomer(Contact customer)
    {
        selectedContact = customer;
        if (selectedCustomerObject != null)
            Destroy(selectedCustomerObject);

        selectedCustomerObject = GameObject.Instantiate(selectedCustomerPrefab, quickSelection.transform);
        selectedCustomerObject.transform.SetAsFirstSibling();
        selectedCustomerObject.name = customer.name;
        selectedCustomerObject.transform.Find("Text_Name").GetComponent<TMP_Text>().text = selectedContact.name;
        selectedCustomerObject.transform.Find("Button_Delete").GetComponent<Button>().onClick.AddListener(() => {
            selectedContact = null;
            Destroy(selectedCustomerObject);
        });

        droplist_customers.InputDeSelected();
    }

    public void SelectProduct(Product product, SaleItem item = null)
    {
        SaleItem saleItem = new SaleItem();
        saleItem.product = product;
        products.Remove(product);

        GameObject saleItemObject = Instantiate(saleItemPrefab, saleItemsContentRoot.transform);

        ImageUtils.Instance.LoadImage(saleItemObject.transform.Find("Product").transform.Find("Image_ProductImage").GetComponent<RawImage>(), Application.persistentDataPath + "/" + product.imageURL);
        TMP_InputField input_quantity = saleItemObject.transform.Find("Quantity").transform.Find("InputField - Quantity").GetComponent<TMP_InputField>();

        if (product.unit.allowDecimal)
            input_quantity.contentType = TMP_InputField.ContentType.DecimalNumber;
        else
            input_quantity.contentType = TMP_InputField.ContentType.IntegerNumber;


        saleItemObject.transform.Find("Product").transform.Find("productName").GetComponent<TMP_Text>().text = product.name;
        UpdateStockAvailableAmount(saleItemObject, product, saleItem);

        if (item != null)
            saleItem.quantity = item.quantity;
        else
            saleItem.quantity = 1.00f;

        input_quantity.text = saleItem.quantity.ToString();

        UpdateSubTotal(saleItemObject, product, saleItem);

        saleItemObject.transform.Find("Quantity").transform.Find("Button_Minus").GetComponent<MRButton>().onClicked.AddListener(() => {
            if (saleItem.quantity - 1.00f < 0.00f)
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.QuantityLessThanZero, false);
                return;
            }
            saleItem.quantity -= 1.00f;
            UpdateStockAvailableAmount(saleItemObject, product, saleItem);
            input_quantity.text = saleItem.quantity.ToString();
            UpdateSubTotal(saleItemObject, product, saleItem);
            UpdateSaleUI();
        });

        saleItemObject.transform.Find("Quantity").transform.Find("Button_Plus").GetComponent<MRButton>().onClicked.AddListener(() => {
            if (saleItem.quantity + 1.00f > product.currentStock)
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughStock, false);
                return;
            }
            saleItem.quantity += 1.00f;
            UpdateStockAvailableAmount(saleItemObject, product, saleItem);
            input_quantity.text = saleItem.quantity.ToString();
            UpdateSubTotal(saleItemObject, product, saleItem);
            UpdateSaleUI();
        });

        input_quantity.onEndEdit.AddListener((value) => {

            if (string.IsNullOrEmpty(value) || float.TryParse(value, out saleItem.quantity) == false)
            {
                if (saleItem.product.unit.allowDecimal)
                    saleItem.quantity = 1.00f;
                else
                    saleItem.quantity = 1f;

                GUIManager.Instance.ShowToast(Constants.Error, Constants.QuantityLessThanZero, false);
                UpdateStockAvailableAmount(saleItemObject, product, saleItem);
            }
            else
            {
                if (saleItem.quantity <= 0)
                {
                    if (saleItem.product.unit.allowDecimal)
                        saleItem.quantity = 1.00f;
                    else
                        saleItem.quantity = 1f;

                    GUIManager.Instance.ShowToast(Constants.Error, Constants.QuantityLessThanZero, false);
                }
                else if (saleItem.quantity > product.currentStock)
                {
                    saleItem.quantity = product.currentStock;
                    GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughStock, false);
                }
                else
                {
                    saleItem.quantity = float.Parse(input_quantity.text, CultureInfo.InvariantCulture.NumberFormat);
                }

                UpdateStockAvailableAmount(saleItemObject, product, saleItem);
            }

            input_quantity.text = saleItem.quantity.ToString();
            UpdateSubTotal(saleItemObject, product, saleItem);
            UpdateSaleUI();
        });


        TMP_InputField input_salePrice = saleItemObject.transform.Find("Price").transform.Find("InputField - Price").GetComponent<TMP_InputField>();
        input_salePrice.contentType = TMP_InputField.ContentType.DecimalNumber;

        if (product.salePrice <= 0.00f)
            saleItem.salePrice = 1.00f;
        else
            saleItem.salePrice = product.salePrice;

        if (item != null)
            saleItem.salePrice = item.salePrice;

        input_salePrice.text = saleItem.salePrice.ToString();
        UpdateSubTotal(saleItemObject, product, saleItem);

        input_salePrice.onEndEdit.AddListener((value) => {
            if (string.IsNullOrEmpty(value))
            {
                saleItem.salePrice = 1.00f;
                GUIManager.Instance.ShowToast(Constants.Error, Constants.SalePriceWrong, false);
            }
            if (float.TryParse(value, out saleItem.salePrice) == false)
            {
                saleItem.salePrice = 1.00f;
                GUIManager.Instance.ShowToast(Constants.Error, Constants.SalePriceWrong, false);
            }
            else if (saleItem.salePrice <= 0.00f)
            {
                saleItem.salePrice = 1.00f;
                GUIManager.Instance.ShowToast(Constants.Error, Constants.SalePriceWrong, false);
            }
            input_salePrice.text = saleItem.salePrice.ToString();
            UpdateSubTotal(saleItemObject, product, saleItem);
            UpdateSaleUI();
        });

        saleItemObject.transform.Find("Button_Delete").GetComponent<MRButton>().onClicked.AddListener(() => {
            saleItems.Remove(saleItem);

            //products.Add(product);
            //products = products.OrderBy(p => p.id).ToList();

            Destroy(saleItemObject);
            UpdateSubTotal(saleItemObject, product, saleItem);
            UpdateSaleUI();
        });

        saleItemObject.transform.Find("Button_Return").gameObject.SetActive(false);
        saleItemObject.transform.Find("Button_NotReturn").gameObject.SetActive(false);

        saleItems.Add(saleItem);
        droplist_products.InputDeSelected();
        UpdateSaleUI();
    }

    void UpdateStockAvailableAmount(GameObject saleItemObject, Product product, SaleItem saleItem)
    {
        saleItemObject.transform.Find("Product").transform.Find("stockAvailable").GetComponent<TMP_Text>().text = (product.currentStock - saleItem.quantity) + " " + product.unit.name;
    }

    void UpdateSubTotal(GameObject saleItemObject, Product product, SaleItem saleItem)
    {
        saleItemObject.transform.Find("SubTotal").transform.Find("subTotal").GetComponent<TMP_Text>().text = (saleItem.salePrice * saleItem.quantity) + Constants.Currency;
    }

    void ProductsDropList()
    {
        droplist_products.onInputEndEdit.RemoveAllListeners();
        droplist_products.onInputEndEdit.AddListener((string inputFieldText) =>
        {
            if (string.IsNullOrEmpty(inputFieldText))
            {
                products.Clear();
                droplist_products.ClearContentDroplist();
                return;
            }

            Preloader.Instance.ShowFull();
            ProductsManager.Instance.GetProducts(
                inputFieldText,
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    products = response.data;

                    foreach (SaleItem saleItem in saleItems)
                        products.RemoveAll(p => p.name == saleItem.product.name);

                    text_label_productsCount.text = this.products.Count.ToString();

                    droplist_products.PopulateItems();
                },
                (response) =>
                {
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                }
            );
        });

        droplist_products.onPopulateItems.RemoveAllListeners();
        droplist_products.onPopulateItems.AddListener(() => {
            droplist_products.ClearContentDroplist();
            foreach (Product product in products.FindAll(p => p.IsEnabledOnGrid))
            {
                GameObject item = droplist_products.GetPooledItem();
                if (item != null)
                {
                    item.name = product.name.ToLower();

                    item.transform.Find("Text_Name").GetComponent<TMP_Text>().text = product.name;
                    item.transform.Find("Text_Company").GetComponent<TMP_Text>().text = product.company.name;
                    item.transform.Find("Text_Label_SalePrice").GetComponent<TMP_Text>().text = product.salePrice + Constants.Currency;
                    item.transform.Find("Text_Label_AvailableStock").GetComponent<TMP_Text>().text = product.currentStock + " " + product.unit.name;

                    if (product.currentStock > 0.00)
                    {
                        item.transform.Find("Text_Label_AvailableStock").GetComponent<TMP_Text>().color = Constants.PositiveColor;
                        item.GetComponent<MRButton>().onClicked.RemoveAllListeners();
                        item.GetComponent<MRButton>().onClicked.AddListener(() =>
                        {
                            SelectProduct(product);
                        });
                        item.GetComponent<MRButton>().interactible = true;
                        item.GetComponent<CanvasGroup>().alpha = 1f;
                    }
                    else
                    {
                        item.transform.Find("Text_Label_AvailableStock").GetComponent<TMP_Text>().color = Constants.NegativeColor;
                        item.GetComponent<MRButton>().interactible = false;
                        item.GetComponent<CanvasGroup>().alpha = 0.5f;
                    }

                    item.SetActive(true);
                }
                else
                {
                    GUIManager.Instance.ShowToast(Constants.Error, Constants.PoolError, false);
                }
            }
        });

        droplist_products.onDroplistClosed.RemoveAllListeners();
        droplist_products.onDroplistClosed.AddListener(() => {
            products.Clear();
            text_label_productsCount.text = products.Count().ToString();
        });

    }

    void CustomersDropList()
    {
        droplist_customers.onPopulateItems.AddListener(() =>
        {
            droplist_customers.ClearContentDroplist();
            foreach (Contact contact in customers.FindAll(p => p.IsEnabledOnGrid))
            {
                GameObject item = droplist_customers.GetPooledItem();
                if (item != null)
                {
                    item.transform.Find("Text_Name").GetComponent<TMP_Text>().text = contact.name;
                    item.transform.Find("Text_Label_Phone").GetComponent<TMP_Text>().text = contact.number;

                    item.GetComponent<MRButton>().onClicked.RemoveAllListeners();
                    item.GetComponent<MRButton>().onClicked.AddListener(() =>
                    {
                        SelectCustomer(contact);
                    });
                    item.SetActive(true);
                }
                else
                {
                    GUIManager.Instance.ShowToast(Constants.Error, Constants.PoolError, false);
                }
            }
        });

        droplist_customers.onInputEndEdit.AddListener((string inputFieldText) =>
        {
            foreach (Contact customer in customers) customer.IsEnabledOnGrid = true;
            foreach (Contact filtered in customers.FindAll(p => !p.name.ToLower().Contains(inputFieldText)))
                filtered.IsEnabledOnGrid = false;

            droplist_customers.PopulateItems();
        });
    }

    bool saleLock = false;
    public void Button_SaleClicked()
    {
        OnDiscountValueChanged();
        OnPaymentReceivedValueChanged();

        if (datepicker_saleDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
            return;
        }

        if (saleItems.Count == 0 || CalculateTotalSaleAmount() == 0)
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.TotalAmountZero, false);
            return;
        }

        if (string.IsNullOrEmpty(input_bookNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterBookNumber, false);
            return;
        }

        if (string.IsNullOrEmpty(input_billNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterBillNumber, false);
            return;
        }

        if (string.IsNullOrEmpty(input_paymentReceived.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterPaymentReceived, false);
            return;
        }

        if ((paymentReceived < netSaleAmount) && selectedContact == null)
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.MustSelectCustomerCreditSale, false);
            return;
        }

        if (dropdown_paymentType.value == 1 && selectedContact == null)
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.MustSelectCustomerOnlineAccount, false);
            return;
        }

        if (dropdown_paymentType.value == 1 && onlineAccountsStrings.Count <= 0)
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.MustSelectOnlineAccount, false);
            return;
        }

        if (saleLock) return;
        saleLock = true;

        //Applying Discount on all items
        if (discount > 0)
        {
            float totalQuantity = 0;
            foreach (SaleItem si in saleItems)
                totalQuantity += si.quantity;

            float minusAmount = discount / totalQuantity;
            foreach (SaleItem si in saleItems)
                si.salePrice -= minusAmount;
        }

        Sale sale = new Sale();
        sale.returnApplied = false;
        sale.totalAmount = CalculateTotalSaleAmount();

        if (!string.IsNullOrEmpty(input_discount.text))
            sale.discount = float.Parse(input_discount.text, CultureInfo.InvariantCulture.NumberFormat);
        else
            sale.discount = 0;
        if (datepicker_saleDate.SelectedDate == DateTime.MinValue)
            sale.saleDate = DateTime.Now;
        else
            sale.saleDate = datepicker_saleDate.SelectedDate;
        sale.bookNumber = input_bookNumber.text;
        sale.billNumber = input_billNumber.text;
        if (selectedContact != null)
            sale.contactId = selectedContact.id;
        else
            sale.contactId = 0;
        sale.saleitems = saleItems;

        sale.notes = input_notes.text;

        sale.salepayment = new SalePayment();
        sale.salepayment.receivedAmount = paymentReceived;
        sale.salepayment.receivedDate = sale.saleDate;
        if (dropdown_paymentType.value == 0)
        {
            sale.salepayment.paymentType = PaymentType.Cash;
            sale.salepayment.accountId = 0;
        }
        else
        {
            sale.salepayment.paymentType = PaymentType.Online;
            sale.salepayment.accountId =
                onlineAccounts[dropdown_onlineAccount.options.
                FindIndex(p => p.text == dropdown_onlineAccount.options[dropdown_onlineAccount.value].text)].id;
        }
        SalesManager.Instance.AddSale(sale, (response) =>
        {
            GUIManager.Instance.ShowToast(Constants.Success, Constants.SaleCreated);
            if (SalesManager.onSaleAdded != null) SalesManager.onSaleAdded();
            GUIManager.Instance.Back();
        },
        (response) => {
            GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
        });
    }

    public void Button_ViewSalePaymentsClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.ViewSalePayments);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ViewSalePayments>().ShowView(saleId);
    }

    public void Button_SaleReturnClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Screen_Sales_ReturnItems);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Sales_ReturnItems>().ShowView(this.sale);
    }

    public void Button_ViewSaleReturnsClicked()
    {
        Debug.Log("Clicked here");
    }
}
