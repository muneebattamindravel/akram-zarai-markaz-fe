using System;
using Newtonsoft.Json;
[System.Serializable]
public class ProductStock {
    public int id;
    public int lotNumber;
    public string batchNumber;
    public string invoiceNumber;
    public DateTime expiryDate;
    public float costPrice;
    public float quantity;
    public float initialQuantity;
    public string notes;
    public DateTime createdAt;
    public DateTime updatedAt;
    public Product product;
    public int productId;
    [JsonProperty("purchaseId", NullValueHandling = NullValueHandling.Ignore)]
    public int purchaseId;

    public bool IsEnabledOnGrid = true;
}

public class ProductStockSummary
{
    public float currentStock;
    public float currentStockAmount;
    public ProductStockSummary(float ts, float tsa) { currentStock = ts; currentStockAmount = tsa; }
}