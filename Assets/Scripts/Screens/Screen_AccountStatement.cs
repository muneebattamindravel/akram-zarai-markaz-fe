using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

public class Screen_AccountStatement : OSA<BaseParamsWithPrefab, AccountStatementViewHolder>
{
    public GameObject contentRoot;
    public TMP_Text text_accountName, text_accountType, text_accountBalance;
    public MRDateFilterPicker dateFilterPicker;
    int? accountId;

    Account account;
    List<AccountTransaction> statement;

    public SimpleDataHelper<AccountTransaction> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<AccountTransaction>(this);
        base.Start();
    }

    protected override AccountStatementViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new AccountStatementViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(AccountStatementViewHolder newOrRecycled)
    {
        AccountTransaction transaction = Data[newOrRecycled.ItemIndex];

        newOrRecycled.date.text = transaction.transactionDate.ToString(Constants.DateDisplayFormat);
        newOrRecycled.type.text = transaction.type;

        if (transaction.type == Constants.Expense)
            newOrRecycled.details.text = ((ExpenseType)int.Parse(transaction.details)).ToString();
        else if (transaction.type == Constants.Purchase)
            newOrRecycled.details.text = ((PurchaseType)int.Parse(transaction.details)).ToString();
        else
            newOrRecycled.details.text = transaction.details;

        newOrRecycled.referenceId.text = transaction.referenceId.ToString();
        newOrRecycled.closingBalance.text = transaction.closingBalance.ToCommaSeparatedNumbers() + Constants.Currency;

        if (transaction.amount == 0)
        {
            newOrRecycled.creditAmount.text = "0";
            newOrRecycled.debitAmount.text = "0";
        }
        else
        {
            newOrRecycled.creditAmount.text = "";
            newOrRecycled.debitAmount.text = "";
        }

        newOrRecycled.invoiceNumber.text = transaction.invoiceNumber;
        newOrRecycled.prNumber.text = transaction.prNumber;
        newOrRecycled.bookNumber.text = transaction.bookNumber;
        newOrRecycled.billNumber.text = transaction.billNumber;

        if (transaction.amount > 0)
            newOrRecycled.creditAmount.text = transaction.amount.ToCommaSeparatedNumbers() + Constants.Currency;
        else if (transaction.amount < 0)
            newOrRecycled.debitAmount.text = transaction.amount.ToCommaSeparatedNumbers() + Constants.Currency;

        newOrRecycled.referenceId.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.referenceId.GetComponent<MRButton>().onClicked.AddListener(() => {
            if (transaction.type == Constants.ACCOUNT_CREATED)
                ViewAccount(transaction.referenceId);
            else if (transaction.type == Constants.SALE || transaction.type == Constants.SALE_PAYMENT)
                ViewSale(transaction.referenceId);
            else if (transaction.type == Constants.BOOKING)
                ViewBooking(transaction.referenceId);
            else if (transaction.type == Constants.Expense)
                ViewExpense(transaction.referenceId);
            else if (transaction.type == Constants.Transfer)
                ViewTransfer(transaction.referenceId);
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowFull();
        text_accountName.text = account.name;
        text_accountType.text = account.type;
        text_accountBalance.text = account.balance + Constants.Currency;

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, statement.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideFull();
    }

    private void OnEnable()
    {
        dateFilterPicker.onDateSelected += GetAccount;
    }
    private void OnDisable()
    {
        dateFilterPicker.onDateSelected -= GetAccount;
        statement = null;
        GC.Collect();
    }

    public void ShowView(int? accountId)
    {
        this.accountId = accountId;
        GetAccount();
    }

    void GetAccount()
    {
        Preloader.Instance.ShowFull();

        AccountsManager.Instance.GetAccount(accountId, (response) => {
            account = response.data;
            AccountsManager.Instance.GetAccountStatement(accountId, dateFilterPicker.GetDateRange(), (response) =>
            {
                statement = response.data;
                statement.Reverse();
                PopulateData();

                dateFilterPicker.onDateSelected -= GetAccount;
                dateFilterPicker.onDateSelected += GetAccount;
            });
        }, null);
    }

    void ViewAccount(int accountId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Accounts_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Accounts_View_Add>().ShowView(accountId, true);
    }

    void ViewBooking(int bookingId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Bookings_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Bookings_View_Add>().ShowView(bookingId, true);
    }

    void ViewExpense(int expenseId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Expenses_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Expenses_View_Add>().ShowView(expenseId, true);
    }

    void ViewTransfer(int transferId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Transfers_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Transfers_View_Add>().ShowView(transferId, true);
    }

    void ViewSale(int saleId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Sale_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Sale_View_Add>().ShowView(saleId);
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}

public class AccountStatementViewHolder : BaseItemViewsHolder
{
    public TMP_Text date, type, details, referenceId, debitAmount, creditAmount,
        closingBalance, invoiceNumber, prNumber, bookNumber, billNumber;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("date", out date);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("details", out details);
        root.GetComponentAtPath("referenceId", out referenceId);
        root.GetComponentAtPath("debitAmount", out debitAmount);
        root.GetComponentAtPath("creditAmount", out creditAmount);
        root.GetComponentAtPath("closingBalance", out closingBalance);
        root.GetComponentAtPath("invoiceNumber", out invoiceNumber);
        root.GetComponentAtPath("prNumber", out prNumber);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
    }
}
