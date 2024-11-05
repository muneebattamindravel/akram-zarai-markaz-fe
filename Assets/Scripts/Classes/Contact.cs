using Newtonsoft.Json;

[System.Serializable]
public class Contact {
    public int id;
    public string name;
    public string type;
    public string businessName;
    public string number;
    public string email;
    public string address;
    public string notes;
    public int? accountId;
    public float openingBalance;

    public bool IsEnabledOnGrid = true;

    public Contact(string name, string type, string businessName, string number, string email, string address, string notes, float openingBalance) {
        this.name = name;
        this.type = type;
        this.businessName = businessName;
        this.number = number;
        this.email = email;
        this.address = address;
        this.notes = notes;
        this.openingBalance = openingBalance;
    }
}