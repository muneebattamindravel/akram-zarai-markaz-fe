using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Screen_Accounts_View_Add : MonoBehaviour
{
    public TMP_InputField input_name, input_openingBalance, input_description, input_bankName, input_bankAccountNumber;
    public TMP_Dropdown dropdown_type, dropdown_company;
    Product product;
    List<Company> companies;
    List<Category> categories;
    List<Unit> units;
    ViewMode mode;
    Account account;
    public TMP_Text text_title;

    public GameObject buttonSave;

    private void OnEnable()
    {
        input_name.text = "";
        input_description.text = "";
        input_bankName.text = "";
        input_bankAccountNumber.text = "";
        input_openingBalance.text = "";
        buttonSave.SetActive(true);

        KeyboardManager.enterPressed += OnEnterPressed;

        dropdown_type.onValueChanged.AddListener((int value) => {

            dropdown_company.gameObject.SetActive(false);
            input_bankName.gameObject.SetActive(false);
            input_bankAccountNumber.gameObject.SetActive(false);

            if (dropdown_type.options[value].text == AccountType.Company.ToString())
                dropdown_company.gameObject.SetActive(true);
            else if (dropdown_type.options[value].text == AccountType.Online.ToString())
            {
                input_bankName.gameObject.SetActive(true);
                input_bankAccountNumber.gameObject.SetActive(true);
            }
        });
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void ShowView()
    {
        dropdown_company.enabled = true;
        dropdown_type.enabled = true;
        input_description.enabled = true;
        input_name.enabled = true;
        input_openingBalance.enabled = true;
        input_openingBalance.text = "";

        dropdown_type.options.Clear();

        dropdown_type.options.Add(new TMP_Dropdown.OptionData("Select Type"));
        dropdown_type.options.Add(new TMP_Dropdown.OptionData(AccountType.Cash.ToString()));
        dropdown_type.options.Add(new TMP_Dropdown.OptionData(AccountType.Online.ToString()));


        dropdown_type.value = 0;
        dropdown_type.onValueChanged.Invoke(0);

        mode = ViewMode.ADD;
        GetCompanies();
        text_title.text = Constants.Add + " " + Constants.Account;
    }

    public void ShowView(int accountId, bool viewOnly = false)
    {
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            buttonSave.SetActive(false);
            text_title.text = Constants.View + " " + Constants.Account;

            dropdown_company.enabled = false;
            dropdown_type.enabled = false;
            input_description.enabled = false;
            input_name.enabled = false;
            input_openingBalance.enabled = false;
        }
        else
        {
            mode = ViewMode.EDIT;
            text_title.text = Constants.Edit + " " + Constants.Account;

            dropdown_company.enabled = false;
            dropdown_type.enabled = false;
            input_description.enabled = true;
            input_name.enabled = true;
            input_openingBalance.enabled = false;
        }

        dropdown_type.options.Clear();

        dropdown_type.options.Add(new TMP_Dropdown.OptionData("Select Type"));
        dropdown_type.options.Add(new TMP_Dropdown.OptionData(AccountType.Cash.ToString()));
        dropdown_type.options.Add(new TMP_Dropdown.OptionData(AccountType.Online.ToString()));
        dropdown_type.options.Add(new TMP_Dropdown.OptionData(AccountType.Company.ToString()));
        dropdown_type.options.Add(new TMP_Dropdown.OptionData(AccountType.Customer.ToString()));

        dropdown_type.value = 0;
        GetAccount(accountId);
    }

    void GetCompanies()
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies((response) =>
        {
            companies = response.data;
            List<string> companyNames = new List<string>();
            foreach (Company company in companies) companyNames.Add(company.name);
            dropdown_company.AddOptions(companyNames);
            Preloader.Instance.HideFull();
        });
    }

    void GetAccount(int accountId)
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies((response) =>
        {
            companies = response.data;
            List<string> companyNames = new List<string>();
            foreach (Company company in companies) companyNames.Add(company.name);
            dropdown_company.AddOptions(companyNames);

            AccountsManager.Instance.GetAccount(accountId, (response) =>
            {
                account = response.data;

                input_bankAccountNumber.text = response.data.bankAccountNumber;
                input_bankName.text = response.data.bankName;
                input_description.text = response.data.description;
                input_name.text = response.data.name;
                input_openingBalance.text = response.data.openingBalance.ToString();

                dropdown_type.value = dropdown_type.options.FindIndex(p => p.text == response.data.type.ToString());
                Preloader.Instance.HideFull();

            }, (response) => {
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            });
        });
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public void Button_SaveClicked()
    {
        if (IsAccountBodyValid())
        {
            Preloader.Instance.ShowFull();
            if (mode == ViewMode.ADD)
            {
                Account account = new Account();
                account.name = input_name.text;
                account.type = dropdown_type.options[dropdown_type.value].text;
                account.openingBalance = float.Parse(input_openingBalance.text);
                account.description = input_description.text;
                account.bankAccountNumber = input_bankAccountNumber.text;
                account.bankName = input_bankName.text;
                account.referenceId = 0;
                
                AccountsManager.Instance.AddAccount(account,
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.AccountAdded);
                    if (AccountsManager.onAccountAdded != null) AccountsManager.onAccountAdded();
                    GUIManager.Instance.Back();
                },
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
            }
            else if (mode == ViewMode.EDIT)
            {
                account.name = input_name.text;
                account.description = input_description.text;
                account.bankAccountNumber = input_bankAccountNumber.text;
                account.bankName = input_bankName.text;
                account.openingBalance = float.Parse(input_openingBalance.text);

                AccountsManager.Instance.UpdateAccount(account, account.id, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.AccountUpdated);
                    if (AccountsManager.onAccountUpdated != null) AccountsManager.onAccountUpdated();
                    GUIManager.Instance.Back();
                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                });
            }
        }
    }

    bool IsAccountBodyValid()
    {
        if (string.IsNullOrEmpty(input_name.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.AccountNameEmpty, false);
            return false;
        }

        if (dropdown_type.value == 0)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectAccountType, false);
            return false;
        }

        if (string.IsNullOrEmpty(input_openingBalance.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.OpeningBalanceZero, false);
            return false;
        }

        if (dropdown_type.options[dropdown_type.value].text.ToUpper() == AccountType.Company.ToString())
        {
            if (dropdown_company.value == 0)
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.SelectCompanyForCompanyTypeAccount, false);
                return false;
            }
        }

        return true;
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
