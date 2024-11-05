using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Widget_BusinessReport : MonoBehaviour
{
    public TMP_Text text_cash, text_companies, text_customerLoans, text_stock, text_total, text_capital;
    public GameObject loader, body;
    private void OnEnable()
    {
        body.SetActive(false);
        loader.SetActive(true);

        DashboardManager.Instance.GetBusinesReport(
        (response) =>
        {
            float total = 0f;
            
            text_cash.text = response.data.totalCashAmount.ToCommaSeparatedNumbers() + Constants.Currency;
            text_companies.text = response.data.amountInCompanies.ToCommaSeparatedNumbers() + Constants.Currency;
            text_customerLoans.text = response.data.customerLoansAmount.ToCommaSeparatedNumbers() + Constants.Currency;
            text_stock.text = response.data.totalStockAmount.ToCommaSeparatedNumbers() + Constants.Currency;
            text_capital.text = response.data.totalCapitalAmount.ToCommaSeparatedNumbers() + Constants.Currency;
            total += response.data.totalCashAmount;
            total += response.data.amountInCompanies;
            total += response.data.customerLoansAmount;
            total += response.data.totalStockAmount;
            text_total.text = total.ToCommaSeparatedNumbers() + Constants.Currency;

            body.SetActive(true);
            loader.SetActive(false);
        },
        (response) =>
        {

        });
    }
}
