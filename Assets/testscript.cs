using UnityEngine;
using Newtonsoft.Json;
using System;

public class testscript : MonoBehaviour
{
    private void Start()
    {
        //string jsonstring = "{'id':1,'name':'Antracool 1 kg','salePrice':0,'description':'','alertQuantity':5,'imageURL':'','nextLotNumber':1,'createdAt':'2021-01-08T03:59:18.000Z','updatedAt':'2021-01-08T03:59:18.000Z','currentStock':null,'company':{'id':2,'name':'Bayer','description':''},'category':{'id':1,'name':'Pesticides','description':'All Pesticides Medicines'},'unit':{'id':2,'name':'Kg','description':'Kilogram'}}";
        string jsonstring = "{\"currentStock\":null}";
        Debug.Log(jsonstring);
        TestClass c = JsonConvert.DeserializeObject<TestClass>(jsonstring);
        Debug.Log(c.currentStock);
    }
}

public class TestClass
{
    public float currentStock;
}
