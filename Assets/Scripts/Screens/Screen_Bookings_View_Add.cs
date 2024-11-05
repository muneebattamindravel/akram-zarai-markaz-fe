using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_Bookings_View_Add : MonoBehaviour
{
    public TMP_InputField input_prNumber, input_totalAmount, input_notes, input_policyName, input_policyPercentage, input_netRate;
    public MRDatePicker datepicker_bookingDate;
    public TMP_Dropdown dropdown_company, dropdown_fromAccount;
    public TMP_Dropdown dropdown_policyType, dropdown_bookingType;

    public TMP_Text text_title;

    List<Account> accounts;
    List<Company> companies;

    Company selectedCompany;
    Account fromAccount;

    ViewMode mode;
    Booking booking;
    int bookingId;

    public GameObject buttonSave;

    private void OnEnable()
    {
        block = false;

        input_prNumber.text = "";
        input_notes.text = "";
        selectedCompany = null;
        input_totalAmount.text = "";
        input_policyName.text = "";
        input_policyPercentage.text = "";
        input_netRate.text = "";
        buttonSave.SetActive(true);

        input_policyPercentage.gameObject.SetActive(true);
        input_netRate.gameObject.SetActive(false);


        TMP_Dropdown.OptionData defaultOption = dropdown_fromAccount.options[0];
        dropdown_fromAccount.options.Clear();
        dropdown_fromAccount.options.Add(defaultOption);
        dropdown_fromAccount.value = 0;

        defaultOption = dropdown_company.options[0];
        dropdown_company.options.Clear();
        dropdown_company.options.Add(defaultOption);
        dropdown_company.value = 0;

        KeyboardManager.enterPressed += OnEnterPressed;

        dropdown_policyType.onValueChanged.AddListener((value) => {
            input_netRate.gameObject.SetActive(false);
            input_policyPercentage.gameObject.SetActive(false);

            if (value == 0)
                input_policyPercentage.gameObject.SetActive(true);
            else
                input_netRate.gameObject.SetActive(true);
        });

        dropdown_policyType.value = 0;
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public void ShowView()
    {
        mode = ViewMode.ADD;
        text_title.text = Constants.Add + " " + Constants.BOOKING;

        dropdown_company.interactable = true;
        dropdown_fromAccount.interactable = true;
        input_prNumber.interactable = true;
        input_totalAmount.interactable = true;
        input_notes.interactable = true;
        dropdown_bookingType.interactable = true;
        dropdown_policyType.interactable = true;
        input_policyName.interactable = true;
        input_netRate.interactable = true;
        input_policyPercentage.interactable = true;

        GetAccountsAndCompanies();
    }

    public void ShowView(int bookingId, bool viewOnly = false)
    {
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            buttonSave.SetActive(false);
            text_title.text = Constants.View + " " + Constants.BOOKING;
        }
        else
        {
            mode = ViewMode.EDIT;
            text_title.text = Constants.Edit + " " + Constants.BOOKING;
        }
        
        this.bookingId = bookingId;
        GetAccountsAndCompanies();
    }

    void GetAccountsAndCompanies()
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies(
            (response) =>
            {
                companies = response.data;
                AccountsManager.Instance.GetAccounts(
                    (response) =>
                    {
                        accounts = response.data;
                        accounts = accounts.FindAll(p => p.type == AccountType.Cash.ToString() || p.type == AccountType.Online.ToString());

                        List<string> companyNames = new List<string>();
                        List<string> accountNames = new List<string>();

                        foreach (Company company in companies)
                            companyNames.Add(company.name);
                        foreach (Account account in accounts)
                            accountNames.Add(account.name);

                        dropdown_company.AddOptions(companyNames);
                        dropdown_company.onValueChanged.AddListener((changedValue) => {
                            selectedCompany = companies.Find(p => p.name == dropdown_company.options[changedValue].text);
                        });

                        dropdown_fromAccount.AddOptions(accountNames);
                        dropdown_fromAccount.onValueChanged.AddListener((changedValue) => {
                            fromAccount = accounts.Find(p => p.name == dropdown_fromAccount.options[changedValue].text);
                        });

                        if (mode == ViewMode.EDIT || mode == ViewMode.VIEW)
                        {
                            BookingsManager.Instance.GetBooking(bookingId, (response) => {
                                this.booking = response.data;

                                dropdown_company.interactable = false;
                                dropdown_fromAccount.interactable = false;
                                input_prNumber.interactable = false;
                                input_totalAmount.interactable = false;
                                input_notes.interactable = false;
                                dropdown_bookingType.interactable = false;
                                dropdown_policyType.interactable = false;
                                input_policyName.interactable = false;
                                input_netRate.interactable = false;
                                input_policyPercentage.interactable = false;

                                dropdown_company.value = dropdown_company.options.FindIndex(p => p.text == booking.company.name);
                                dropdown_fromAccount.value = dropdown_fromAccount.options.FindIndex(p => p.text == (accounts.Find(p => p.id == booking.fromAccountId)).name);
                                input_prNumber.text = booking.prNumber;
                                input_totalAmount.text = booking.totalAmount.ToString();
                                datepicker_bookingDate.SelectedDate = booking.bookingDate;
                                input_notes.text = booking.notes;
                                input_policyName.text = booking.policyName;
                                input_policyPercentage.text = booking.policyPercentage;
                                input_netRate.text = booking.netRate;
                                dropdown_bookingType.value = dropdown_bookingType.options.FindIndex(p => p.text == booking.bookingType);
                                dropdown_policyType.value = dropdown_policyType.options.FindIndex(p => p.text == booking.policyType);

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

    public bool block = false;
    public void Button_SaveClicked()
    {
        if (selectedCompany == null)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectCompanyForBooking, false);
            return;
        }
        if (dropdown_fromAccount.value == 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectFromAccount, false);
            return;
        }
        if (string.IsNullOrEmpty(input_prNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.PrNumberEmpty, false);
            return;
        }
        if (string.IsNullOrEmpty(input_totalAmount.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.TotalAmountZero, false);
            return;
        }
        if (datepicker_bookingDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.BookingDateEmpty, false);
            return;
        }
        if (string.IsNullOrEmpty(input_policyName.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.PolicyNameEmpty, false);
            return;
        }

        if (dropdown_policyType.value == 0)
        {
            if (string.IsNullOrEmpty(input_policyPercentage.text))
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.PolicyPercentageEmpty, false);
                return;
            }
        }
        else if (dropdown_policyType.value == 1)
        {
            if (string.IsNullOrEmpty(input_netRate.text))
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.NetRateEmpty, false);
                return;
            }
        }

        if (block) return;
        block = true;

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            Booking booking = new Booking();
            booking.bookingDate = datepicker_bookingDate.SelectedDate;
            booking.prNumber = input_prNumber.text;
            booking.companyId = selectedCompany.id;
            booking.notes = input_notes.text;
            booking.totalAmount = float.Parse(input_totalAmount.text);
            booking.fromAccountId = fromAccount.id;
            booking.policyName = input_policyName.text;
            booking.policyPercentage = input_policyPercentage.text;
            booking.netRate = input_netRate.text;
            booking.bookingType = dropdown_bookingType.options[dropdown_bookingType.value].text;
            booking.policyType = dropdown_policyType.options[dropdown_policyType.value].text;

            BookingsManager.Instance.AddBooking(booking,
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.BookingAdded);
                    if (BookingsManager.onBookingAdded != null) BookingsManager.onBookingAdded();
                    GUIManager.Instance.Back();
                },
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                }
            );
        }
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
