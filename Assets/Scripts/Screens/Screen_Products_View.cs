using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;
using Crosstales.FB;
using System.IO;

public class Screen_Products_View : MonoBehaviour
{
    public TMP_InputField input_name, input_salePrice, input_alertQuantity, input_description;
    public TMP_Dropdown dropdown_company, dropdown_category, dropdown_unit;
    public TMP_Text text_totalStockQuantity, text_totalStockAmount;
    Product product;
    List<Company> companies;
    List<Category> categories;
    List<Unit> units;
    public TMP_Text text_filePath;
    public RawImage image_product;

    public GameObject contentRoot;
    public List<ProductStock> productstocks;
    public List<ColumnHeader> columnHeaders;

    int productId;

    private void OnEnable()
    {
        input_name.text = "";
        input_salePrice.text = "0.00";
        input_alertQuantity.text = "5";
        input_description.text = "";
        text_filePath.text = "";
        //KeyboardManager.enterPressed += OnEnterPressed;
        ProductsManager.onProductStockAdded += GetProduct;
        ProductsManager.onProductStockUpdated += GetProduct;
        image_product.texture = Resources.Load<Texture2D>(Constants.DefaultProductImagePath);

        FileBrowser.Instance.OnOpenFilesComplete += Instance_OnOpenFilesComplete;

        InitializeColumnsHeaders();
    }

