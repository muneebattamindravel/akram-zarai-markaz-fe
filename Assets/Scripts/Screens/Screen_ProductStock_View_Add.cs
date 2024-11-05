using UnityEngine.Events;
using UnityEngine;
using TMPro;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_ProductStock_View_Add : MonoBehaviour
{
    public TMP_InputField input_productName, input_costPrice, input_initialQuantity,
        input_totalAmount, input_batchNumber, input_notes, input_invoiceNumber;
    public TMP_Text text_title, text_unit;
    public MRDatePicker datepicker_expiryDate;
    ViewMode mode;
    Product product;
    ProductStock productStock;
    float totalAmount = 0;
    public GameObject buttonSave;

    private void OnEnable()
    {
        input_costPrice.text = "";
        input_totalAmount.text = "";
        input_batchNumber.text = "";
        input_initialQuantity.text = "";
        input_invoiceNumber.text = "";
        input_notes.text = "";

        buttonSave.SetActive(true);

        totalAmount = 0;

        input_productName.interactable = false;
        input_totalAmount.interactable = false;

        KeyboardManager.enterPressed += OnEnterPressed;
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void ShowView(Product product)
    {
        mode = ViewMode.ADD;
        text_title.text = Constants.Add + " " + Constants.ProductStock;
        input_productName.text = product.name;
        this.product = product;
        text_unit.text = "("+product.unit.name+")";
    }

    public void ShowView(int productStockId, Product product, bool viewOnly = false)
    {
        this.product = product;
        input_productName.text = product.name;
        text_unit.text = "(" + product.unit.name + ")";
        GetProductStock(productStockId);

        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            buttonSave.SetActive(false);
            text_title.text = Constants.View + " " + Constants.ProductStock;
        }
        else
        {
            mode = ViewMode.EDIT;
            buttonSave.SetActive(true);
            text_title.text = Constants.Edit + " " + Constants.ProductStock;
        }
    }

    void GetProductStock(int productStockId)
    {
        Preloader.Instance.ShowFull();
        ProductsManager.Instance.GetProductStock(productStockId,
            (response) => {
                Preloader.Instance.HideFull();
                productStock = response.data;
                input_costPrice.text = productStock.costPrice.ToString();
                input_initialQuantity.text = productStock.initialQuantity.ToString();
                input_totalAmount.text = (productStock.initialQuantity * productStock.costPrice).ToString();
                input_batchNumber.text = productStock.batchNumber;
                input_invoiceNumber.text = productStock.invoiceNumber;
                datepicker_expiryDate.SelectedDate = productStock.expiryDate;
            },
            (response) => {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
            });
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public void Button_SaveClicked()
    {
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
        if (totalAmount <= 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.TotalAmountZero, false);
            return;
        }
        if (string.IsNullOrEmpty(input_batchNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.BatchNumberEmpty, false);
            return;
        }
        if (string.IsNullOrEmpty(input_invoiceNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.InvoiceNumberEmpty, false);
            return;
        }
        if ( datepicker_expiryDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ExpiryDateEmpty, false);
            return;
        }

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            ProductStock productStock = new ProductStock();
            productStock.batchNumber = input_batchNumber.text;
            productStock.invoiceNumber = input_invoiceNumber.text;
            productStock.costPrice = float.Parse(input_costPrice.text);
            productStock.initialQuantity = int.Parse(input_initialQuantity.text);
            productStock.notes = input_notes.text;
            productStock.productId = product.id;
            productStock.purchaseId = 0; // 0 means backend will convert it to null

            if ( datepicker_expiryDate.SelectedDate == DateTime.MinValue)
                productStock.expiryDate = DateTime.Now.AddYears(2);
            else
            {
                productStock.expiryDate = datepicker_expiryDate.SelectedDate;
                productStock.expiryDate = productStock.expiryDate.AddSeconds(86399);
            }

            Debug.Log(productStock.expiryDate);

            ProductsManager.Instance.AddProductStock(productStock,
            (response) =>
            {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Success, Constants.ProductStockAdded);
                if (ProductsManager.onProductStockAdded != null) ProductsManager.onProductStockAdded();
                GUIManager.Instance.Back();
            },
            (response) =>
            {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            }
            );
        }
        else if (mode == ViewMode.EDIT)
        {
            this.productStock.batchNumber = input_batchNumber.text;
            this.productStock.costPrice = float.Parse(input_costPrice.text);
            this.productStock.initialQuantity = int.Parse(input_initialQuantity.text);
            this.productStock.notes = input_notes.text;

            ProductsManager.Instance.UpdateProductStock(this.productStock, this.productStock.id,
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.ProductStockUpdated);
                    if (ProductsManager.onProductStockUpdated != null) ProductsManager.onProductStockUpdated();
                    GUIManager.Instance.Back();
                },
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                }
            );
        }
    }

    public void CalculateTotalAmount()
    {
        if (string.IsNullOrEmpty(input_costPrice.text) || string.IsNullOrEmpty(input_initialQuantity.text))
        {
            input_totalAmount.text = "0" + Constants.Currency;
            return;
        }

        totalAmount = (float.Parse(input_costPrice.text) * float.Parse(input_initialQuantity.text));
        input_totalAmount.text = totalAmount.ToString("0.##") + Constants.Currency;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
