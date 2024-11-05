using System;
using System.Collections.Generic;

[System.Serializable]
public class Incentive {
    public int id;
    public DateTime date;
    public string notes;
    public string type;
    public float amount;
    public int companyId;
    public Company company;

    public bool IsEnabledOnGrid = true;
}