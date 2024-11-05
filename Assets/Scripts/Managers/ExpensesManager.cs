using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ExpensesManager : MonoBehaviour
{
    public static ExpensesManager Instance;
    public delegate void ExpensesEvent();
    public static ExpensesEvent onExpenseAdded, onExpenseUpdated, onExpenseDeleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string EXPENSES_ROUTE = "expenses";

    public void AddExpense(Expense expense, ResponseAction<Expense> successAction, ResponseAction<Expense> failAction = null)
    {
        APIManager.Instance.Post<Expense>(EXPENSES_ROUTE, expense, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetExpenses(MRDateRange range, ResponseAction <List<Expense>> successAction, ResponseAction<List<Expense>> failAction = null)
    {
        Debug.Log("Getting Expenses From " + range.from + " " + range.to);
        APIManager.Instance.Get<List<Expense>>(EXPENSES_ROUTE + "?from="+range.from+"&to="+range.to, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetExpense(int expenseId, ResponseAction<Expense> successAction, ResponseAction<Expense> failAction = null)
    {
        APIManager.Instance.Get<Expense>(EXPENSES_ROUTE + "/" + expenseId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateExpense(Expense expense, int expenseId, ResponseAction<Expense> successAction, ResponseAction<Expense> failAction = null)
    {
        APIManager.Instance.Patch<Expense>(EXPENSES_ROUTE + "/" + expenseId, expense, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
