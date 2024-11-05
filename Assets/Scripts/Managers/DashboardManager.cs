using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class DashboardManager : MonoBehaviour
{
    public static DashboardManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string DASHBOARD_TOPBAR_ROUTE = "dashboard/topbar";
    string DASHBOARD_BUSINESS_REPORT_ROUTE = "dashboard/businessreport";
    string DASHBOARD_TOP_LOANS_ROUTE = "dashboard/toploans";

    public void GetTopBarData(DateTime from, DateTime to, ResponseAction<TopBarData> successAction, ResponseAction<TopBarData> failAction = null)
    {
        APIManager.Instance.Get<TopBarData>(DASHBOARD_TOPBAR_ROUTE + "?from=" + from + "&to=" + to, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetBusinesReport(ResponseAction<BusinessReport> successAction, ResponseAction<BusinessReport> failAction = null)
    {
        APIManager.Instance.Get<BusinessReport>(DASHBOARD_BUSINESS_REPORT_ROUTE, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetTopLoans(ResponseAction<List<Account>> successAction, ResponseAction<List<Account>> failAction = null)
    {
        APIManager.Instance.Get<List<Account>>(DASHBOARD_TOP_LOANS_ROUTE, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
