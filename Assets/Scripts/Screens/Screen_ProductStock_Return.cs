using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_ProductStock_Return : MonoBehaviour
{
    public TMP_InputField input_returnQuantity, input_details;
    public TMP_Text text_companyName, text_productName, text_invoiceNumber,
        text_lotNumber, text_batchNumber, text_expiryDate, text_currentStock, text_purchaseId;

    public MRDatePicker datepicker_returnDate;
    float quantityRemining = 0;

    Purchase purchase;
    Company company;
    ProductStock productStock;

    private void OnEnable()
    {
        input_returnQuantity.text = "";
        input_details.text = "";
        quantityRemining = 0;
        block = false;
    }

    public void ShowView(Purchase purchase, Company company, ProductStock productStock)
    {
        this.purchase = purchase;
        this.company = company;
        this.productStock = productStock;

        text_companyName.text = company.name;
        text_productName.text = productStock.product.name;
        text_invoiceNumber.text = purchase.invoiceNumber;
        text_lotNumber.text = productStock.lotNumber.ToString();
        text_batchNumber.text = productStock.batchNumber.ToString();
        text_expiryDate.text = productStock.expiryDate.ToDateString();
        text_currentStock.text = productStock.quantity.ToString() + " " + productStock.product.unit.name;
        text_purchaseId.text = purchase.id.ToString();

        this.quantityRemining = productStock.quantity;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    bool block = false;
    public void Button_SaveClicked()
    {
        if (string.IsNullOrEmpty(input_returnQuantity.text) || float.Parse(input_returnQuantity.text) <= 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.EnterReturnQuantity, false);
            return;
        }

        if (float.Parse(input_returnQuantity.text) > this.quantityRemining)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ReturnQuantityLarger, false);
            return;
        }

        if ( datepicker_returnDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
            return;
        }

        if (block) return;
        block = true;

        Preloader.Instance.ShowFull();

        ReturnProductStockParams stockReturn = new ReturnProductStockParams();
        stockReturn.productStockId = this.productStock.id;
        stockReturn.companyAccountId = this.company.accountId;
        stockReturn.returnQuantity = float.Parse(input_returnQuantity.text);
        stockReturn.details = input_details.text;
        stockReturn.returnDate = datepicker_returnDate.SelectedDate;
        stockReturn.invoiceNumber = this.purchase.invoiceNumber;

        ProductsManager.Instance.ReturnProductStock(stockReturn, (response) =>
        {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Success, Constants.StockReturned);
            GUIManager.Instance.Back();
        }, null);
    }
}
