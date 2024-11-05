using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class SalePayment {
    public int id;
    public float receivedAmount;
    public DateTime receivedDate;
    public PaymentType paymentType;
    public string bookNumber;
    public string billNumber;
    public DateTime createdAt;
    public DateTime updatedAt;
    public int saleId;
    public int accountId;
    public Account account;

    public bool IsEnabledOnGrid = true;
}