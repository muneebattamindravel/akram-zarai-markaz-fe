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

public class Screen_IncentivesList : OSA<BaseParamsWithPrefab, IncentivesListViewHolder>
{
    public GameObject contentRoot;
    public List<Incentive> incentives;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;
    public TMP_Text text_totalIncentives;

    public SimpleDataHelper<Incentive> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Incentive>(this);
        base.Start();
    }

    protected override IncentivesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new IncentivesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(IncentivesListViewHolder newOrRecycled)
    {
        Incentive incentive = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.GetComponent<TMP_Text>().text = incentive.id.ToString();
        newOrRecycled.notes.GetComponent<TMP_Text>().text = incentive.notes.ToString();
        newOrRecycled.date.GetComponent<TMP_Text>().text = incentive.date.ToString(Constants.DateDisplayFormat);
        newOrRecycled.amount.GetComponent<TMP_Text>().text = incentive.amount.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.companyName.GetComponent<TMP_Text>().text = incentive.company.name;
        newOrRecycled.type.GetComponent<TMP_Text>().text = incentive.type;

        newOrRecycled.delete.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.delete.GetComponent<MRButton>().onClicked.AddListener(() =>
        {
            DeleteIncentiveCompletely(incentive.id);
        });
    }

    private void OnEnable()
    {
        GetIncentives();

        IncentivesManager.onIncentiveAdded += GetIncentives;
        dateFilterPicker.gameObject.SetActive(true);
        dateFilterPicker.onDateSelected += GetIncentives;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        IncentivesManager.onIncentiveAdded -= GetIncentives;
        dateFilterPicker.onDateSelected -= GetIncentives;

        incentives = null;
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
                FieldInfo fieldInfo = typeof(Incentive).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    incentives = incentives.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    incentives = incentives.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    incentives = incentives.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Incentive item in incentives) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Incentive).GetField(header.dataField);
                foreach (Incentive filtered in incentives.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetIncentives()
    {
        Preloader.Instance.ShowWindowed();
        IncentivesManager.Instance.GetIncentives(dateFilterPicker.GetDateRange(), (response) => {
            incentives = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        }, null);
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();
        float totalIncentives = 0f;
        foreach (Incentive i in incentives)
            totalIncentives += i.amount;
        text_totalIncentives.text = totalIncentives.ToCommaSeparatedNumbers();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, incentives.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    void DeleteIncentiveCompletely(int incentiveId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Incentive,
            Constants.DELETE, Constants.CANCEL,
            () => {
                //Confirmed
                Preloader.Instance.ShowFull();
                IncentivesManager.Instance.DeleteIncentive(incentiveId, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.IncentiveDeleted);
                    GetIncentives();
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

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Incentives_View_Add);
    }

    public void Button_ReloadClicked()
    {
        GetIncentives();
    }
}

public class IncentivesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, date, contact, companyName, type, notes, amount;
    public MRButton view, delete;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("date", out date);
        root.GetComponentAtPath("contact", out contact);
        root.GetComponentAtPath("companyName", out companyName);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("notes", out notes);
        root.GetComponentAtPath("amount", out amount);
        root.GetComponentAtPath("Button_View", out view);
        root.GetComponentAtPath("Button_Delete", out delete);
    }
}