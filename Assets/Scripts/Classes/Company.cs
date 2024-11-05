using Newtonsoft.Json;

[System.Serializable]
public class Company {
    public int id;
    public string name;
    public string description;
    public string number;
    public int? accountId;
    public float openingBalance;

    public bool IsEnabledOnGrid = true;

    public Company(string name, string description, string number, float openingBalance)
    {
        this.name = name;
        this.description = description;
        this.number = number;
        this.openingBalance = openingBalance;
    }
}