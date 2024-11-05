using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using TMPro;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using UnityEngine.UI;

public class Screen_PartnersList : OSA<BaseParamsWithPrefab, PartnersListViewHolder>
{
    public GameObject contentRoot;
    public List<Account> accounts;
    public List<ColumnHeader> columnHeaders;

    public TMP_Text text_totalCapital;
    float totalCapital;

    public SimpleDataHelper<Account> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Account>(this);
        base.Start();
    }

    protected override PartnersListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new PartnersListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(PartnersListViewHolder newOrRecycled)
    {
        Account account = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.GetComponent<TMP_Text>().text = account.id.ToString();
        if (account.isDefault)
            newOrRecycled.name.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
        else
            newOrRecycled.name.GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;

        newOrRecycled.name.GetComponent<TMP_Text>().text = account.name;

        if (account.balance >= 0)
            newOrRecycled.balance.GetComponent<TMP_Text>().color = Constants.PositiveColor;
        else
            newOrRecycled.balance.GetComponent<TMP_Text>().color = Constants.NegativeColor;

        newOrRecycled.balance.GetComponent<TMP_Text>().text = account.balance.ToCommaSeparatedNumbers() + Constants.Currency;

        float percentage = (account.balance / totalCapital) * 100.0f;
        newOrRecycled.share.GetComponent<TMP_Text>().text = percentage + Constants.Percentage;

        newOrRecycled.viewStatement.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.viewStatement.GetComponent<MRButton>().onClicked.AddListener(() => {
            ViewAccountStatement(account.id);
        });
    }

    private void OnEnable()
    {
        GetAccounts();
        text_totalCapital.text = "";

        InitializeColumnsHeaders();

        AccountsManager.onPartnersListEvent += GetAccounts;
        accountStatementPicker.defaultDateType = DateType.AllTime;
    }

    private void OnDisable()
    {
        AccountsManager.onPartnersListEvent -= GetAccounts;

        accounts = null;
        GC.Collect();
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

    void GetAccounts()
    {
        Preloader.Instance.ShowWindowed();
        AccountsManager.Instance.GetAccounts((response) => {

            response.data.RemoveAll(p => p.type != AccountType.Partner.ToString());
            accounts = response.data;
            columnHeaders[0].ResetState();
            columnHeaders[0].SetNextState();
            PopulateData();

        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();
        totalCapital = 0f;

        foreach (Account account in accounts.FindAll(p => p.IsEnabledOnGrid))
            totalCapital += account.balance;

        text_totalCapital.text = totalCapital.ToCommaSeparatedNumbers() + Constants.Currency;
        
        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, accounts.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
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

    public void Button_WithdrawCapitalClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Transfers_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Transfers_View_Add>().ShowView(Screen_Transfers_View_Add.TransferType.WITHDRAW_CAPITAL);
    }

    public void Button_PostProfitClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.PartnersProfit_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_PartnersProfit_Add>().ShowView();
    }

    public void Button_AddCapitalClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.PartnersCapital_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_PartnersCapital_Add>().ShowView();
    }
}

public class PartnersListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, balance, share;
    public MRButton viewStatement;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("balance", out balance);
        root.GetComponentAtPath("share", out share);
        root.GetComponentAtPath("Button_ViewStatement", out viewStatement);
    }
}