using System;
using System.Collections;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class MRDatePicker : MonoBehaviour
{
    public bool allowFutureDates = false;

    DatePickerSettings datePickerSettings;
    [SerializeField] TMP_Text text_date;
    GameObject datePickerUIObject;

    [SerializeField] GameObject datePickerUIPrefab;

    public UnityEvent OnSelectionChanged;

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        SelectedDate = DateTime.MinValue;
        if (datePickerUIObject != null)
            Destroy(datePickerUIObject);
        text_date.text = string.Empty;
    }

    DateTime _dateTime;
    public DateTime SelectedDate {
        get
        {
            return _dateTime;
        }
        set
        {
            if (!allowFutureDates && value > DateTime.Now)
            {
                GUIManager.Instance.ShowToast(Constants.Failed, Constants.FutureDateError, false);
                _dateTime = DateTime.MinValue;
                text_date.text = string.Empty;
                return;
            }

            _dateTime = value;
            text_date.text = _dateTime.ToString("dd-MM-yyyy");
        }
    }

    public new bool enabled
    {
        set
        {
            GetComponent<Button>().interactable = value;
        }
    }

    public void Show()
    {
        Show(null);
    }

    public void Show(Action<DateTime> onSelectionChanged = null)
    {
        datePickerUIObject = Instantiate(datePickerUIPrefab);
        datePickerUIObject.gameObject.SetActive(true);
        datePickerSettings = datePickerUIObject.transform.GetComponentInChildren<DatePickerSettings>();

        datePickerSettings.Content.OnSelectionChanged.AddListener(() =>
        {
            SelectedDate = datePickerSettings.Content.Selection.GetItem(0);
            onSelectionChanged?.Invoke(datePickerSettings.Content.Selection.GetItem(0));
            OnSelectionChanged?.Invoke();
            Close();
        });
    }

    public void Close()
    {
        if (datePickerUIObject != null)
            Destroy(datePickerUIObject);
    }
}
