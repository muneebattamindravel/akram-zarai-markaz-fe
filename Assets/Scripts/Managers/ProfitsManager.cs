using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ProfitsManager : MonoBehaviour
{
    public static ProfitsManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string PROFITS_ROUTE = "profits";

    public void GetCounterSaleProfit(DateTime from, DateTime to, ResponseAction<AmountInRange> successAction, ResponseAction<AmountInRange> failAction = null)
    {
        APIManager.Instance.Get<AmountInRange>(PROFITS_ROUTE + "/counterSalesProfit?from="+from +"&to="+to, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetSaleProfits(MRDateRange range, ResponseAction<List<SaleProfit>> successAction, ResponseAction<List<SaleProfit>> failAction = null)
    {
        APIManager.Instance.Get<List<SaleProfit>>(PROFITS_ROUTE + "/saleProfits?from=" + range.from + "&to=" + range.to, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
