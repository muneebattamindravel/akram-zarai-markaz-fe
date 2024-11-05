using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_ManualStock_Return : MonoBehaviour
{

    public TMP_InputField input_returnQuantity, input_returnPrice, input_totalAmount;
    public MRDatePicker datepicker_date;
    public TMP_Text text_productName, text_stockAmountRemaining;

    Product product;
    ProductStock productStock;

    public bool block = false;
    int totalReturnAmount = 0;

    private void OnEnable()
    {
        block = false;
        input_returnQuantity.text = "";
        input_returnPrice.text = "";
        totalReturnAmount = 0;
    }

    public void ShowView(Product product, ProductStock productStock)
    {
        input_returnQuantity.text = "";
        input_returnPrice.text = "";
        totalReturnAmount = 0;
        this.product = product;
        this.productStock = productStock;
        text_productName.text = product.name;
        text_stockAmountRemaining.text = productStock.quantity.ToString();
        input_returnPrice.text = productStock.costPrice.ToString();
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    public void OnInputValueChanged()
    {
        if (string.IsNullOrEmpty(input_returnQuantity.text) || string.IsNullOrEmpty(input_returnPrice.text))
        {
            input_totalAmount.text = "";
            return;
        }

        int returnQuantity = int.Parse(input_returnQuantity.text);
        int returnPrice = int.Parse(input_returnPrice.text);

        totalReturnAmount = returnPrice * returnQuantity;
        input_totalAmount.text = totalReturnAmount.ToString();
    }

    public void Button_ReturnClicked()
    {
        if ( datepicker_date.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
            return;
        }

        if (string.IsNullOrEmpty(input_returnQuantity.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterReturnQuantity, false);
            return;
        }

        if (string.IsNullOrEmpty(input_returnPrice.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterReturnPrice, false);
            return;
        }

        if (productStock.quantity < float.Parse(input_returnQuantity.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.NotEnoughStock, false);
            return;
        }

        if (block) return;
        block = true;

        ReturnProductStockParams stockReturn = new ReturnProductStockParams();
        stockReturn.productStockId = productStock.id;
        stockReturn.companyAccountId = product.company.accountId;
        stockReturn.returnQuantity = float.Parse(input_returnQuantity.text);
        stockReturn.returnPrice = float.Parse(input_returnPrice.text);
        stockReturn.details = "";
        stockReturn.returnDate = datepicker_date.SelectedDate;
        stockReturn.invoiceNumber = "";

        ProductsManager.Instance.ReturnManualProductStock(stockReturn, (response) =>
        {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Success, Constants.ManualStockReturned);
            GUIManager.Instance.Back();
            GUIManager.Instance.Back();
            ProductsManager.onProductAdded?.Invoke();
        }, null);
    }
}
