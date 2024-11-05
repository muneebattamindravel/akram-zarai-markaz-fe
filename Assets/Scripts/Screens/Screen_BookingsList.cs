using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

public class Screen_BookingsList : OSA<BaseParamsWithPrefab, BookingsListViewHolder>
{
    public GameObject contentRoot;
    public List<Booking> bookings;
    public List<ColumnHeader> columnHeaders;
    public TMP_Text text_totalBookingsAmount;

    public SimpleDataHelper<Booking> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Booking>(this);
        base.Start();
    }

    protected override BookingsListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new BookingsListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(BookingsListViewHolder newOrRecycled)
    {
        Booking booking = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = booking.id.ToString();
        newOrRecycled.totalAmount.text = booking.totalAmount.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.prNumber.text = booking.prNumber.ToString();
        newOrRecycled.company.text = booking.company.name;
        newOrRecycled.bookingDate.text = booking.bookingDate.ToString(Constants.DateDisplayFormat);

        newOrRecycled.button_view.onClicked.RemoveAllListeners();
        newOrRecycled.button_view.onClicked.AddListener(() => {
            ViewBooking(booking.id);
        });

        newOrRecycled.button_delete.onClicked.RemoveAllListeners();
        newOrRecycled.button_delete.onClicked.AddListener(() => {
            DeleteBooking(booking.id);
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        float totalBookingsAmount = 0f;
        foreach (Booking b in bookings)
            totalBookingsAmount += b.totalAmount;
        text_totalBookingsAmount.text = totalBookingsAmount.ToCommaSeparatedNumbers();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, bookings.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    private void OnEnable()
    {
        GetBookings();
        BookingsManager.onBookingAdded += GetBookings;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        BookingsManager.onBookingAdded -= GetBookings;
        bookings = null;
        GC.Collect();
    }

    public void InitializeColumnsHeaders()
    {
        foreach (ColumnHeader header in columnHeaders)
        {
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.RemoveAllListeners();
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.AddListener(() => {
                foreach (ColumnHeader hdr in columnHeaders)
                    if (hdr != header)
                        hdr.ResetState();

                ColumnState nextState = header.SetNextState();
                FieldInfo fieldInfo = typeof(Booking).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    bookings = bookings.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    bookings = bookings.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    bookings = bookings.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {

                foreach (Booking item in bookings) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Booking).GetField(header.dataField);
                foreach (Booking filtered in bookings.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetBookings()
    {
        Preloader.Instance.ShowWindowed();
        BookingsManager.Instance.GetBookings((response) => {
            bookings = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        });
    }

    void ViewBooking(int bookingId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Bookings_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Bookings_View_Add>().ShowView(bookingId, true);
    }

    void DeleteBooking(int bookingId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Loan,
            Constants.DELETE, Constants.CANCEL,
            () => {
                //Confirmed
                Preloader.Instance.ShowFull();
                BookingsManager.Instance.DeleteBooking(bookingId, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.BookingDeleted);
                    GetBookings();
                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
                });
            },
            () => {
                //Cancelled
            }
        );
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Bookings_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Bookings_View_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetBookings();
    }
}

public class BookingsListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, totalAmount, prNumber, bookingDate, company, notes;
    public MRButton button_view, button_delete;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("totalAmount", out totalAmount);
        root.GetComponentAtPath("prNumber", out prNumber);
        root.GetComponentAtPath("bookingDate", out bookingDate);
        root.GetComponentAtPath("company", out company);
        root.GetComponentAtPath("notes", out notes);
        root.GetComponentAtPath("Button_View", out button_view);
        root.GetComponentAtPath("Button_Delete", out button_delete);
    }
}

