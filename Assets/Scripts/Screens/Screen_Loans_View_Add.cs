using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_Loans_View_Add : MonoBehaviour
{
    List<Account> accounts;
    List<string> accountStrings;

    public TMP_InputField input_bookNumber, input_billNumber, input_amount;
    public TMP_Dropdown dropdown_account;
    public MRDatePicker datepicker_date;
    public TMP_Text text_customerName;
    public TMP_Text text_closingBalance;
    public TMP_Text text_title;
        
    Contact selectedCustomer = null;
    List<Contact> customers;
    public MRDroplist droplist_customers;

    ViewMode mode;
    Loan loan;
    int loanId;
    bool isReceiving = false;

    private void OnEnable()
    {
        block = false;

        input_billNumber.text = "";
        input_bookNumber.text = "";
        input_amount.text = "";
        text_customerName.text = "";
        text_closingBalance.text = "";

        GetData();
    }

    public void ShowView(int loanId, bool viewOnly = false)
    {
        this.loanId = loanId;
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
        }
        else
        {
            mode = ViewMode.EDIT;
        }

        GetData();
    }

    public void ShowView(bool isReceiving)
    {
        this.isReceiving = isReceiving;
        if (isReceiving)
            text_title.text = Constants.TakeLoan;
        else
            text_title.text = Constants.GiveLoan;
    }

    void GetData()
    {
        Preloader.Instance.ShowFull();
        AccountsManager.Instance.GetAccounts(
            (response) =>
            {
                accounts = response.data;
                accounts = accounts.FindAll(p => p.type == AccountType.Online.ToString() || p.type == AccountType.Cash.ToString());
                accountStrings = new List<string>();
                foreach (Account account in accounts) accountStrings.Add(account.name + "\n" + account.bankName + " - " + account.bankAccountNumber);
                dropdown_account.ClearOptions();
                dropdown_account.AddOptions(accountStrings);

                ContactsManager.Instance.GetAllCustomers(
                    (response) =>
                    {
                        customers = response.data;
                        List<string> customerNames = new List<string>();
                        customerNames.Add(Constants.None);
                        foreach (Contact contact in customers) customerNames.Add(contact.name);

                        CustomersDropList();

                        if (mode == ViewMode.VIEW || mode == ViewMode.EDIT)
                        {
                            LoansManager.Instance.GetLoan
                            (this.loanId, (response) => {

                                this.loan = response.data;

                                //input_billNumber.text = loan.billNumber;
                                //input_bookNumber.text = loan.bookNumber;
                                //input_paymentReceived.text = loan.amount.ToString();
                                //datepicker_receivedDate.SelectedDate = loan.date;
                                //text_customerName.text = loan.contact.name;

                                //dropdown_paymentType.value = dropdown_paymentType.options.FindIndex(p => p.text == loan.paymentType.ToString());
                                //dropdown_onlineAccount.value = dropdown_onlineAccount.options.FindIndex(p => p.text == (onlineAccounts.Find(p => p.id == loan.accountId)).name);

                            }, null);
                        }

                        Preloader.Instance.HideFull();
                    },
                    (response) =>
                    {
                        GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                    }
                );
            },
            (response) =>
            {
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            }
        );
    }

    void CustomersDropList()
    {
        droplist_customers.onPopulateItems.AddListener(() =>
        {
            droplist_customers.ClearContentDroplist();
            foreach (Contact contact in customers.FindAll(p => p.IsEnabledOnGrid))
            {
                GameObject item = droplist_customers.GetPooledItem();
                if (item != null)
                {
                    item.transform.Find("Text_Name").GetComponent<TMP_Text>().text = contact.name;
                    item.transform.Find("Text_Label_Phone").GetComponent<TMP_Text>().text = contact.number;

                    item.GetComponent<MRButton>().onClicked.RemoveAllListeners();
                    item.GetComponent<MRButton>().onClicked.AddListener(() =>
                    {
                        SelectCustomer(contact);
                    });
                    item.SetActive(true);
                }
                else
                {
                    GUIManager.Instance.ShowToast(Constants.Error, Constants.PoolError, false);
                }
            }
        });

        droplist_customers.onInputEndEdit.AddListener((string inputFieldText) =>
        {
            foreach (Contact customer in customers) customer.IsEnabledOnGrid = true;
            foreach (Contact filtered in customers.FindAll(p => !p.name.ToLower().Contains(inputFieldText)))
                filtered.IsEnabledOnGrid = false;

            droplist_customers.PopulateItems();
        });
    }

    Account customerAccount;
    public void SelectCustomer(Contact customer)
    {
        selectedCustomer = customer;
        text_customerName.text = selectedCustomer.name;
        droplist_customers.InputDeSelected();

        AccountsManager.Instance.GetAccount(customer.accountId, (result) => {
            customerAccount = result.data;
            text_closingBalance.text = customerAccount.balance + Constants.Currency;
        }, (res) => { });
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    public bool block = false;
    public void Button_ConfirmClicked()
    {
        

        if (selectedCustomer == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.MustSelectCustomer, false);
            return;
        }

        if ( datepicker_date.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
            return;
        }

        if (string.IsNullOrEmpty(input_bookNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterBookNumber, false);
            return;
        }

        if (string.IsNullOrEmpty(input_billNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterBillNumber, false);
            return;
        }

        if (string.IsNullOrEmpty(input_amount.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterAmount, false);
            return;
        }

        if (block) return;
        block = true;

        Preloader.Instance.ShowFull();
        Loan loan = new Loan();
        loan.amount = float.Parse(input_amount.text);
        loan.billNumber = input_billNumber.text;
        loan.bookNumber = input_bookNumber.text;
        loan.date = datepicker_date.SelectedDate;
        loan.contactId = selectedCustomer.id;
        loan.isReceived = this.isReceiving;
        loan.accountId =
            accounts[dropdown_account.options.
            FindIndex(p => p.text == dropdown_account.options[dropdown_account.value].text)].id;

        LoansManager.Instance.CreateLoan(loan, (response) => {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Success, Constants.LoanAdded);
            if (LoansManager.onLoanAdded != null) LoansManager.onLoanAdded();
            GUIManager.Instance.Back();
        }, null);
    }
}
