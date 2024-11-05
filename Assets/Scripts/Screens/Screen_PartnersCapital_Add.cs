using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_PartnersCapital_Add : MonoBehaviour
{
    public TMP_InputField input_amount, input_details;
    public TMP_Dropdown dropdown_partnerAccount, dropdown_creditAccount;
    public MRDatePicker datepicker_date;
    List<Account> accounts;
    Account selectedPartnerAccount, selectedCreditAccount;

    private void OnEnable()
    {
        input_amount.text = "";
        input_details.text = "";

        block = false;

        SetDropdownsToDefaultState();
    }

    void SetDropdownsToDefaultState()
    {
        TMP_Dropdown.OptionData defaultOption = dropdown_partnerAccount.options[0];
        dropdown_partnerAccount.options.Clear();
        dropdown_partnerAccount.options.Add(defaultOption);
        dropdown_partnerAccount.value = 0;

        defaultOption = dropdown_creditAccount.options[0];
        dropdown_creditAccount.options.Clear();
        dropdown_creditAccount.options.Add(defaultOption);
        dropdown_creditAccount.value = 0;
    }

    public void ShowView()
    {
        GetData();
    }

    void GetData()
    {
        Preloader.Instance.ShowFull();
        AccountsManager.Instance.GetAccounts((response) =>
        {
            accounts = response.data;

            List<string> partnerAccountNames = new List<string>();
            List<string> cashAccountNames = new List<string>();

            foreach (Account account in accounts.FindAll(p => p.type == AccountType.Partner.ToString()))
                partnerAccountNames.Add(account.name);
            foreach (Account account in accounts.FindAll(p => p.type == AccountType.Cash.ToString() || p.type == AccountType.Online.ToString()))
                cashAccountNames.Add(account.name);

            dropdown_partnerAccount.AddOptions(partnerAccountNames);
            dropdown_partnerAccount.onValueChanged.AddListener((changedValue) => {
                selectedPartnerAccount = accounts.Find(p => p.name == dropdown_partnerAccount.options[changedValue].text);
            });

            dropdown_creditAccount.AddOptions(cashAccountNames);
            dropdown_creditAccount.onValueChanged.AddListener((changedValue) => {
                selectedCreditAccount = accounts.Find(p => p.name == dropdown_creditAccount.options[changedValue].text);
            });

            Preloader.Instance.HideFull();
        });
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    bool block = false;
    public void Button_SaveClicked()
    {
        if (selectedPartnerAccount == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectParnerAccount, false);
            return;
        }

        if (selectedCreditAccount == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectCreditAccount, false);
            return;
        }

        if (datepicker_date.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
            return;
        }

        if (string.IsNullOrEmpty(input_amount.text) || float.Parse(input_amount.text) <= 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.EnterAmount, false);
            return;
        }

        if (block) return;
        block = true;

        Preloader.Instance.ShowFull();

        PartnerCapitalAddParam capitalAdd = new PartnerCapitalAddParam();
        capitalAdd.amount = float.Parse(input_amount.text);
        capitalAdd.creditAccountId = selectedCreditAccount.id;
        capitalAdd.partnerAccountId = selectedPartnerAccount.id;
        capitalAdd.date = datepicker_date.SelectedDate;
        capitalAdd.details = input_details.text;

        AccountsManager.Instance.AddPartnerCapital(capitalAdd, (response) =>
        {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Success, Constants.CapitalAdded);
            if (AccountsManager.onPartnersListEvent != null) AccountsManager.onPartnersListEvent();
            GUIManager.Instance.Back();
        }, null);
    }
}
