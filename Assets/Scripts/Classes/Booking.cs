using System;
using System.Collections.Generic;

[System.Serializable]
public class Booking {
    public int id;
    public float totalAmount;
    public string prNumber;
    public DateTime bookingDate;
    public string notes;
    public Company company;
    public int companyId;
    public int fromAccountId;
    public string policyName;
    public string policyPercentage;
    public string netRate;
    public string bookingType;
    public string policyType;

    public bool IsEnabledOnGrid = true;
}