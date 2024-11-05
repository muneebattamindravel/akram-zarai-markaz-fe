using System;
using System.Collections.Generic;

[System.Serializable]
public class Recovery {
    public int id;
    public DateTime date;
    public int contactId;
    public Contact contact;
    public string bookNumber;
    public string billNumber;
    public float amount;
    public bool isReceived;
    public int accountId;
    public Account account;

    public bool IsEnabledOnGrid = true;
}