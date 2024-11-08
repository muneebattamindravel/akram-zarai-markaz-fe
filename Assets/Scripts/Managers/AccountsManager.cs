using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class AccountsManager : MonoBehaviour
{
    public static AccountsManager Instance;
    public delegate void AccountsEvent();

    public static AccountsEvent onAccountAdded, onAccountUpdated, onPartnersListEvent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        ;
    }

    

    string ACCOUNTS_ROUTE = "accounts";
    string ADD_CAPITAL_ROUTE = "accounts/capital";
    string ADD_PROFIT_ROUTE = "accounts/profit";
    string ACCOUNTS_STATEMENT_ROUTE = "accounts/statement";
    string DEFAULT_ACCOUNT_BALANCE   = "defaultAccount/balance";

    public void AddAccount(Account account, ResponseAction<Account> successAction, ResponseAction<Account> failAction = null)
    {
        APIManager.Instance.Post<Account>(ACCOUNTS_ROUTE, account, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetAccount(int? accountId, ResponseAction<Account> successAction, ResponseAction<Account> failAction = null)
    {
        APIManager.Instance.Get<Account>(ACCOUNTS_ROUTE + "/" + accountId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetAccountStatement(int? accountId, MRDateRange range, ResponseAction<List<AccountTransaction>> successAction, ResponseAction<List<AccountTransaction>> failAction = null)
    {
        APIManager.Instance.Get<List<AccountTransaction>>(ACCOUNTS_STATEMENT_ROUTE + "/" + accountId + "?from=" + range.from + "&to=" + range.to, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetDefaultAccountBalance(ResponseAction<Amount> successAction, ResponseAction<Amount> failAction = null)
    {
        APIManager.Instance.Get<Amount>(ACCOUNTS_ROUTE + "/" + DEFAULT_ACCOUNT_BALANCE, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetAccounts(ResponseAction<List<Account>> successAction, ResponseAction<List<Account>> failAction = null)
    {
        APIManager.Instance.Get<List<Account>>(ACCOUNTS_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateAccount(Account account, int accountId, ResponseAction<Account> successAction, ResponseAction<Account> failAction = null)
    {
        APIManager.Instance.Patch<Account>(ACCOUNTS_ROUTE + "/" + accountId, account, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteAccount(int accountId, ResponseAction<Account> successAction, ResponseAction<Account> failAction = null)
    {
        APIManager.Instance.Delete<Account>(ACCOUNTS_ROUTE + "/" + accountId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }

    public void AddPartnerCapital(PartnerCapitalAddParam capital, ResponseAction<PartnerCapitalAddParam> successAction, ResponseAction<PartnerCapitalAddParam> failAction = null)
    {
        APIManager.Instance.Post<PartnerCapitalAddParam>(ADD_CAPITAL_ROUTE, capital, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void AddPartnerProfit(PartnerProfitAddParam profit, ResponseAction<PartnerProfitAddParam> successAction, ResponseAction<PartnerProfitAddParam> failAction = null)
    {
        APIManager.Instance.Post<PartnerProfitAddParam>(ADD_PROFIT_ROUTE, profit, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}

[System.Serializable]
public class PartnerCapitalAddParam
{
    public int partnerAccountId;
    public int creditAccountId;
    public float amount;
    public DateTime date;
    public string details;
}


[System.Serializable]
public class PartnerProfitAddParam
{
    public int partnerAccountId;
    public float amount;
    public DateTime date;
    public string details;
}