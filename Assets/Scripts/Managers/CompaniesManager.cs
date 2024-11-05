using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CompaniesManager : MonoBehaviour
{
    public static CompaniesManager Instance;
    public delegate void CompaniesEvent();

    public static CompaniesEvent onCompanyAdded, onCompanyUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string COMPANIES_ROUTE = "companies";

    public void AddCompany(Company company, ResponseAction<Company> successAction, ResponseAction<Company> failAction = null)
    {
        APIManager.Instance.Post<Company>(COMPANIES_ROUTE, company, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetCompany(int companyId, ResponseAction<Company> successAction, ResponseAction<Company> failAction = null)
    {
        APIManager.Instance.Get<Company>(COMPANIES_ROUTE + "/" + companyId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetCompanies(ResponseAction<List<Company>> successAction, ResponseAction<List<Company>> failAction = null)
    {
        APIManager.Instance.Get<List<Company>>(COMPANIES_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateCompany(Company company, int companyId, ResponseAction<Company> successAction, ResponseAction<Company> failAction = null)
    {
        APIManager.Instance.Patch<Company>(COMPANIES_ROUTE + "/" + companyId, company, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteCompany(int companyId, ResponseAction<Company> successAction, ResponseAction<Company> failAction = null)
    {
        APIManager.Instance.Delete<Company>(COMPANIES_ROUTE + "/" + companyId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }
}
