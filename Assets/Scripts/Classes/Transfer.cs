using System;
using System.Collections.Generic;

[System.Serializable]
public class Transfer {
    public int id;
    public DateTime date;
    public int fromAccountId;
    public Account fromAccount;
    public int toAccountId;
    public Account toAccount;
    public string bookNumber;
    public string billNumber;
    public float amount;
    public string notes;
    public string details = "";

    public bool IsEnabledOnGrid = true;
}