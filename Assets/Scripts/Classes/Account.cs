using System;
using System.Collections.Generic;

[System.Serializable]
public class Account {
    public int id;
    public DateTime createdDate;
    public string name;
    public string type;
    public float openingBalance;
    public string description;
    public string bankName;
    public string bankAccountNumber;
    public bool isDefault = false;
    public float balance;
    public int? referenceId;

    public bool IsEnabledOnGrid = true;
}