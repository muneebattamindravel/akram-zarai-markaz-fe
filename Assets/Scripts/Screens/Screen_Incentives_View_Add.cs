using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_Incentives_View_Add : MonoBehaviour
{
    public TMP_InputField input_amount, input_notes;
    public TMP_Dropdown dropdown_company, dropdown_type;
    public MRDatePicker datepicker_date;  
    List<Company> companies;

    ViewMode mode;
    Incentive incentive;
    int incentiveId;

    private void OnEnable()
    {
        block = false;
        input_amount.text = "";
        input_notes.text = "";
        dropdown_company.options.Clear();

        GetData();
    }

    public void ShowView(int incentiveId, bool viewOnly = false)
    {
        this.incentiveId = incentiveId;
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

    public void ShowView()
    {
    }

    void GetData()
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies((response) => {
            companies = response.data;
            List<string> companyNames = new List<string>();
            foreach (Company company in companies) companyNames.Add(company.name);
            dropdown_company.AddOptions(companyNames);

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
        if (datepicker_date.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.DateEmpty, false);
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
        Incentive incentive = new Incentive();
        incentive.amount = float.Parse(input_amount.text);
        incentive.notes = input_notes.text;
        incentive.date = datepicker_date.SelectedDate;
        incentive.type = dropdown_type.options[dropdown_type.value].text;
        incentive.companyId = companies.Find(p => p.name == dropdown_company.options[dropdown_company.value].text).id;

        IncentivesManager.Instance.CreateIncentive(incentive, (response) => {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Success, Constants.IncentiveAdded);
            if (IncentivesManager.onIncentiveAdded != null) IncentivesManager.onIncentiveAdded();
            GUIManager.Instance.Back();
        }, null);
    }
}
