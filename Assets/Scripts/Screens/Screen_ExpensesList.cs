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

public class Screen_ExpensesList : OSA<BaseParamsWithPrefab, ExpensesListViewHolder>
{
    public GameObject contentRoot;
    public List<Expense> expenses;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;

    public TMP_Text text_totalExpenses;
    public SimpleDataHelper<Expense> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Expense>(this);
        base.Start();
    }

    protected override ExpensesListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new ExpensesListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(ExpensesListViewHolder newOrRecycled)
    {
        Expense expense = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = expense.id.ToString();
        newOrRecycled.date.text = expense.date.ToString(Constants.DateDisplayFormat);
        newOrRecycled.type.text = ((ExpenseType)expense.type).ToString();
        newOrRecycled.bookNumber.text = expense.bookNumber.ToString();
        newOrRecycled.billNumber.text = expense.billNumber.ToString();
        newOrRecycled.amount.text = expense.amount.ToCommaSeparatedNumbers();
        newOrRecycled.description.text = expense.description.ToString();

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            EditExpense(expense.id);
        });
    }

    private void OnEnable()
    {
        GetExpenses();

        ExpensesManager.onExpenseAdded += GetExpenses;
        dateFilterPicker.onDateSelected += GetExpenses;

        dateFilterPicker.gameObject.SetActive(true);
        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        ExpensesManager.onExpenseAdded -= GetExpenses;
        this.dateFilterPicker.onDateSelected -= GetExpenses;

        expenses = null;
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
                FieldInfo fieldInfo = typeof(Expense).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    expenses = expenses.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    expenses = expenses.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    expenses = expenses.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Expense item in expenses) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Expense).GetField(header.dataField);
                foreach (Expense filtered in expenses.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetExpenses()
    {
        Preloader.Instance.ShowWindowed();
        ExpensesManager.Instance.GetExpenses(dateFilterPicker.GetDateRange(), (response) => {
            expenses = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        }, null);
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        float totalExpensesAmount = 0f;
        foreach (Expense e in expenses)
            totalExpensesAmount += e.amount;
        text_totalExpenses.text = totalExpensesAmount.ToCommaSeparatedNumbers();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, expenses.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    void EditExpense(int expenseId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Expenses_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Expenses_View_Add>().ShowView(expenseId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Expenses_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Expenses_View_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetExpenses();
    }
}

public class ExpensesListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, date, type, bookNumber, billNumber, amount, description;
    public MRButton edit;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("date", out date);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
        root.GetComponentAtPath("amount", out amount);
        root.GetComponentAtPath("description", out description);
        root.GetComponentAtPath("Button_Edit", out edit);
    }
}