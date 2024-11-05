using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_ViewSalePayments : MonoBehaviour
{
    int saleId;
    Sale sale;
    List<SalePayment> salePayments;

    //UI
    public TMP_Text text_saleId, text_saleTotalAmount, text_salePendingAmount, text_customerName;
    float remainingAmount = 0f;

    public GameObject contentRoot, salePaymentsItemPrefab;

    public void ShowView(int saleId)
    {
        this.saleId = saleId;
        GetSaleDetails();
    }

    void GetSaleDetails()
    {
        SalesManager.Instance.GetSale(saleId,
            (response) =>
            {
                sale = response.data;
                text_saleId.text = sale.id.ToString();
                text_saleTotalAmount.text = sale.totalAmount + Constants.Currency;
                remainingAmount = sale.totalAmount - sale.receivedAmount;
                text_salePendingAmount.text = remainingAmount + Constants.Currency;

                if (sale.contact != null)
                    text_customerName.text = sale.contact.name;

                SalesManager.Instance.GetSalePayments(saleId, (response) => {
                    salePayments = response.data;
                    PopulateData();
                }, null);
            },
            (response) =>
            {
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
            });
    }

    void PopulateData()
    {
        ClearContentRoot();

        foreach (SalePayment salePayment in salePayments)
        {
            GameObject obj = Instantiate(salePaymentsItemPrefab, contentRoot.transform);
            obj.name = salePayment.id.ToString();
            obj.transform.Find("date").GetComponent<TMP_Text>().text = salePayment.receivedDate.ToShortDateString();
            obj.transform.Find("type").GetComponent<TMP_Text>().text = salePayment.paymentType.ToString();
            obj.transform.Find("bookNumber").GetComponent<TMP_Text>().text = salePayment.bookNumber;
            obj.transform.Find("billNumber").GetComponent<TMP_Text>().text = salePayment.billNumber;
            obj.transform.Find("creditedAccount").GetComponent<TMP_Text>().text = salePayment.account.name;
            obj.transform.Find("amount").GetComponent<TMP_Text>().text = salePayment.receivedAmount + Constants.Currency;
        }
    }

    void ClearContentRoot()
    {
        foreach (Transform t in contentRoot.transform)
            Destroy(t.gameObject);
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
