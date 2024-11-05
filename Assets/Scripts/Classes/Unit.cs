[System.Serializable]
public class Unit {
    public int id;
    public string name;
    public string description;
    public bool allowDecimal;

    public bool IsEnabledOnGrid = true;

    public Unit(string name, string description, bool allowDecimal) { this.name = name; this.description = description; this.allowDecimal = allowDecimal; }
}