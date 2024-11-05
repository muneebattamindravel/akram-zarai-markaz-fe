using System;
[System.Serializable]
public class SaleProfit
{
    public int id;
    public float amount;
    public DateTime date;
    public int saleId;
    public int saleItemId;
    public Sale sale;
    public SaleItem saleItem;
}