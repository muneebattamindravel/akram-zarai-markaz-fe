using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Crosstales.FB;
using System.IO;

public class Screen_Products_Add : MonoBehaviour
{
    public TMP_InputField input_name, input_salePrice, input_alertQuantity, input_description;
    public TMP_Dropdown dropdown_company, dropdown_category, dropdown_unit;
    Product product;
    List<Company> companies;
    List<Category> categories;
    List<Unit> units;
    public TMP_Text text_filePath;
    public RawImage image_product;

    private void OnEnable()
    {
        input_name.text = "";
        input_salePrice.text = "0.00";
        input_alertQuantity.text = "5";
        input_description.text = "";
        text_filePath.text = "";
        image_product.texture = null;

        dropdown_category.options.Clear();
        dropdown_company.options.Clear();
        dropdown_unit.options.Clear();

        FileBrowser.Instance.OnOpenFilesComplete += Instance_OnOpenFilesComplete;
    }

    private void Instance_OnOpenFilesComplete(bool selected, string singleFile, string[] files)
    {
        if (selected)
        {
            text_filePath.text = singleFile;
            ImageUtils.Instance.LoadImage(image_product, singleFile);
        }
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
        FileBrowser.Instance.OnOpenFilesComplete -= Instance_OnOpenFilesComplete;
    }

    public void ShowView()
    {
        GetData();
    }

    void GetData()
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies((response) => {
            companies = response.data;
            List<string> companyNames = new List<string>();
            foreach (Company company in companies) companyNames.Add(company.name);
            dropdown_company.AddOptions(companyNames);

            CategoriesManager.Instance.GetCategories((response) => {
                categories = response.data;
                List<string> categoryNames = new List<string>();
                foreach (Category category in categories) categoryNames.Add(category.name);
                dropdown_category.AddOptions(categoryNames);

                UnitsManager.Instance.GetUnits((response) => {
                    units = response.data;
                    List<string> unitNames = new List<string>();
                    foreach (Unit unit in units) unitNames.Add(unit.name);
                    dropdown_unit.AddOptions(unitNames);

                    Preloader.Instance.HideFull();
                });
            });
        });
    }

    public void OnEnterPressed()
    {
        Button_SaveClicked();
    }

    public void Button_SaveClicked()
    {
        if (string.IsNullOrEmpty(input_name.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ProductNameEmpty, false);
            return;
        }

        Preloader.Instance.ShowFull();

        Product product = new Product();
        product.name = input_name.text;

        if (!string.IsNullOrEmpty(input_salePrice.text))
            product.salePrice = float.Parse(input_salePrice.text);
        else
            product.salePrice = 0.00f;
        
        product.description = input_description.text;
        product.alertQuantity = float.Parse(input_alertQuantity.text);
        product.imageURL = "";
        product.companyId = companies.Find(p => p.name == dropdown_company.options[dropdown_company.value].text).id;
        product.unitId = units.Find(p => p.name == dropdown_unit.options[dropdown_unit.value].text).id;
        product.categoryId = categories.Find(p => p.name == dropdown_category.options[dropdown_category.value].text).id;

        ProductsManager.Instance.AddProduct(product,
        (response) => {

            if (!string.IsNullOrEmpty(text_filePath.text))
            {
                string newPath = Constants.ProductImage + "-" + product.name.Replace(" ", "") + "-" + response.data.id;
                File.Copy(text_filePath.text, Application.persistentDataPath + "/" + newPath, true);
                response.data.imageURL = newPath;
            }
            else
                response.data.imageURL = "";

            ProductsManager.Instance.UpdateProduct(response.data, response.data.id, (updated) => {

                Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.ProductAdded);

                    if (ProductsManager.onProductAdded != null)
                        ProductsManager.onProductAdded();

                    GUIManager.Instance.Back();

                }, null);
            
        },
        (response) =>
        {
            Preloader.Instance.HideFull();
            GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
        }
        );
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }

    public void Button_SelectImageFileClicked()
    {
        FileBrowser.Instance.OpenSingleFile();
    }
}
