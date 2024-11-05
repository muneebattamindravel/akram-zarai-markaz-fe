using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CategoriesManager : MonoBehaviour
{
    public static CategoriesManager Instance;
    public delegate void CategoriesEvent();

    public static CategoriesEvent onCategoryAdded, onCategoryUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string CATEGORIES_ROUTE = "categories";

    public void AddCategory(Category category, ResponseAction<Category> successAction, ResponseAction<Category> failAction = null)
    {
        APIManager.Instance.Post<Category>(CATEGORIES_ROUTE, category, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetCategory(int categoryId, ResponseAction<Category> successAction, ResponseAction<Category> failAction = null)
    {
        APIManager.Instance.Get<Category>(CATEGORIES_ROUTE + "/" + categoryId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetCategories(ResponseAction<List<Category>> successAction, ResponseAction<List<Category>> failAction = null)
    {
        APIManager.Instance.Get<List<Category>>(CATEGORIES_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateCategory(Category category, int categoryId, ResponseAction<Category> successAction, ResponseAction<Category> failAction = null)
    {
        APIManager.Instance.Patch<Category>(CATEGORIES_ROUTE + "/" + categoryId, category, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteCategory(int categoryId, ResponseAction<Category> successAction, ResponseAction<Category> failAction = null)
    {
        APIManager.Instance.Delete<Category>(CATEGORIES_ROUTE + "/" + categoryId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }
}
