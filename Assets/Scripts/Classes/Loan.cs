using System;
using System.Collections.Generic;

[System.Serializable]
public class Loan {
    public int id;
    public DateTime date;
    public string bookNumber;
    public string billNumber;
    public float amount;
    public bool isReceived;
    public int contactId;
    public Contact contact;
    public int accountId;
    public Account account;
    public int contactAccountId;
    public Account contactAccount;

    public bool IsEnabledOnGrid = true;
}