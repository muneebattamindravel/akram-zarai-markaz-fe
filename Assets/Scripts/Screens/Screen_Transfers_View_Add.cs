using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using System;

public class Screen_Transfers_View_Add : MonoBehaviour
{
    public TMP_InputField input_amount, input_notes, input_bookNumber, input_billNumber;
    public TMP_Dropdown dropdown_fromAccount, dropdown_toAccount;
    Transfer transfer;
    int transferId;
    List<Account> accounts;
    ViewMode mode;
    public TMP_Text text_title;
    Account selectedFromAccount, selectedToAccount;
    public MRDatePicker datepicker_date;
    public TMP_Text text_fromAccountBalance;
    public GameObject buttonSave;

    public enum TransferType
    {
        NORMAL_TRANSFER, WITHDRAW_CAPITAL
    }

    TransferType currentTransferType;

    private void OnEnable()
    {
        block = false;

        input_notes.text = "";
        input_amount.text = "";
        input_bookNumber.text = "";
        input_billNumber.text = "";
        text_fromAccountBalance.text = "";
        buttonSave.SetActive(true);

        SetDropdownsToDefaultState();

        KeyboardManager.enterPressed += OnEnterPressed;
    }

    void SetDropdownsToDefaultState()
    {
        TMP_Dropdown.OptionData defaultOption = dropdown_fromAccount.options[0];
        dropdown_fromAccount.options.Clear();
        dropdown_fromAccount.options.Add(defaultOption);
        dropdown_fromAccount.value = 0;

        defaultOption = dropdown_toAccount.options[0];
        dropdown_toAccount.options.Clear();
        dropdown_toAccount.options.Add(defaultOption);
        dropdown_toAccount.value = 0;
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    //bool drawCapital = false;
    public void ShowView(TransferType type = TransferType.NORMAL_TRANSFER)
    {
        mode = ViewMode.ADD;
        GetAccounts();

        currentTransferType = type;

        if (currentTransferType == TransferType.WITHDRAW_CAPITAL)
            text_title.text = Constants.WithdrawCapital;
        else if (currentTransferType == TransferType.NORMAL_TRANSFER)
            text_title.text = Constants.Add + " " + Constants.Transfer;
    }

    public void ShowView(int transferId, bool viewOnly = false)
    {
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            buttonSave.SetActive(false);
            text_title.text = Constants.View + " " + Constants.Transfer;
        }
        else
        {
            mode = ViewMode.EDIT;
            text_title.text = Constants.Edit + " " + Constants.Transfer;
        }
        
        this.transferId = transferId;
        
        GetTransfer();
    }

    void GetTransfer()
    {
        GetAccounts();
    }

