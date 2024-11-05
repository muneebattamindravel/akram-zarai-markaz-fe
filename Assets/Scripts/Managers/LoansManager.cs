using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoansManager : MonoBehaviour
{
    public static LoansManager Instance;
    public delegate void LoansEvent();
    public static LoansEvent onLoanAdded, onLoanDeleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string LOANS_ROUTE = "loans";

    public void CreateLoan(Loan loan, ResponseAction<Loan> successAction, ResponseAction<Loan> failAction = null)
    {
        APIManager.Instance.Post<Loan>(LOANS_ROUTE, loan, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetLoans(MRDateRange range, ResponseAction<List<Loan>> successAction, ResponseAction<List<Loan>> failAction = null)
    {
        APIManager.Instance.Get<List<Loan>>(LOANS_ROUTE + "?from=" + range.from + "&to=" + range.to, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetLoan(int loanId, ResponseAction<Loan> successAction, ResponseAction<Loan> failAction = null)
    {
        APIManager.Instance.Get<Loan>(LOANS_ROUTE + "/" + loanId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteLoan(int loanId, ResponseAction<Loan> successAction, ResponseAction<Loan> failAction = null)
    {
        APIManager.Instance.Delete<Loan>(LOANS_ROUTE + "/" + loanId, (response) =>
        {
            successAction(response);
            onLoanDeleted?.Invoke();
        }, (response) =>
        {
            failAction(response);
        });
    }
}
