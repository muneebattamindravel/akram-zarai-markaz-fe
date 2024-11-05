using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class Screen_CompaniesView_Add : MonoBehaviour
{
    public TMP_InputField input_name, input_description, input_number;
    public TMP_Text text_title;
    ViewMode mode;
    Company company;
    public TMP_InputField input_openingBalance;
    public GameObject buttonSave;

    private void OnEnable()
    {
        input_name.text = "";
        input_description.text = "";
        input_number.text = "";
        input_openingBalance.text = "";
        buttonSave.SetActive(true);

        KeyboardManager.enterPressed += OnEnterPressed;
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void ShowView()
    {
        mode = ViewMode.ADD;
        text_title.text = Constants.Add + " " + Constants.Company;
        input_openingBalance.enabled = true;
    }

    public void ShowView(int companyId, bool viewOnly = false)
    {
        if (viewOnly)
        {
            mode = ViewMode.VIEW;
            buttonSave.SetActive(false);
            text_title.text = Constants.View + " " + Constants.Company;
        }
        else
        {
            mode = ViewMode.EDIT;
            buttonSave.SetActive(true);
            text_title.text = Constants.Edit + " " + Constants.Company;
        }

        input_openingBalance.enabled = false;
        GetCompany(companyId);
    }

    void GetCompany(int companyId)
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompany(companyId,
            (response) => {
                Preloader.Instance.HideFull();
                company = response.data;
                input_name.text = company.name;
                input_description.text = company.description;
                input_number.text = company.number;
                input_openingBalance.text = company.openingBalance.ToString();
            },
            (response) => {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
            });
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public void Button_SaveClicked()
    {
        if (string.IsNullOrEmpty(input_name.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.CompanyNameEmpty, false);
            return;
        }

        if (string.IsNullOrEmpty(input_openingBalance.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.OpeningBalanceZero, false);
            return;
        }

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            CompaniesManager.Instance.AddCompany(
                new Company(input_name.text, input_description.text, input_number.text, float.Parse(input_openingBalance.text)),
            (response) => {
                GUIManager.Instance.ShowToast(Constants.Success, Constants.CompanyAdded);
                if (CompaniesManager.onCompanyAdded != null) CompaniesManager.onCompanyAdded();

                Preloader.Instance.HideFull();
                GUIManager.Instance.Back();
            },
            (response) =>
            {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            }
            );
        }
        else if (mode == ViewMode.EDIT)
        {
            company.name = input_name.text;
            company.description = input_description.text;
            company.number = input_number.text;
            company.openingBalance = float.Parse(input_openingBalance.text);

            CompaniesManager.Instance.UpdateCompany(company, company.id,
                (response) => {
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.CompanyUpdated);
                    if (CompaniesManager.onCompanyUpdated != null) CompaniesManager.onCompanyUpdated();

                    Preloader.Instance.HideFull();
                    GUIManager.Instance.Back();
                },
                (response) =>
                {
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