    void GetAccounts()
    {
        Preloader.Instance.ShowFull();
        AccountsManager.Instance.GetAccounts((response) =>
        {
            accounts = response.data;
            accounts = accounts.FindAll(p => p.type == AccountType.Cash.ToString() || p.type == AccountType.Online.ToString() || p.type == AccountType.Partner.ToString());

            List<string> accountNames = new List<string>();
            List<string> partnerAccountNames = new List<string>();

            foreach (Account account in accounts)
            {
                if (account.type == AccountType.Partner.ToString())
                    partnerAccountNames.Add(account.name);
                else
                    accountNames.Add(account.name);
            }
                
            dropdown_fromAccount.AddOptions(accountNames);

            if (currentTransferType == TransferType.WITHDRAW_CAPITAL)
                dropdown_toAccount.AddOptions(partnerAccountNames);
            else if (currentTransferType == TransferType.NORMAL_TRANSFER)
                dropdown_toAccount.AddOptions(accountNames);

            dropdown_fromAccount.onValueChanged.AddListener((changedValue) => {
                selectedFromAccount = accounts.Find(p => p.name == dropdown_fromAccount.options[changedValue].text);
                if (selectedFromAccount != null)
                    text_fromAccountBalance.text = selectedFromAccount.balance + Constants.Currency;
            });

            dropdown_toAccount.onValueChanged.AddListener((changedValue) => {
                selectedToAccount = accounts.Find(p => p.name == dropdown_toAccount.options[changedValue].text);
            });

            if (mode == ViewMode.EDIT || mode == ViewMode.VIEW)
            {
                TransfersManager.Instance.GetTransfer(transferId, (result) => {
                    this.transfer = result.data;
                    this.datepicker_date.SelectedDate = this.transfer.date;
                    this.input_amount.text = this.transfer.amount.ToString();
                    this.input_notes.text = this.transfer.notes;
                    this.input_bookNumber.text = this.transfer.bookNumber;
                    this.input_billNumber.text = this.transfer.billNumber;

                    dropdown_fromAccount.value = dropdown_fromAccount.options.FindIndex(p => p.text == (accounts.Find(p => p.id == transfer.fromAccountId)).name);
                    dropdown_toAccount.value = dropdown_toAccount.options.FindIndex(p => p.text == (accounts.Find(p => p.id == transfer.toAccountId)).name);

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
        if (IsTransferBodyValid())
        {
            if (block) return;
            block = true;

            Preloader.Instance.ShowFull();
            if (mode == ViewMode.ADD)
            {
                Transfer transfer = new Transfer();
                transfer.date = datepicker_date.SelectedDate;
                transfer.notes = input_notes.text;
                transfer.bookNumber = input_bookNumber.text;
                transfer.billNumber = input_billNumber.text;
                transfer.amount = float.Parse(input_amount.text);
                transfer.fromAccountId = selectedFromAccount.id;
                transfer.toAccountId = selectedToAccount.id;

                if (currentTransferType == TransferType.WITHDRAW_CAPITAL)
                    transfer.details = Constants.Transfer_Capital_Withdrawn;
                else if (currentTransferType == TransferType.NORMAL_TRANSFER)
                    transfer.details = Constants.Transfer_Normal;

                TransfersManager.Instance.AddTransfer(transfer,
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    if (currentTransferType == TransferType.WITHDRAW_CAPITAL)
                    {
                        GUIManager.Instance.ShowToast(Constants.Success, Constants.CapitalWithdrawn);
                        if (AccountsManager.onPartnersListEvent != null) AccountsManager.onPartnersListEvent();
                    }
                    else if (currentTransferType == TransferType.NORMAL_TRANSFER)
                    {
                        GUIManager.Instance.ShowToast(Constants.Success, Constants.TransferAdded);
                        if (TransfersManager.onTransferAdded != null) TransfersManager.onTransferAdded();
                    }

                    GUIManager.Instance.Back();
                },
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
            }
            else if (mode == ViewMode.EDIT)
            {
                transfer.date = datepicker_date.SelectedDate;
                transfer.notes = input_notes.text;
                transfer.bookNumber = input_bookNumber.text;
                transfer.billNumber = input_billNumber.text;
                transfer.amount = float.Parse(input_amount.text);
                transfer.fromAccountId = selectedFromAccount.id;
                transfer.toAccountId = selectedToAccount.id;

                TransfersManager.Instance.UpdateTransfer(transfer, transfer.id, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.TransferUpdated);
                    if (TransfersManager.onTransferAdded != null) TransfersManager.onTransferAdded();
                    GUIManager.Instance.Back();

                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
            }
        }
    }

    bool IsTransferBodyValid()
    {
        if (selectedFromAccount == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectFromAccount, false);
            return false;
        }

        if (selectedToAccount == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectToAccount, false);
            return false;
        }

        if (string.IsNullOrEmpty(input_amount.text) || float.Parse(input_amount.text) <= 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.TransferAmountEmpty, false);
            return false;
        }

        if (currentTransferType == TransferType.WITHDRAW_CAPITAL && selectedToAccount.balance < float.Parse(input_amount.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughInCapitalAccount, false);
            return false;
        }

        if (selectedFromAccount.id == selectedToAccount.id) {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.BothAccountsAreSame, false);
            return false;
        }

        if ( datepicker_date.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.EnterTransferDate, false);
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

        if (selectedFromAccount.balance < (float.Parse(input_amount.text)))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughBalance, false);
            return false;
        }

        return true;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
