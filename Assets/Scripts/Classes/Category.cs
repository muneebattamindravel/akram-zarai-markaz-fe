[System.Serializable]
public class Category {
    public int id;
    public string name;
    public string description;



    public bool IsEnabledOnGrid = true;

    public Category(string name, string description) { this.name = name; this.description = description; }
}