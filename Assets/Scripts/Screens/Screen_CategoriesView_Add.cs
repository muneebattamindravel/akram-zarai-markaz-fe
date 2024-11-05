using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class Screen_CategoriesView_Add : MonoBehaviour
{
    public TMP_InputField input_name, input_description;
    public TMP_Text text_title;
    ViewMode mode;
    Category category;

    private void OnEnable()
    {
        input_name.text = ""; input_description.text = "";
        KeyboardManager.enterPressed += OnEnterPressed;
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void ShowView()
    {
        mode = ViewMode.ADD;
        text_title.text = Constants.Add + " " + Constants.Category;
    }

    public void ShowView(int categoryId, bool viewOnly = false)
    {
        mode = ViewMode.EDIT;
        text_title.text = Constants.Edit + " " + Constants.Category;
        GetCategory(categoryId);
    }

    void GetCategory(int categoryId)
    {
        Preloader.Instance.ShowFull();
        CategoriesManager.Instance.GetCategory(categoryId,
            (response) => {
                Preloader.Instance.HideFull();
                category = response.data;
                input_name.text = category.name;
                input_description.text = category.description;
            },
            (response) => {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
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
            GUIManager.Instance.ShowToast(Constants.Error, Constants.CategoryNameEmpty, false);
            return;
        }

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            CategoriesManager.Instance.AddCategory(new Category(input_name.text, input_description.text),
            (response) => {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Success, Constants.CategoryAdded);
                if (CategoriesManager.onCategoryAdded != null) CategoriesManager.onCategoryAdded();
                GUIManager.Instance.Back();
            },
            (response) =>
            {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
            }
            );
        }
        else if (mode == ViewMode.EDIT)
        {
            category.name = input_name.text;
            category.description = input_description.text;
            CategoriesManager.Instance.UpdateCategory(category, category.id,
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.CategoryUpdated);
                    if (CategoriesManager.onCategoryUpdated != null) CategoriesManager.onCategoryUpdated();
                    GUIManager.Instance.Back();
                },
                (response) =>
                {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                }
            );
        }
    }

    public void Button_CloseClicked()
    {
        GUIManager.Instance.Back();
    }
}
