using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Screen_Units_View_Add : MonoBehaviour
{
    public TMP_InputField input_name, input_description;
    public Toggle toggle_allowDecimal;
    public TMP_Text text_title;
    ViewMode mode;
    Unit unit;

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
        text_title.text = Constants.Add + " " + Constants.Unit;
    }

    public void ShowView(int unitId)
    {
        mode = ViewMode.EDIT;
        text_title.text = Constants.Edit + " " + Constants.Unit;
        GetUnit(unitId);
    }

    void GetUnit(int unitId)
    {
        Preloader.Instance.ShowFull();
        UnitsManager.Instance.GetUnit(unitId,
            (response) => {
                Preloader.Instance.HideFull();
                unit = response.data;
                input_name.text = unit.name;
                input_description.text = unit.description;
                toggle_allowDecimal.isOn = unit.allowDecimal;
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
            GUIManager.Instance.ShowToast(Constants.Error, Constants.UnitNameEmpty, false);
            return;
        }

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            UnitsManager.Instance.AddUnit(new Unit(input_name.text, input_description.text, toggle_allowDecimal.isOn),
            (response) => {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Success, Constants.UnitAdded);
                if (UnitsManager.onUnitAdded != null) UnitsManager.onUnitAdded();
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
            unit.name = input_name.text;
            unit.description = input_description.text;
            unit.allowDecimal = toggle_allowDecimal.isOn;
            UnitsManager.Instance.UpdateUnit(unit, unit.id,
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.UnitUpdated);
                    if (UnitsManager.onUnitUpdated != null) UnitsManager.onUnitUpdated();
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
