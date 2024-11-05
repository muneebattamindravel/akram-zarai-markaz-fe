using System;

[System.Serializable]
public class StockBookEntry {
    public int id;
    public DateTime date;
    public string bookNumber;
    public string billNumber;
    public string invoiceNumber;
    public string type;
    public string notes;
    public float amount;
    public float closing;
    public Product product;
    public int productId;
    public int referenceId;

    public bool IsEnabledOnGrid = true;
}