using System.Collections.Generic;

[System.Serializable]
public class Product {
    public int id;
    public string name;
    public float salePrice;
    public string description;
    public float alertQuantity;
    public string imageURL;
    public int nextLotNumber;

    public Company company;
    public int companyId;
    public Unit unit;
    public int unitId;
    public Category category;
    public int categoryId;
    public List<ProductStock> productstocks;
    public bool IsEnabledOnGrid = true;

    public float currentStock;
    public float currentStockAmount;
}