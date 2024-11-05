using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using System;

public class Screen_Expenses_View_Add : MonoBehaviour
{
    public TMP_InputField input_amount, input_description, input_bookNumber, input_billNumber;
    public TMP_Dropdown dropdown_account, dropdown_type;
    Expense expense;
    int expenseId;
    List<Account> accounts;
    ViewMode mode;
    public TMP_Text text_title;
    Account selectedAccount;
    public MRDatePicker datepicker_date;
    public GameObject buttonSave;

    private void OnEnable()
    {
        block = false;
        input_description.text = "";
        input_amount.text = "";
        input_bookNumber.text = "";
        input_billNumber.text = "";
        buttonSave.SetActive(true);

        dropdown_account.interactable = true;
        dropdown_type.interactable = true;
        input_amount.interactable = true;
        input_billNumber.interactable = true;
        input_bookNumber.interactable = true;
        input_description.interactable = true;

        TMP_Dropdown.OptionData defaultOption = dropdown_account.options[0];
        dropdown_account.options.Clear();
        dropdown_account.options.Add(defaultOption);
        dropdown_account.value = 0;


        KeyboardManager.enterPressed += OnEnterPressed;
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void ShowView()
    {
        mode = ViewMode.ADD;
        GetAccounts();
        text_title.text = Constants.Add + " " + Constants.Expense;
    }

    public void ShowView(int expenseId, bool viewOnly = false)
    {
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            buttonSave.SetActive(false);
            text_title.text = Constants.View + " " + Constants.Expense;

            dropdown_account.interactable = false;
            dropdown_type.interactable = false;
            input_amount.interactable = false;
            input_billNumber.interactable = false;
            input_bookNumber.interactable = false;
            input_description.interactable = false;
        }
        else
        {
            mode = ViewMode.EDIT;
            text_title.text = Constants.Edit + " " + Constants.Expense;
        }
        
        this.expenseId = expenseId;
        
        GetExpense();
    }

    void GetExpense()
    {
        GetAccounts();
    }

    void GetAccounts()
    {
        Preloader.Instance.ShowFull();
        AccountsManager.Instance.GetAccounts((response) =>
        {
            accounts = response.data;
            accounts = accounts.FindAll(p => p.type == AccountType.Cash.ToString() || p.type == AccountType.Online.ToString());
            List<string> accountNames = new List<string>();
            foreach (Account account in accounts)
                accountNames.Add(account.name);
            dropdown_account.AddOptions(accountNames);
            dropdown_account.onValueChanged.AddListener((changedValue) => {
                selectedAccount = accounts.Find(p => p.name == dropdown_account.options[changedValue].text);
            });

            if (mode == ViewMode.EDIT || mode == ViewMode.VIEW)
            {
                ExpensesManager.Instance.GetExpense(expenseId, (result) => {
                    this.expense = result.data;
                    this.datepicker_date.SelectedDate = this.expense.date;
                    this.input_amount.text = this.expense.amount.ToString();
                    this.input_description.text = this.expense.description;
                    this.input_bookNumber.text = this.expense.bookNumber;
                    this.input_billNumber.text = this.expense.billNumber;

                    dropdown_type.value = dropdown_type.options.FindIndex(p => p.text == expense.type.ToString());
                    dropdown_account.value = dropdown_account.options.FindIndex(p => p.text == (accounts.Find(p => p.id == expense.accountId)).name);

                }, null);
            }

            Preloader.Instance.HideFull();
        });
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public bool block = false;
    public void Button_SaveClicked()
    {
        if (IsExpenseBodyValid())
        {
            if (block) return;
            block = true;

            Preloader.Instance.ShowFull();
            if (mode == ViewMode.ADD)
            {
                Expense expense = new Expense();
                expense.date = datepicker_date.SelectedDate;
                expense.type = (ExpenseType) (dropdown_type.value - 1);
                expense.description = input_description.text;
                expense.bookNumber = input_bookNumber.text;
                expense.billNumber = input_billNumber.text;
                expense.amount = float.Parse(input_amount.text);
                expense.accountId = selectedAccount.id;
                
                ExpensesManager.Instance.AddExpense(expense,
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.ExpenseAdded);
                    if (ExpensesManager.onExpenseAdded != null) ExpensesManager.onExpenseAdded();
                    GUIManager.Instance.Back();
                },
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
            }
            else if (mode == ViewMode.EDIT)
            {
                expense.date = datepicker_date.SelectedDate;
                expense.type = (ExpenseType)(dropdown_type.value - 1);
                expense.description = input_description.text;
                expense.bookNumber = input_bookNumber.text;
                expense.billNumber = input_billNumber.text;
                expense.amount = float.Parse(input_amount.text);
                expense.accountId = selectedAccount.id;

                ExpensesManager.Instance.UpdateExpense(expense, expense.id, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.ExpenseUpdated);
                    if (ExpensesManager.onExpenseAdded != null) ExpensesManager.onExpenseAdded();
                    GUIManager.Instance.Back();

                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
            }
        }
    }

    bool IsExpenseBodyValid()
    {
        if (selectedAccount == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectFromAccount, false);
            return false;
        }

        if (datepicker_date.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.EnterExpenseDate, false);
            return false;
        }

        if (string.IsNullOrEmpty(input_amount.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ExpenseAmountEmpty, false);
            return false;
        }

        if (string.IsNullOrEmpty(input_bookNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.EnterBookNumber, false);
            return false;
        }

        if (string.IsNullOrEmpty(input_billNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.EnterBillNumber, false);
            return false;
        }

        if (dropdown_type.value == 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectExpenseType, false);
            return false;
        }

        return true;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
