using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_PartnersProfit_Add : MonoBehaviour
{
    public TMP_InputField input_amount, input_details;
    public TMP_Dropdown dropdown_partnerAccount;
    public MRDatePicker datepicker_date;
    List<Account> accounts;
    Account selectedPartnerAccount;

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

            foreach (Account account in accounts.FindAll(p => p.type == AccountType.Partner.ToString()))
                partnerAccountNames.Add(account.name);

            dropdown_partnerAccount.AddOptions(partnerAccountNames);
            dropdown_partnerAccount.onValueChanged.AddListener((changedValue) => {
                selectedPartnerAccount = accounts.Find(p => p.name == dropdown_partnerAccount.options[changedValue].text);
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

        if ( datepicker_date.SelectedDate == DateTime.MinValue)
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

        PartnerProfitAddParam profitAdd = new PartnerProfitAddParam();
        profitAdd.amount = float.Parse(input_amount.text);
        profitAdd.partnerAccountId = selectedPartnerAccount.id;
        profitAdd.date = datepicker_date.SelectedDate;
        profitAdd.details = input_details.text;

        AccountsManager.Instance.AddPartnerProfit(profitAdd, (response) =>
        {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Success, Constants.ProfitPosted);
            if (AccountsManager.onPartnersListEvent != null) AccountsManager.onPartnersListEvent();
            GUIManager.Instance.Back();
        }, null);
    }
}
