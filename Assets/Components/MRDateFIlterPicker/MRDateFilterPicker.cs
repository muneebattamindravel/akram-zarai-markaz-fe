using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Bitsplash.DatePicker;

public class MRDateRange
{
    public DateTime from, to;
    public MRDateRange(DateTime pFrom, DateTime pTo)
    {
        this.from = pFrom;
        this.to = pTo;
    }
}

[System.Serializable]
public enum DateType
{
    Today = 0, Yesterday = 1, Last7Days = 2, Last30Days = 3, Last90Days = 4, CurrentMonth = 5, CurrentYear = 6, LastYear = 7, Custom = 8, AllTime = 9
}

public class MRDateFilterPicker : MonoBehaviour
{
    public TMP_Text text_datePickerTitle;

    public GameObject dropdownContainer;

    public delegate void MRDateFilterEvent();
    public MRDateFilterEvent onDateSelected = null;

    public DateType defaultDateType = DateType.CurrentMonth;

    DateTime from, to;

    [SerializeField] MRDatePicker picker_from, picker_to;

    public MRDateRange GetDateRange()
    {
        //to = to.AddSeconds(86399);
        return new MRDateRange(from, to);
    }

    private void OnEnable()
    {
        ResetDates();

        dropdownContainer.SetActive(false);
        text_datePickerTitle.text = "<b>TODAY</b> " + this.from.ToString(Constants.DateDisplayFormat);
        Button_DateSelected((int)defaultDateType);

        picker_from.OnSelectionChanged.AddListener(() => {
            from = picker_from.SelectedDate;
        });

        picker_to.OnSelectionChanged.AddListener(() => {
            to = picker_to.SelectedDate;
        });

        
    }

    void ResetDates()
    {
        picker_from.Reset();
        picker_to.Reset();
        from = DateTime.MinValue;
        to = DateTime.MinValue;
    }

    public void Button_PickerClicked()
    {
        dropdownContainer.SetActive(true);
        ResetDates();
    }

    public void ClosePicker()
    {
        dropdownContainer.SetActive(false);
    }

    public void Button_DateSelected(int dateType)
    {
        switch ((DateType) dateType)
        {
            case (DateType.Today):
                SetCurrentRange(DateTime.Today, DateTime.Today);
                text_datePickerTitle.text = "<b>TODAY</b> " + this.from.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.Yesterday):
                SetCurrentRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));
                text_datePickerTitle.text = "<b>YESTERDAY</b> " + this.from.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.Last7Days):
                SetCurrentRange(DateTime.Today.AddDays(-7), DateTime.Today);
                text_datePickerTitle.text = "<b>LAST 7 DAYS</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.Last30Days):
                SetCurrentRange(DateTime.Today.AddDays(-30), DateTime.Today);
                text_datePickerTitle.text = "<b>LAST 30 DAYS</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.Last90Days):
                SetCurrentRange(DateTime.Today.AddDays(-90), DateTime.Today);
                text_datePickerTitle.text = "<b>LAST 90 DAYS</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.CurrentMonth):
                DateTime now = DateTime.Now;
                SetCurrentRange(new DateTime(now.Year, now.Month, 1), DateTime.Today);
                text_datePickerTitle.text = "<b>CURRENT MONTH</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.CurrentYear):
                now = DateTime.Now;
                SetCurrentRange(new DateTime(now.Year, 1, 1), DateTime.Today);
                text_datePickerTitle.text = "<b>CURRENT YEAR</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.LastYear):
                now = DateTime.Now;
                SetCurrentRange(new DateTime((now.Year - 1), 1, 1), new DateTime(now.Year, 1, 1).AddDays(-1));
                text_datePickerTitle.text = "<b>LAST YEAR</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                break;

            case (DateType.Custom):
                if (from != DateTime.MinValue && to != DateTime.MinValue) {
                    SetCurrentRange(from, to);
                    text_datePickerTitle.text = "<b>CUSTOM RANGE</b> (" + this.from.ToString(Constants.DateDisplayFormat) + " <b>to</b> " + this.to.ToString(Constants.DateDisplayFormat);
                }
                else
                {
                    text_datePickerTitle.text = "SELECT CORRECT FROM & TO DATE";
                }
                break;

            case (DateType.AllTime):
                SetCurrentRange(new DateTime(2001, 1, 1), DateTime.Today);
                text_datePickerTitle.text = "<b>ALL TIME</b>";
                break;
        }
    }

    void SetCurrentRange(DateTime pFrom, DateTime pTo)
    {
        from = pFrom;
        to = pTo;

        Debug.Log("Date Range Set = " + this.from.ToString(Constants.DateDisplayFormat) + " , " + this.to.ToString(Constants.DateDisplayFormat));

        dropdownContainer.SetActive(false);
        onDateSelected?.Invoke();
    }
}
