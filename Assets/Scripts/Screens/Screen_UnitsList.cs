using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

public class Screen_UnitsList : OSA<BaseParamsWithPrefab, UnitsListViewHolder>
{
    public GameObject contentRoot;
    public List<Unit> units;
    public List<ColumnHeader> columnHeaders;

    public SimpleDataHelper<Unit> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Unit>(this);
        base.Start();
    }

    protected override UnitsListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new UnitsListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(UnitsListViewHolder newOrRecycled)
    {
        Unit unit = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = unit.id.ToString();
        newOrRecycled.name.text = unit.name.ToString();
        newOrRecycled.description.text = unit.description.ToString();

        newOrRecycled.name.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.name.GetComponent<MRButton>().onClicked.AddListener(() => {
            ViewUnit(unit.id);
        });

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            ViewUnit(unit.id);
        });

        newOrRecycled.allowDecimal.isOn = unit.allowDecimal;
    }

    private void OnEnable()
    {
        GetUnits();
        UnitsManager.onUnitAdded += GetUnits;
        UnitsManager.onUnitUpdated += GetUnits;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        UnitsManager.onUnitAdded -= GetUnits;
        UnitsManager.onUnitUpdated -= GetUnits;

        units = null;
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
                FieldInfo fieldInfo = typeof(Unit).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    units = units.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    units = units.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    units = units.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Unit item in units) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Unit).GetField(header.dataField);
                foreach (Unit filtered in units.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetUnits()
    {
        Preloader.Instance.ShowWindowed();
        UnitsManager.Instance.GetUnits((response) => {
            units = response.data;
            columnHeaders[0].ResetState();
            columnHeaders[0].SetNextState();
            PopulateData();
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();
        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, units.FindAll(p => p.IsEnabledOnGrid));
        Preloader.Instance.HideWindowed();
    }

    void ViewUnit(int unitId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Units_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Units_View_Add>().ShowView(unitId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Units_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Units_View_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetUnits();
    }
}

public class UnitsListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, description;
    public MRButton edit;
    public Toggle allowDecimal;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("description", out description);
        root.GetComponentAtPath("Button_Edit", out edit);
        root.GetComponentAtPath("Toggle_AllowDecimal", out allowDecimal);
    }
}

