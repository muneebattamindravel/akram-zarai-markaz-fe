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

public class Screen_CategoriesList : OSA<BaseParamsWithPrefab, CategoriesListViewHolder>
{
    public GameObject contentRoot;
    public List<Category> categories;
    public List<ColumnHeader> columnHeaders;

    public SimpleDataHelper<Category> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Category>(this);
        base.Start();
    }

    protected override CategoriesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new CategoriesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(CategoriesListViewHolder newOrRecycled)
    {
        Category category = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = category.id.ToString();
        newOrRecycled.name.text = category.name.ToString();
        newOrRecycled.description.text = category.description.ToString();

        newOrRecycled.name.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.name.GetComponent<MRButton>().onClicked.AddListener(() => {
            ViewCategory(category.id);
        });

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            ViewCategory(category.id);
        });
    }

    private void OnEnable()
    {
        GetCategories();
        CategoriesManager.onCategoryAdded += GetCategories;
        CategoriesManager.onCategoryUpdated += GetCategories;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        CategoriesManager.onCategoryAdded -= GetCategories;
        CategoriesManager.onCategoryUpdated -= GetCategories;

        categories = null;
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
                FieldInfo fieldInfo = typeof(Category).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    categories = categories.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    categories = categories.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    categories = categories.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Category item in categories) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Category).GetField(header.dataField);
                foreach (Category filtered in categories.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetCategories()
    {
        Preloader.Instance.ShowWindowed();
        CategoriesManager.Instance.GetCategories((response) => {
            categories = response.data;
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
        this.Data.InsertItems(0, categories.FindAll(p => p.IsEnabledOnGrid));
        Preloader.Instance.HideWindowed();
    }

    void ViewCategory(int categoryId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Categories_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_CategoriesView_Add>().ShowView(categoryId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Categories_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_CategoriesView_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetCategories();
    }
}

public class CategoriesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, description;
    public MRButton edit;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("description", out description);
        root.GetComponentAtPath("Button_Edit", out edit);
    }
}
