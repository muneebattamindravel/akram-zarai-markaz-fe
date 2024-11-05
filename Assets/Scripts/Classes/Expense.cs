using System;
using System.Collections.Generic;

[System.Serializable]
public class Expense {
    public int id;
    public DateTime date;
    public ExpenseType type;
    public string description;
    public string bookNumber;
    public string billNumber;
    public float amount;
    public int accountId;

    public bool IsEnabledOnGrid = true;
}