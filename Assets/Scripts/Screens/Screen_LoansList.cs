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

public class Screen_LoansList : OSA<BaseParamsWithPrefab, LoansListViewHolder>
{
    public GameObject contentRoot;
    public List<Loan> loans;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;

    public SimpleDataHelper<Loan> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Loan>(this);
        base.Start();
    }

    protected override LoansListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new LoansListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(LoansListViewHolder newOrRecycled)
    {
        Loan loan = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.GetComponent<TMP_Text>().text = loan.id.ToString();

        newOrRecycled.contact.GetComponent<TMP_Text>().text = loan.contact.name;
        newOrRecycled.date.GetComponent<TMP_Text>().text = loan.date.ToString(Constants.DateDisplayFormat);
        newOrRecycled.bookNumber.GetComponent<TMP_Text>().text = loan.bookNumber.ToString();
        newOrRecycled.billNumber.GetComponent<TMP_Text>().text = loan.billNumber.ToString();
        newOrRecycled.amount.GetComponent<TMP_Text>().text = loan.amount + Constants.Currency;
        newOrRecycled.accountName.GetComponent<TMP_Text>().text = loan.account.name;

        newOrRecycled.type.transform.Find("given").gameObject.SetActive(false);
        newOrRecycled.type.transform.Find("taken").gameObject.SetActive(false);
        if (loan.isReceived)
            newOrRecycled.type.transform.Find("taken").gameObject.SetActive(true);
        else
            newOrRecycled.type.transform.Find("given").gameObject.SetActive(true);

        newOrRecycled.delete.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.delete.GetComponent<MRButton>().onClicked.AddListener(() =>
        {
            DeleteLoanCompletely(loan.id);
        });
    }

    private void OnEnable()
    {
        GetLoans();

        LoansManager.onLoanAdded += GetLoans;
        dateFilterPicker.gameObject.SetActive(true);
        dateFilterPicker.onDateSelected += GetLoans;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        LoansManager.onLoanAdded -= GetLoans;
        dateFilterPicker.onDateSelected -= GetLoans;

        loans = null;
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

                ColumnState nextState = header.SetNextState();
                FieldInfo fieldInfo = typeof(Loan).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    loans = loans.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    loans = loans.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    loans = loans.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Loan item in loans) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Loan).GetField(header.dataField);
                foreach (Loan filtered in loans.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetLoans()
    {
        Preloader.Instance.ShowWindowed();
        LoansManager.Instance.GetLoans(dateFilterPicker.GetDateRange(), (response) => {
            loans = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        }, null);
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, loans.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    void DeleteLoanCompletely(int loanId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Loan,
            Constants.DELETE, Constants.CANCEL,
            () => {
                //Confirmed
                Preloader.Instance.ShowFull();
                LoansManager.Instance.DeleteLoan(loanId, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.LoanDeleted);
                    GetLoans();
                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
                });
            },
            () => {
                //Cancelled
            }
        );
    }

    public void Button_GiveLoanClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Loans_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Loans_View_Add>().ShowView(false);
    }

    public void Button_TakeLoanClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Loans_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Loans_View_Add>().ShowView(true);
    }

    public void Button_ReloadClicked()
    {
        GetLoans();
    }
}

public class LoansListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, date, contact, accountName, type, bookNumber, billNumber, amount;
    public MRButton view, delete;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("date", out date);
        root.GetComponentAtPath("contact", out contact);
        root.GetComponentAtPath("accountName", out accountName);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
        root.GetComponentAtPath("amount", out amount);
        root.GetComponentAtPath("Button_View", out view);
        root.GetComponentAtPath("Button_Delete", out delete);
    }
}