using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using TMPro;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

public class Screen_AccountsList : OSA<BaseParamsWithPrefab, AccountsListViewHolder>
{
    public GameObject contentRoot;
    public List<Account> accounts, accountsFiltered;
    public List<ColumnHeader> columnHeaders;
    public TMP_Dropdown dropdown_accountType;
    AccountType selectedAccountType;

    public TMP_Text text_totalBalance;
    float totalBalance;

    public SimpleDataHelper<Account> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Account>(this);
        base.Start();
    }

    protected override AccountsListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new AccountsListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(AccountsListViewHolder newOrRecycled)
    {
        Account account = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = account.id.ToString();
        newOrRecycled.name.text = account.name.ToString();
        newOrRecycled.type.text = account.type.ToString();

        if (account.isDefault)
            newOrRecycled.name.fontStyle = FontStyles.Bold;
        else
            newOrRecycled.name.fontStyle = FontStyles.Normal;

        if (account.balance >= 0)
            newOrRecycled.balance.color = Constants.PositiveColor;
        else
            newOrRecycled.balance.color = Constants.NegativeColor;

        newOrRecycled.balance.text = account.balance.ToCommaSeparatedNumbers() + Constants.Currency;

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            ViewAccount(account.id);
        });

        newOrRecycled.viewStatement.onClicked.RemoveAllListeners();
        newOrRecycled.viewStatement.onClicked.AddListener(() => {
            ViewAccountStatement(account.id);
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        totalBalance = 0f;
        foreach (Account account in accountsFiltered.FindAll(p => p.IsEnabledOnGrid))
            totalBalance += account.balance;
        text_totalBalance.text = totalBalance.ToCommaSeparatedNumbers() + Constants.Currency;

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, accountsFiltered.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    private void OnEnable()
    {
        GetAccounts();
        text_totalBalance.text = "";
        AccountsManager.onAccountAdded += GetAccounts;
        AccountsManager.onAccountUpdated += GetAccounts;
        SalesManager.onSaleAdded += GetAccounts;

        InitializeColumnsHeaders();

        accountStatementPicker.defaultDateType = DateType.Last30Days;
    }

    public void InitializeColumnsHeaders()
    {
        foreach (ColumnHeader header in columnHeaders)
        {
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.RemoveAllListeners();
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.AddListener(() => {
                foreach (ColumnHeader hdr in columnHeaders)
                    if (hdr != header)
                        hdr.ResetState();

                Debug.Log(header.dataField);

                ColumnState nextState = header.SetNextState();
                FieldInfo fieldInfo = typeof(Account).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    accounts = accounts.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    accounts = accounts.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    accounts = accounts.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {             

                foreach (Account item in accounts) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Account).GetField(header.dataField);
                foreach (Account filtered in accounts.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    private void OnDisable()
    {
        AccountsManager.onAccountAdded -= GetAccounts;
        AccountsManager.onAccountUpdated -= GetAccounts;
        SalesManager.onSaleAdded -= GetAccounts;

        accounts = null;
        GC.Collect();
    }

    void GetAccounts()
    {
        Preloader.Instance.ShowWindowed();
        AccountsManager.Instance.GetAccounts((response) => {

            response.data.RemoveAll(p => p.type == AccountType.Partner.ToString());
            accounts = response.data;

            accountsFiltered = accounts;

            columnHeaders[0].ResetState();
            columnHeaders[0].SetNextState();
            PopulateData();

            AccountTypeChanged();
        });
    }

    public void AccountTypeChanged()
    {
        selectedAccountType = (AccountType) dropdown_accountType.value;

        if (selectedAccountType != AccountType.All)
            accountsFiltered = accounts.FindAll(p => p.type.ToString() == selectedAccountType.ToString());
        else
            accountsFiltered = accounts;

        PopulateData();
    }

    void ViewAccount(int accountId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Accounts_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Accounts_View_Add>().ShowView(accountId);
    }

    public MRDateFilterPicker accountStatementPicker;
    void ViewAccountStatement(int accountId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.AccountStatement);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_AccountStatement>().ShowView(accountId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Accounts_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Accounts_View_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetAccounts();
    }
}

public class AccountsListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, type, balance;
    public MRButton viewStatement, edit;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("balance", out balance);
        root.GetComponentAtPath("Button_ViewStatement", out viewStatement);
        root.GetComponentAtPath("Button_Edit", out edit);
    }
}

