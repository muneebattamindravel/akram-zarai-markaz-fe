using System;

[System.Serializable]
public class AccountTransaction {
    public int id;
    public DateTime transactionDate;
    public float amount;
    public float closingBalance;
    public string description;
    public int accountId;
    public DateTime createdAt;
    public string type;
    public string details;
    public int referenceId;

    public string invoiceNumber;
    public string prNumber;
    public string bookNumber;
    public string billNumber;

    public bool IsEnabledOnGrid = true;
}