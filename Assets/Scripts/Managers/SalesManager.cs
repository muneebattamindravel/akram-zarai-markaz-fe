using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class SalesManager : MonoBehaviour
{
    public static SalesManager Instance;
    public delegate void SalesEvent();
    public static SalesEvent onSaleAdded, onSaleUpdated, onSaleDeleted, onReceivedSalePayment;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string SALES_ROUTE = "sales";
    string SALE_RETURNS_ROUTE = "saleReturns";
    string SALES_PAYMENTS_ROUTE = "salePayments";
    string SALES_COUNTER_SALES_AMOUNT = "/counterSalesAmount/range";

    public void AddSale(Sale sale, ResponseAction<Sale> successAction, ResponseAction<Sale> failAction = null)
    {
        APIManager.Instance.Post<Sale>(SALES_ROUTE, sale, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void ReturnSaleItems(SaleReturn saleReturn, ResponseAction<SaleReturn> successAction, ResponseAction<SaleReturn> failAction = null)
    {
       APIManager.Instance.Post<SaleReturn>(SALE_RETURNS_ROUTE + "/" + saleReturn.saleId, saleReturn, (response) =>
       {
           successAction(response);
       }, (response) => {
           if (failAction != null)
               failAction(response);
       });
    }

    public void GetSale(int saleId, ResponseAction<Sale> successAction, ResponseAction<Sale> failAction = null)
    {
        APIManager.Instance.Get<Sale>(SALES_ROUTE + "/" + saleId, (response) =>
        {
            ConsolidateLostUsedInformation(response.data);
            successAction(response);

        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetSales(MRDateRange range, ResponseAction < List<Sale>> successAction, ResponseAction<List<Sale>> failAction = null)
    {
        APIManager.Instance.Get<List<Sale>>(SALES_ROUTE + "?from="+range.from+"&to="+range.to, (response) => {

            foreach(Sale sale in response.data)
                ConsolidateLostUsedInformation(sale);

            successAction(response);

        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetSaleReturns(MRDateRange range, ResponseAction <List<SaleReturn>> successAction, ResponseAction<List<SaleReturn>> failAction = null)
    {
        APIManager.Instance.Get<List<SaleReturn>>(SALE_RETURNS_ROUTE + "?from="+range.from+"&to="+range.to, (response) => {

            successAction(response);

        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    void ConsolidateLostUsedInformation(Sale sale) {
        if (sale == null || sale.saleitems == null)
            return;
        foreach(SaleItem item in sale.saleitems) {
            item.ConsolidateLotUsedInformation();
        }
    }

    public void GetSalePayments(int saleId, ResponseAction<List<SalePayment>> successAction, ResponseAction<List<SalePayment>> failAction = null)
    {
        APIManager.Instance.Get<List<SalePayment>>(SALES_PAYMENTS_ROUTE + "/" + saleId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateSale(Sale sale, int saleId, ResponseAction<Sale> successAction, ResponseAction<Sale> failAction = null)
    {
        APIManager.Instance.Patch<Sale>(SALES_ROUTE + "/" + saleId, sale, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteSale(int saleId, ResponseAction<Sale> successAction, ResponseAction<Sale> failAction = null)
    {
        APIManager.Instance.Delete<Sale>(SALES_ROUTE + "/" + saleId, (response) =>
        {
            successAction(response);
            onSaleDeleted?.Invoke();
        }, (response) =>
        {
            failAction(response);
        });
    }

    public void GetCounterSaleAmount(DateTime from, DateTime to, ResponseAction<AmountInRange> successAction, ResponseAction<AmountInRange> failAction = null)
    {
        APIManager.Instance.Get<AmountInRange>(SALES_ROUTE + SALES_COUNTER_SALES_AMOUNT + "?from=" + from + "&to=" + to, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void ReceiveSalePayment(SalePayment payment, ResponseAction<SalePayment> successAction, ResponseAction<SalePayment> failAction = null)
    {
        APIManager.Instance.Post<SalePayment>(SALES_PAYMENTS_ROUTE, payment, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
