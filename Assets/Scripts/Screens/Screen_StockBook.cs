using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Screen_StockBook : MonoBehaviour
{
    public GameObject contentRoot;
    public TMP_Text text_productName, text_companyName;
    public MRDateFilterPicker dateFilterPicker;
    int productId;

    Product product;
    List<StockBookEntry> stockBook;

    private void OnDisable()
    {
        dateFilterPicker.onDateSelected -= GetStockBook;
    }

    public void ShowView(int productId)
    {
        this.productId = productId;
        GetStockBook();
    }

    void GetStockBook()
    {
        Preloader.Instance.ShowFull();
        ProductsManager.Instance.GetProduct(productId, (response) => {
            product = response.data;
            ProductsManager.Instance.GetStockBook(productId, this.dateFilterPicker.GetDateRange(), (response) => {
                stockBook = response.data;
                stockBook.Reverse();
                PopulateData();
                dateFilterPicker.onDateSelected -= GetStockBook;
                dateFilterPicker.onDateSelected += GetStockBook;
            }, null);
        }, null);
    }

    void PopulateData()
    {
        Preloader.Instance.ShowFull();
        text_productName.text = product.name;
        text_companyName.text = product.company.name;

        ClearContentRoot();

        foreach(StockBookEntry transaction in stockBook)
        {
            GameObject transactionObject = PoolManager.Instance.GetPooledItem(PoolManager.PoolType.StockBookList);
            transactionObject.transform.parent = contentRoot.transform;
            transactionObject.transform.localScale = Vector3.one;

            transactionObject.name = transaction.id.ToString();
            transactionObject.transform.Find("date").GetComponent<TMP_Text>().text = transaction.date.ToString("dd-MM-yyyy");

            transactionObject.transform.Find("type").GetComponent<TMP_Text>().text = transaction.type;

            transactionObject.transform.Find("bookNumber").GetComponent<TMP_Text>().text = transaction.bookNumber.ToString();
            transactionObject.transform.Find("billNumber").GetComponent<TMP_Text>().text = transaction.billNumber.ToString();
            transactionObject.transform.Find("invoiceNumber").GetComponent<TMP_Text>().text = transaction.invoiceNumber.ToString();

            transactionObject.transform.Find("closing").GetComponent<TMP_Text>().text = transaction.closing.ToString() + " " + product.unit.name;

            transactionObject.transform.Find("creditAmount").GetComponent<TMP_Text>().text = "";
            transactionObject.transform.Find("debitAmount").GetComponent<TMP_Text>().text = "";

            if (transaction.amount > 0)
                transactionObject.transform.Find("creditAmount").GetComponent<TMP_Text>().text = transaction.amount.ToString();
            else if (transaction.amount < 0)
                transactionObject.transform.Find("debitAmount").GetComponent<TMP_Text>().text = transaction.amount.ToString();

            transactionObject.transform.Find("referenceId").GetComponent<TMP_Text>().text = transaction.referenceId.ToString();

            transactionObject.transform.Find("referenceId").GetComponent<Button>().onClick.RemoveAllListeners();
            transactionObject.transform.Find("referenceId").GetComponent<Button>().onClick.AddListener(() => {
                if (transaction.type == Constants.ACCOUNT_CREATED)
                    ;//ViewAccount(transaction.referenceId);
                else if (transaction.type == Constants.SALE || transaction.type == Constants.SALE_PAYMENT)
                    ViewSale(transaction.referenceId);
                else if (transaction.type == Constants.BOOKING)
                    ;// ViewBooking(transaction.referenceId);
                else if (transaction.type == Constants.Expense)
                    ;// ViewExpense(transaction.referenceId);
                else if (transaction.type == Constants.Transfer)
                    ;// ViewTransfer(transaction.referenceId);
            });

            transactionObject.SetActive(true);
        }
        Preloader.Instance.HideFull();
    }

    void ViewSale(int saleId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Sale_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Sale_View_Add>().ShowView(saleId);
    }

    void ClearContentRoot()
    {
        foreach (Transform t in contentRoot.transform)
            t.gameObject.SetActive(false);
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
