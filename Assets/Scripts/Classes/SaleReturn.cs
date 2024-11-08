using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class SaleReturn {
    public int id;
    public int saleId;
    public List<SaleItem> returnItems;
    public string bookNumber;
    public string billNumber;
    public DateTime returnDate;
    public Product product;
    public float quantity;
    public float returnAmount;

    public bool IsEnabledOnGrid = true;
}