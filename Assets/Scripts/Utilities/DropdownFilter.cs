using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownFilter : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TMP_Dropdown dropdown;

    public List<TMP_Dropdown.OptionData> dropdownOptions;

    private void Start()
    {
        dropdownOptions = dropdown.options;
    }
    public void FilterDropdown(string input)
    {
        dropdown.options = dropdownOptions.FindAll(option => option.text.IndexOf(input) >= 0);
    }
}
