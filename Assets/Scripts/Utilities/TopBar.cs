using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TopBar : MonoBehaviour
{
    public static TopBar Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        Hide();
    }

    public TMP_Text text_todaysSale, text_todaysProfit, text_defaultAccountBalance, text_todaysDate;
    public GameObject container;

    private void OnEnable()
    {
        SalesManager.onSaleAdded += FetchTopBarData;
        SalesManager.onSaleDeleted += FetchTopBarData;
        LoginManager.onLoggedIn += FetchTopBarData;
        BookingsManager.onBookingAdded += FetchTopBarData;
        SalesManager.onReceivedSalePayment += FetchTopBarData;
        RecoveriesManager.onRecoveryAdded += FetchTopBarData;
        ExpensesManager.onExpenseAdded += FetchTopBarData;
        TransfersManager.onTransferAdded += FetchTopBarData;
        RecoveriesManager.onRecoveryDeleted += FetchTopBarData;
        LoansManager.onLoanAdded += FetchTopBarData;
        LoansManager.onLoanDeleted += FetchTopBarData;

        text_todaysSale.text = "0.00" + Constants.Currency;
        text_todaysProfit.text = "0.00" + Constants.Currency;
        text_defaultAccountBalance.text = "0.00" + Constants.Currency;
    }

    private void OnDisable()
    {
        SalesManager.onSaleAdded -= FetchTopBarData;
        SalesManager.onSaleDeleted -= FetchTopBarData;
        LoginManager.onLoggedIn -= FetchTopBarData;
        BookingsManager.onBookingAdded -= FetchTopBarData;
        SalesManager.onReceivedSalePayment -= FetchTopBarData;
        RecoveriesManager.onRecoveryAdded -= FetchTopBarData;
        ExpensesManager.onExpenseAdded -= FetchTopBarData;
        TransfersManager.onTransferAdded -= FetchTopBarData;
        RecoveriesManager.onRecoveryDeleted -= FetchTopBarData;
        LoansManager.onLoanAdded -= FetchTopBarData;
        LoansManager.onLoanDeleted -= FetchTopBarData;
    }

    //.ToString("#,##0.00")
    public void FetchTopBarData()
    {
        DashboardManager.Instance.GetTopBarData(DateTime.Now, DateTime.Now,
        (response) => {
            text_todaysSale.text = response.data.todaySale.ToCommaSeparatedNumbers() + Constants.Currency;
            text_todaysProfit.text = response.data.todayProfit.ToCommaSeparatedNumbers() + Constants.Currency;
            text_defaultAccountBalance.text = response.data.totalCash.ToCommaSeparatedNumbers() + Constants.Currency;
        },
        (response) => {
        });

        text_todaysDate.text = DateTime.Now.ToString(Constants.DateDisplayFormat);
    }

    public void Show()
    {
        container.SetActive(true);
    }
    public void Hide()
    {
        container.SetActive(false);
    }

    public void Button_NewSaleClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Sale_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Sale_View_Add>().ShowView();
    }

    //void GetTodaysSale()
    //{
    //    SalesManager.Instance.GetCounterSaleAmount(DateTime.Now, DateTime.Now,
    //    (response) => {
    //        text_todaysSale.text = response.data.amount + Constants.Currency;
    //    },
    //    (response) => {

    //    });
    //}

    //void GetTodaysProfit()
    //{
    //    ProfitsManager.Instance.GetCounterSaleProfit(DateTime.Now, DateTime.Now,
    //    (response) =>
    //    {
    //        text_todaysProfit.text = response.data.amount + Constants.Currency;
    //    },
    //    (response) =>
    //    {

    //    });
    //}

    //void GetDefaultAccountBalance()
    //{
    //    AccountsManager.Instance.GetDefaultAccountBalance(
    //    (response) => {
    //        text_defaultAccountBalance.text = response.data.amount + Constants.Currency;
    //    },
    //    (response) => {

    //    });
    //}

    public void Button_UploadClicked()
    {
        Preloader.Instance.ShowFull();
        Backup backup = new Backup();
        backup.date = DateTime.Now.ToString(Constants.BackupFileNamePrefix);

        DataManager.Instance.BackUpAndUpload(backup, (response) => {
            Preloader.Instance.HideFull();
            if (response.data.fileCreated)
                GUIManager.Instance.ShowToast(Constants.Success, Constants.BackupDone, true);
            if (response.data.fileUploaded)
                GUIManager.Instance.ShowToast(Constants.Success, Constants.DataUploaded, true);
        }, (response) => {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.Failed, false);
        });
    }
}
