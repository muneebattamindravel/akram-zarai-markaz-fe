using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UI.Dates;
using System;
using System.Globalization;

public class Screen_ReceiveSalePayment : MonoBehaviour
{
    List<Account> onlineAccounts;
    List<string> onlineAccountsStrings;
    int saleId;
    Sale sale;

    //UI
    public TMP_Text text_saleId, text_saleTotalAmount, text_customerName, text_salePendingAmount;
    public TMP_InputField input_bookNumber, input_billNumber, input_paymentReceived;
    public GameObject buttonReceivePayment, buttonSadReceivePayment;
    public TMP_Dropdown dropdown_paymentType, dropdown_onlineAccount;
    public MRDatePicker datepicker_receivedDate;

    float paymentReceived = 0f;
    float remainingAmount = 0f;

    private void OnEnable()
    {
        input_billNumber.text = "";
        input_bookNumber.text = "";
        input_paymentReceived.text = "";
        paymentReceived = 0f;
        buttonReceivePayment.SetActive(true);
        buttonSadReceivePayment.SetActive(false);
        dropdown_onlineAccount.gameObject.SetActive(false);
        dropdown_paymentType.value = 0;
    }

    public void ShowView(int saleId)
    {
        this.saleId = saleId;
        GetOnlineAccounts();
        GetSaleDetails();
    }

    void GetSaleDetails()
    {
        SalesManager.Instance.GetSale(saleId,
            (response) =>
            {
                sale = response.data;
                text_customerName.text = sale.contact.name;
                text_saleId.text = sale.id.ToString();
                text_saleTotalAmount.text = sale.totalAmount + Constants.Currency;
                remainingAmount = sale.totalAmount - sale.receivedAmount;
                text_salePendingAmount.text = remainingAmount + Constants.Currency;
            },
            (response) =>
            {
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
            });
    }

    public void OnPaymentReceivedValueChanged()
    {
        buttonReceivePayment.SetActive(true);
        buttonSadReceivePayment.SetActive(false);

        if (!string.IsNullOrEmpty(input_paymentReceived.text))
        {
            paymentReceived = float.Parse(input_paymentReceived.text, CultureInfo.InvariantCulture.NumberFormat);
            if (paymentReceived < remainingAmount)
                buttonSadReceivePayment.SetActive(true);
            else if (paymentReceived > remainingAmount)
            {
                input_paymentReceived.text = "";
                paymentReceived = 0;
                GUIManager.Instance.ShowToast(Constants.Failed, Constants.ReceivedAmountGreater, false);
            }
        }
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    void GetOnlineAccounts()
    {
        AccountsManager.Instance.GetAccounts(
            (response) =>
            {
                onlineAccounts = response.data;
                onlineAccounts = onlineAccounts.FindAll(p => p.type == AccountType.Online.ToString());
                onlineAccountsStrings = new List<string>();
                foreach (Account account in onlineAccounts) onlineAccountsStrings.Add(account.name + "\n" + account.bankName + " - " + account.bankAccountNumber);
                dropdown_onlineAccount.ClearOptions();
                dropdown_onlineAccount.AddOptions(onlineAccountsStrings);
            },
            (response) =>
            {
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            }
        );
    }

    public void OnPaymentTypeChanged()
    {
        if (dropdown_paymentType.value == 0)
            dropdown_onlineAccount.gameObject.SetActive(false);
        else
            dropdown_onlineAccount.gameObject.SetActive(true);
    }

    public void Button_ReceiveClicked()
    {
        OnPaymentReceivedValueChanged();

        if ( datepicker_receivedDate.SelectedDate == DateTime.MinValue)
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ReceivedDateEmtpy, false);
            return;
        }

        if (string.IsNullOrEmpty(input_bookNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterBookNumber, false);
            return;
        }

        if (string.IsNullOrEmpty(input_billNumber.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterBillNumber, false);
            return;
        }

        if (string.IsNullOrEmpty(input_paymentReceived.text))
        {
            GUIManager.Instance.ShowToast(Constants.Failed, Constants.EnterPaymentReceived, false);
            return;
        }

        SalePayment payment = new SalePayment();
        payment.receivedAmount = paymentReceived;
        payment.bookNumber = input_bookNumber.text;
        payment.billNumber = input_billNumber.text;
        payment.receivedDate = datepicker_receivedDate.SelectedDate;
        payment.saleId = sale.id;
        if (dropdown_paymentType.value == 0)
        {
            payment.paymentType = PaymentType.Cash;
            payment.accountId = 0;
        }
        else
        {
            payment.paymentType = PaymentType.Online;
            payment.accountId =
                onlineAccounts[dropdown_onlineAccount.options.
                FindIndex(p => p.text == dropdown_onlineAccount.options[dropdown_onlineAccount.value].text)].id;
        }

        SalesManager.Instance.ReceiveSalePayment(payment, (response) => {
            GUIManager.Instance.ShowToast(Constants.Success, Constants.SalePaymentReceived);
            if (SalesManager.onReceivedSalePayment != null) SalesManager.onReceivedSalePayment();
            GUIManager.Instance.Back();
        }, null);
    }
}
