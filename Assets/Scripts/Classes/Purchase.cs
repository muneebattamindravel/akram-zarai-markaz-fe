using System;
using System.Collections.Generic;

[System.Serializable]
public class Purchase {
    public int id;
    public Company company;
    public int companyId;
    public Contact contact;
    public int contactId;
    public string invoiceNumber;
    public PurchaseType purchaseType;
    public DateTime invoiceDate;
    public string notes;
    public float totalAmount;
    public List<ProductStock> purchasedproductstocks;

    public DateTime createdAt;
    public DateTime updatedAt;
    
    public bool IsEnabledOnGrid = true;
}