    public void InitializeColumnsHeaders()
    {
        foreach (ColumnHeader header in columnHeaders)
        {
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.AddListener(() => {
                foreach (ColumnHeader hdr in columnHeaders)
                    if (hdr != header)
                        hdr.ResetState();

                ColumnState nextState = header.SetNextState();
                FieldInfo fieldInfo = typeof(ProductStock).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    productstocks = productstocks.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    productstocks = productstocks.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    productstocks = productstocks.OrderBy(p => p.id).ToList();

                PopulateProductStocksData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onEndEdit.AddListener((endValue) => {
                foreach (ProductStock item in productstocks) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(ProductStock).GetField(header.dataField);
                foreach (ProductStock filtered in productstocks.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateProductStocksData();
            });
        }
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
        //KeyboardManager.enterPressed -= OnEnterPressed;
        ProductsManager.onProductStockAdded -= GetProduct;
        ProductsManager.onProductStockUpdated -= GetProduct;
    }

    public void ShowView(int productId)
    {
        this.productId = productId;
        GetData();
    }

    void GetData()
    {
        Preloader.Instance.ShowFull();
        CompaniesManager.Instance.GetCompanies((response) =>
        {
            companies = response.data;
            List<string> companyNames = new List<string>();
            foreach (Company company in companies) companyNames.Add(company.name);
            dropdown_company.AddOptions(companyNames);

            CategoriesManager.Instance.GetCategories((response) =>
            {
                categories = response.data;
                List<string> categoryNames = new List<string>();
                foreach (Category category in categories) categoryNames.Add(category.name);
                dropdown_category.AddOptions(categoryNames);

                UnitsManager.Instance.GetUnits((response) =>
                {
                    units = response.data;
                    List<string> unitNames = new List<string>();
                    foreach (Unit unit in units) unitNames.Add(unit.name);
                    dropdown_unit.AddOptions(unitNames);

                    GetProduct();
                });
            });
        });
    }

    void GetProduct()
    {
        Preloader.Instance.ShowFull();
        ProductsManager.Instance.GetProduct(productId,
            (response) =>
            {
                product = response.data;

                input_name.text = product.name;
                input_salePrice.text = product.salePrice.ToString();
                input_alertQuantity.text = product.alertQuantity.ToString();
                input_description.text = product.description.ToString();
                dropdown_company.value = dropdown_company.options.FindIndex(p => p.text == product.company.name);
                dropdown_unit.value = dropdown_unit.options.FindIndex(p => p.text == product.unit.name);
                dropdown_category.value = dropdown_category.options.FindIndex(p => p.text == product.category.name);

                ImageUtils.Instance.LoadImage(image_product, Application.persistentDataPath + "/" + response.data.imageURL);

                productstocks = product.productstocks;
                columnHeaders[0].ResetState();
                columnHeaders[0].SetNextState();
                PopulateProductStocksData();
            },
            (response) =>
            {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
            });
    }

    void PopulateProductStocksData()
    {
        ClearContent();
        float totalStockQuantity = 0.00f, totalStockAmount = 0.00f;
        foreach (ProductStock productStock in productstocks.FindAll(p => p.IsEnabledOnGrid))
        {
            GameObject item = PoolManager.Instance.GetPooledItem(PoolManager.PoolType.ProductStockItem);
            item.transform.parent = contentRoot.transform;
            item.transform.localScale = Vector3.one;

            foreach (ColumnHeader header in columnHeaders.FindAll(p => !p.isCustomValue))
            {
                item.transform.Find(header.dataField).GetComponent<TMP_Text>().text = productStock.GetType().GetField(header.dataField).GetValue(productStock).ToString();
                if (header.isViewLink)
                {
                    item.transform.Find(header.dataField).GetComponent<MRButton>().onClicked.RemoveAllListeners();
                    item.transform.Find(header.dataField).GetComponent<MRButton>().onClicked.AddListener(() =>
                    {
                        ViewProductStock(productStock.id);
                    });
                }
            }

            if (productStock.purchaseId == 0)
            {
                item.transform.Find("type").GetComponent<TMP_Text>().text = Constants.Manual;
                item.transform.Find("Button_Return").gameObject.SetActive(true);
                item.transform.Find("Button_View").gameObject.SetActive(true);

                item.transform.Find("Button_Return").GetComponent<MRButton>().onClicked.AddListener(() => {
                    GUIManager.Instance.OpenScreenExplicitly(MRScreenName.ManualStock_Return);
                    GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ManualStock_Return>().ShowView(product, productStock);
                });
            }
            else
            {
                item.transform.Find("type").GetComponent<TMP_Text>().text = Constants.Purchase;
                item.transform.Find("Button_View").gameObject.SetActive(false);
                item.transform.Find("Button_Return").gameObject.SetActive(false);
            }

            item.transform.Find("quantity").GetComponent<TMP_Text>().text = productStock.quantity + " " + product.unit.name;
            item.transform.Find("totalCost").GetComponent<TMP_Text>().text = (productStock.quantity * productStock.costPrice) + Constants.Currency;
            item.transform.Find("expiryDate").GetComponent<TMP_Text>().text = productStock.expiryDate.ToString(Constants.DateDisplayFormat);

            item.transform.Find("Button_View").GetComponent<MRButton>().onClicked.RemoveAllListeners();
            item.transform.Find("Button_View").GetComponent<MRButton>().onClicked.AddListener(() =>
            {
                ViewProductStock(productStock.id);
            });

            totalStockQuantity += productStock.quantity;
            totalStockAmount += (productStock.costPrice * productStock.quantity);

            item.SetActive(true);
        }

        text_totalStockAmount.text = totalStockAmount + Constants.Currency;
        text_totalStockQuantity.text = totalStockQuantity + " " + product.unit.name;

        Preloader.Instance.HideFull();
    }

    void ViewProductStock(int productStockId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.ProductStock_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ProductStock_View_Add>().ShowView(productStockId, product, true);
    }

    public void Button_AddProductStockClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.ProductStock_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ProductStock_View_Add>().ShowView(product);
    }

    void ClearContent()
    {
        foreach (Transform t in contentRoot.transform) t.gameObject.SetActive(false);
    }

    public void OnEnterPressed()
    {
        if (GUIManager.Instance.CURRENTSCREENNAME == MRScreenName.Products_View)
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

        product.name = input_name.text;
        product.salePrice = float.Parse(input_salePrice.text);
        product.description = input_description.text;
        product.alertQuantity = int.Parse(input_alertQuantity.text);
        product.company = companies.Find(p => p.name == dropdown_company.options[dropdown_company.value].text);
        product.companyId = companies.Find(p => p.name == dropdown_company.options[dropdown_company.value].text).id;
        product.unit = units.Find(p => p.name == dropdown_unit.options[dropdown_unit.value].text);
        product.unitId = units.Find(p => p.name == dropdown_unit.options[dropdown_unit.value].text).id;
        product.category = categories.Find(p => p.name == dropdown_category.options[dropdown_category.value].text);
        product.categoryId = categories.Find(p => p.name == dropdown_category.options[dropdown_category.value].text).id;

        if (string.IsNullOrEmpty(product.imageURL) && !string.IsNullOrEmpty(text_filePath.text))
        {
            string newPath = Constants.ProductImage + "-" + product.name.Replace(" ", "") + "-" + product.id;
            File.Copy(text_filePath.text, Application.persistentDataPath + "/" + newPath, true);
            product.imageURL = newPath;
        }

        ProductsManager.Instance.UpdateProduct(product, product.id,
            (response) =>
            {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Success, Constants.ProductUpdated);
                if (ProductsManager.onProductUpdated != null) ProductsManager.onProductUpdated();
                GUIManager.Instance.Back();
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

    public void Button_EditImageClicked()
    {
        FileBrowser.Instance.OpenSingleFile();
    }
}
