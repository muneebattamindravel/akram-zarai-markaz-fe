using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;

public class ProductsManager : MonoBehaviour
{
    public static ProductsManager Instance;
    public delegate void ProductsEvent();

    public static ProductsEvent onProductAdded, onProductUpdated;
    public static UnityAction onProductStockAdded, onProductStockUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string PRODUCTS_ROUTE = "products";
    string PRODUCTS_STOCK_RETURN_ROUTE = "productstocks/return";
    string MANUAL_PRODUCTS_STOCK_RETURN_ROUTE = "productstocks/returnManual";
    string PRODUCT_STOCKS_ROUTE = "productStocks";
    string PRODUCT_STOCK_ROUTE = "productStock";
    string STOCK_BOOKS_ROUTE = "products/stockBook";

    public void AddProduct(Product product, ResponseAction<Product> successAction, ResponseAction<Product> failAction = null)
    {
        APIManager.Instance.Post<Product>(PRODUCTS_ROUTE, product, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetProduct(int productId, ResponseAction<Product> successAction, ResponseAction<Product> failAction = null)
    {
        APIManager.Instance.Get<Product>(PRODUCTS_ROUTE + "/" + productId, (response) =>
        {
            ProductStockSummary summary = GetProductStocksSummary(response.data);
            response.data.currentStock = summary.currentStock;
            response.data.currentStockAmount = summary.currentStockAmount;

            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetProducts(ResponseAction<List<Product>> successAction, ResponseAction<List<Product>> failAction = null)
    {
        APIManager.Instance.Get<List<Product>>(PRODUCTS_ROUTE, (response) => {
            foreach (Product p in response.data)
            {
                ProductStockSummary summary = GetProductStocksSummary(p);
                p.currentStock = summary.currentStock;
                p.currentStockAmount = summary.currentStockAmount;
            }
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetProducts(string nameFilter, ResponseAction<List<Product>> successAction, ResponseAction<List<Product>> failAction = null)
    {
        APIManager.Instance.Get<List<Product>>(PRODUCTS_ROUTE + "/bynamefilter/" + nameFilter, (response) => {
            foreach (Product p in response.data)
            {
                ProductStockSummary summary = GetProductStocksSummary(p);
                p.currentStock = summary.currentStock;
                p.currentStockAmount = summary.currentStockAmount;
            }
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public ProductStockSummary GetProductStocksSummary(Product product)
    {
        float currentStock = 0.00f, currentStockAmount = 0.00f;
        foreach (ProductStock stock in product.productstocks)
        {
            currentStock += stock.quantity;
            currentStockAmount += (stock.quantity * stock.costPrice);
        }

        return new ProductStockSummary(currentStock, currentStockAmount);
    }

    public void AddProductStock(ProductStock productStock, ResponseAction<ProductStock> successAction, ResponseAction<ProductStock> failAction = null)
    {
        APIManager.Instance.Post<ProductStock>(PRODUCT_STOCKS_ROUTE, productStock, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateProductStock(ProductStock productStock, int productStockId, ResponseAction<ProductStock> successAction, ResponseAction<ProductStock> failAction = null)
    {
        APIManager.Instance.Patch<ProductStock>(PRODUCT_STOCK_ROUTE + "/" + productStockId, productStock, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetProductStocks(int productId, ResponseAction<List<ProductStock>> successAction, ResponseAction<List<ProductStock>> failAction = null)
    {
        APIManager.Instance.Get<List<ProductStock>>(PRODUCT_STOCKS_ROUTE + "/" + productId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetProductStocksByPurchaseId(int purchaseId, ResponseAction<List<ProductStock>> successAction, ResponseAction<List<ProductStock>> failAction = null)
    {
        APIManager.Instance.Get<List<ProductStock>>("productstocksbypurchaseid" + "/" + purchaseId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetStockBook(int productId, MRDateRange range, ResponseAction<List<StockBookEntry>> successAction, ResponseAction<List<StockBookEntry>> failAction = null)
    {
        APIManager.Instance.Get<List<StockBookEntry>>(STOCK_BOOKS_ROUTE + "/" + productId + "?from=" + range.from + "&to=" + range.to, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetProductStock(int productStockId, ResponseAction<ProductStock> successAction, ResponseAction<ProductStock> failAction = null)
    {
        APIManager.Instance.Get<ProductStock>(PRODUCT_STOCK_ROUTE + "/" + productStockId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateProduct(Product product, int productId, ResponseAction<Product> successAction, ResponseAction<Product> failAction = null)
    {
        APIManager.Instance.Patch<Product>(PRODUCTS_ROUTE + "/" + productId, product, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteProduct(int productId, ResponseAction<Product> successAction, ResponseAction<Product> failAction = null)
    {
        APIManager.Instance.Delete<Product>(PRODUCTS_ROUTE + "/" + productId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }

    public void ReturnProductStock(ReturnProductStockParams returnParam, ResponseAction<ReturnProductStockParams> successAction, ResponseAction<ReturnProductStockParams> failAction = null)
    {
        APIManager.Instance.Post<ReturnProductStockParams>(PRODUCTS_STOCK_RETURN_ROUTE, returnParam, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void ReturnManualProductStock(ReturnProductStockParams returnParam, ResponseAction<ReturnProductStockParams> successAction, ResponseAction<ReturnProductStockParams> failAction = null)
    {
        APIManager.Instance.Post<ReturnProductStockParams>(MANUAL_PRODUCTS_STOCK_RETURN_ROUTE, returnParam, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}

[System.Serializable]
public class ReturnProductStockParams
{
    public int productStockId;
    public int? companyAccountId;
    public float returnQuantity;
    public string details;
    public DateTime returnDate;
    public string invoiceNumber;
    public float returnPrice;
}