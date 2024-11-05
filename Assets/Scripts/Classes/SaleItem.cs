using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class SaleItem {
    public int id;
    public float salePrice;
    public float quantity;
    public int usedLotNumber;
    public string lotsUsedJson;
    public List<LotInfo> lotInformation;
    public Product product;
    public Sale sale;
    public int saleId;

    public bool IsEnabledOnGrid = true;

    public void Print() {
        Debug.Log($"id {id} salePrice {salePrice} quantity {quantity} " +
            $"lotsUsedJson {lotsUsedJson} productName {product.name} productId {product.id}" +
            $"Sale Id {saleId}");
    }

    public void ConsolidateLotUsedInformation() {
        this.lotInformation = JsonConvert.DeserializeObject<List<LotInfo>>(this.lotsUsedJson);
    }
}

[System.Serializable]
public class LotInfo {
    public int lotNumber;
    public float quantity;
}