using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class Sale {
    public int id;
    public float totalAmount;
    public float receivedAmount;
    public float discount;
    public DateTime saleDate;
    public string bookNumber;
    public string billNumber;
    public string notes;
    public DateTime createdAt;
    public DateTime updatedAt;

    public float profitAmount;

    [JsonProperty("contactId", NullValueHandling = NullValueHandling.Ignore)]
    public int contactId;
    public Contact contact;
    public List<SaleItem> saleitems;
    public List<SaleProfit> saleprofits;

    public SalePayment salepayment;

    public bool returnApplied = false;
    public bool IsEnabledOnGrid = true;
}