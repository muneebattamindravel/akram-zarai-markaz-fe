using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

public class Screen_CompaniesList : OSA<BaseParamsWithPrefab, CompaniesListViewHolder>
{
    public GameObject contentRoot;
    public List<Company> companies;
    public List<ColumnHeader> columnHeaders;

    public SimpleDataHelper<Company> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Company>(this);
        base.Start();
    }

    protected override CompaniesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new CompaniesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(CompaniesListViewHolder newOrRecycled)
    {
        Company company = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = company.id.ToString();
        newOrRecycled.name.text = company.name.ToString();
        newOrRecycled.number.text = company.number.ToString();
        newOrRecycled.description.text = company.description.ToString();

        newOrRecycled.name.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.name.GetComponent<MRButton>().onClicked.AddListener(() => {
            ViewCompany(company.id);
        });

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            ViewCompany(company.id);
        });

        if (company.accountId != null)
        {
            newOrRecycled.viewStatement.gameObject.SetActive(true);
            newOrRecycled.viewStatement.onClicked.RemoveAllListeners();
            newOrRecycled.viewStatement.onClicked.AddListener(() =>
            {
                ViewAccountStatement(company.accountId);
            });
        }
        else
            newOrRecycled.viewStatement.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GetCompanies();
        CompaniesManager.onCompanyAdded += GetCompanies;
        CompaniesManager.onCompanyUpdated += GetCompanies;

        InitializeColumnsHeaders();
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
                FieldInfo fieldInfo = typeof(Company).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    companies = companies.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    companies = companies.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    companies = companies.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) =>
            {
                foreach (Company item in companies) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Company).GetField(header.dataField);
                foreach (Company filtered in companies.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    private void OnDisable()
    {
        CompaniesManager.onCompanyAdded -= GetCompanies;
        CompaniesManager.onCompanyUpdated -= GetCompanies;

        companies = null;
        GC.Collect();
    }

    void GetCompanies()
    {
        Preloader.Instance.ShowWindowed();
        CompaniesManager.Instance.GetCompanies((response) => {
            companies = response.data;
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
        this.Data.InsertItems(0, companies.FindAll(p => p.IsEnabledOnGrid));
        Preloader.Instance.HideWindowed();
    }

    void ViewAccountStatement(int? accountId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.AccountStatement);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_AccountStatement>().ShowView(accountId);
    }

    void ViewCompany(int companyId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Companies_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_CompaniesView_Add>().ShowView(companyId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Companies_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_CompaniesView_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetCompanies();
    }
}

public class CompaniesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, number, description;
    public MRButton edit, viewStatement;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("number", out number);
        root.GetComponentAtPath("description", out description);
        root.GetComponentAtPath("Button_Edit", out edit);
        root.GetComponentAtPath("Button_ViewStatement", out viewStatement);
    }
}

