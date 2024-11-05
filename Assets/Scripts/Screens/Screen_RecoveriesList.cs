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

public class Screen_RecoveriesList : OSA<BaseParamsWithPrefab, RecoveriesListViewHolder>
{
    public GameObject contentRoot;
    public List<Recovery> recoveries;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;

    public SimpleDataHelper<Recovery> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Recovery>(this);
        base.Start();
    }

    protected override RecoveriesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new RecoveriesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(RecoveriesListViewHolder newOrRecycled)
    {
        Recovery recovery = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = recovery.id.ToString();
        newOrRecycled.date.text = recovery.date.ToString(Constants.DateDisplayFormat);
        newOrRecycled.contact.text = recovery.contact.name;

        newOrRecycled.type.transform.Find("given").gameObject.SetActive(false);
        newOrRecycled.type.transform.Find("taken").gameObject.SetActive(false);
        if (recovery.isReceived)
            newOrRecycled.type.transform.Find("taken").gameObject.SetActive(true);
        else
            newOrRecycled.type.transform.Find("given").gameObject.SetActive(true);

        newOrRecycled.accountName.text = recovery.account.name;
        newOrRecycled.bookNumber.text = recovery.bookNumber.ToString();
        newOrRecycled.billNumber.text = recovery.billNumber.ToString();

        if (recovery.isReceived)
            newOrRecycled.amount.color = Constants.PositiveColor;
        else
            newOrRecycled.amount.color = Constants.NegativeColor;

        newOrRecycled.amount.text = recovery.amount.ToString();

        newOrRecycled.view.onClicked.RemoveAllListeners();
        newOrRecycled.view.onClicked.AddListener(() => {
            ViewRecovery(recovery.id);
        });

        newOrRecycled.delete.onClicked.RemoveAllListeners();
        newOrRecycled.delete.onClicked.AddListener(() => {
            DeleteRecoveryCompletely(recovery.id);
        });
    }

    private void OnEnable()
    {
        GetRecoveries();

        RecoveriesManager.onRecoveryAdded += GetRecoveries;
        dateFilterPicker.gameObject.SetActive(true);
        dateFilterPicker.onDateSelected += GetRecoveries;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        RecoveriesManager.onRecoveryAdded -= GetRecoveries;
        dateFilterPicker.onDateSelected -= GetRecoveries;

        recoveries = null;
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
                FieldInfo fieldInfo = typeof(Recovery).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    recoveries = recoveries.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    recoveries = recoveries.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    recoveries = recoveries.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Recovery item in recoveries) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Recovery).GetField(header.dataField);
                foreach (Recovery filtered in recoveries.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetRecoveries()
    {
        Preloader.Instance.ShowWindowed();
        RecoveriesManager.Instance.GetRecoveries(dateFilterPicker.GetDateRange(), (response) => {
            recoveries = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        }, null);
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, recoveries.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    void DeleteRecoveryCompletely(int recoveryId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Recovery,
            Constants.DELETE, Constants.CANCEL,
            () => {
                //Confirmed
                Preloader.Instance.ShowFull();
                RecoveriesManager.Instance.DeleteRecovery(recoveryId, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.RecoveryDeleted);
                    GetRecoveries();
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

    public void Button_GiveRecoveryClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Recoveries_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Recoveries_View_Add>().ShowView(false);
    }

    public void Button_TakeRecoveryClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Recoveries_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Recoveries_View_Add>().ShowView(true);
    }

    void ViewRecovery(int recoveryId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Recoveries_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Recoveries_View_Add>().ShowView(recoveryId, true);
    }

    public void Button_ReloadClicked()
    {
        GetRecoveries();
    }
}

public class RecoveriesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, date, contact, type, accountName, bookNumber, billNumber, amount;
    public MRButton view, delete;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("date", out date);
        root.GetComponentAtPath("contact", out contact);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("accountName", out accountName);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
        root.GetComponentAtPath("amount", out amount);
        root.GetComponentAtPath("Button_View", out view);
        root.GetComponentAtPath("Button_Delete", out delete);
    }
}