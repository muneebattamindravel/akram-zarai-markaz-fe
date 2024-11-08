using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Screen_Sales_ReturnItems : MonoBehaviour
{
    public GameObject saleItemPrefab;
    public GameObject saleItemsContentRoot, returnItemsContentRoot;

    Sale sale;
    public List<SaleItem> saleItems, returnItems;

    public TMP_Text text_saleId, text_customerName, text_totalReturnItems, text_totalReturnAmount;
    public TMP_InputField input_bookNumber, input_billNumber;
    public MRDatePicker datepicker_returnDate;

    void OnEnable() {
        input_billNumber.text = "";
        input_bookNumber.text = "";

        datepicker_returnDate.SelectedDate = DateTime.MinValue;
    }

    public void ShowView(Sale sale)
    {
        this.sale = sale;
        Initialize();
        CopySaleItems();
        Populate();

        text_saleId.text = this.sale.id.ToString();
        if (sale.contact != null)
            text_customerName.text = this.sale.contact.name.ToString();
        else
            text_customerName.text = "-";
    }

    void CopySaleItems()
    {
        foreach(SaleItem item in this.sale.saleitems)
        {
            SaleItem copyItem = new SaleItem();
            copyItem.id = item.id;
            copyItem.product = item.product;
            copyItem.quantity = item.quantity;
            copyItem.salePrice = item.salePrice;
            copyItem.usedLotNumber = item.usedLotNumber;
            copyItem.lotsUsedJson = item.lotsUsedJson;
            copyItem.ConsolidateLotUsedInformation();
            this.saleItems.Add(copyItem);
        }
    }

    void Populate()
    {
        ClearContentRoot();
        PopulateItemsFor(saleItems, false);
        PopulateItemsFor(returnItems, true);

        text_totalReturnItems.text = returnItems.Count.ToString();
        text_totalReturnAmount.text = CalculateTotalReturnAmount() + Constants.Currency;
    }

    float CalculateTotalReturnAmount()
    {
        float totalAmount = 0.00f;
        foreach (SaleItem saleItem in returnItems)
        {
            totalAmount += (saleItem.quantity * saleItem.salePrice);
        }
        return totalAmount;
    }

    float CalculateTotalSaleAmount()
    {
        float totalAmount = 0.00f;
        foreach (SaleItem saleItem in saleItems)
        {
            totalAmount += (saleItem.quantity * saleItem.salePrice);
        }
        return totalAmount;
    }

    void PopulateItemsFor(List<SaleItem> saleItems, bool returnItem)
    {
        foreach (SaleItem item in saleItems)
        {
            if (item.quantity > 0)
            {
                GameObject saleItem;
                if (returnItem)
                    saleItem = GameObject.Instantiate(saleItemPrefab, returnItemsContentRoot.transform);
                else
                    saleItem = GameObject.Instantiate(saleItemPrefab, saleItemsContentRoot.transform);

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

                if (!returnItem)
                {
                    saleItem.transform.Find("Button_Return").gameObject.SetActive(true);
                    saleItem.transform.Find("Button_Return").GetComponent<MRButton>().onClicked.AddListener(() => {
                        AddToReturnItems(item);
                        Populate();
                    });
                }
                else
                {
                    saleItem.transform.Find("Button_NotReturn").gameObject.SetActive(true);
                    saleItem.transform.Find("Button_NotReturn").GetComponent<MRButton>().onClicked.AddListener(() => {
                        RemoveFromReturnItems(item);
                        Populate();
                    });
                }
            }
        }
    }

    void AddToReturnItems(SaleItem item)
    {
        SaleItem returnItem = returnItems.Find(p => p.product.id == item.product.id && p.id == item.id);
        if (returnItem == null)
        {
            SaleItem copyItem = new SaleItem();
            copyItem.id = item.id;
            copyItem.product = item.product;
            copyItem.quantity = 1f;
            copyItem.salePrice = item.salePrice;
            copyItem.usedLotNumber = item.usedLotNumber;
            copyItem.lotsUsedJson = item.lotsUsedJson;
            copyItem.ConsolidateLotUsedInformation();
            returnItems.Add(copyItem);
        }
        else
            returnItem.quantity++;

        SaleItem saleItem = saleItems.Find(p => p.product.id == item.product.id && p.id == item.id);
        if (saleItem != null)
        {
            saleItem.quantity--;
            if (saleItem.quantity < 0)
                saleItems.RemoveAll(p => p.id == item.id && p.product.id == item.id);
        }
    }

    void RemoveFromReturnItems(SaleItem item)
    {
        SaleItem returnItem = returnItems.Find(p => p.product.id == item.product.id && p.id == item.id);
        if (returnItem != null)
        {
            returnItem.quantity--;

            if (returnItem.quantity < 0)
                returnItems.RemoveAll(p => p.id == item.id && p.product.id == item.id);
        }

        SaleItem saleItem = saleItems.Find(p => p.product.id == item.product.id && p.id == item.id);
        if (saleItem == null)
        {
            SaleItem copyItem = new SaleItem();
            copyItem.id = item.id;
            copyItem.product = item.product;
            copyItem.quantity = 1f;
            copyItem.salePrice = item.salePrice;
            copyItem.usedLotNumber = item.usedLotNumber;
            copyItem.lotsUsedJson = item.lotsUsedJson;
            copyItem.ConsolidateLotUsedInformation();
            saleItems.Add(copyItem);
        }
        else
            saleItem.quantity++;
    }

    void Initialize()
    {
        saleItems = new List<SaleItem>();
        returnItems = new List<SaleItem>();
        ClearContentRoot();
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    void ClearContentRoot()
    {
        foreach (Transform t in saleItemsContentRoot.transform) Destroy(t.gameObject);
        foreach (Transform t in returnItemsContentRoot.transform) Destroy(t.gameObject);
    }

    public void Button_ReturnSaleItemsClicked()
    {
        if (datepicker_returnDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
            return;
        }

        if (returnItems.Count <=0 && CalculateTotalReturnAmount() <= 0) {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.NothingToReturn, false);
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

        if (returnItems.Count > 0 && CalculateTotalReturnAmount() > 0)
        {
            GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
            GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
                Constants.ReturnConfirmation,
                Constants.YES, Constants.NO,
                () =>
                {
                    Preloader.Instance.ShowFull();

                    foreach (SaleItem item in returnItems)
                        item.saleId = this.sale.id;


                    SaleReturn saleReturn = new SaleReturn();
                    saleReturn.saleId = this.sale.id;
                    saleReturn.returnItems = this.returnItems;
                    saleReturn.bookNumber = input_bookNumber.text;
                    saleReturn.billNumber = input_billNumber.text;
                    saleReturn.returnDate = datepicker_returnDate.SelectedDate;

                    SalesManager.Instance.ReturnSaleItems(saleReturn, (response) => {
                        GUIManager.Instance.ShowToast(Constants.Success, Constants.SaleItemsReturned);
                        Preloader.Instance.HideFull();
                        GUIManager.Instance.Back();
                        GUIManager.Instance.Back();
                        SalesManager.onSaleAdded?.Invoke();
                    }, 
                    (response) =>
                    {
                        GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                        Preloader.Instance.HideFull();
                    });
                },
                () =>
                {
                    //Cancelled
                }
            );
        }
    }
}
