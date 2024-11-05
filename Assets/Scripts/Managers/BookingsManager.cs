using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BookingsManager : MonoBehaviour
{
    public static BookingsManager Instance;
    public delegate void BookingsEvent();

    public static BookingsEvent onBookingAdded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string BOOKINGS_ROUTE = "bookings";

    public void AddBooking(Booking booking, ResponseAction<Booking> successAction, ResponseAction<Booking> failAction = null)
    {
        APIManager.Instance.Post<Booking>(BOOKINGS_ROUTE, booking, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetBooking(int bookingId, ResponseAction<Booking> successAction, ResponseAction<Booking> failAction = null)
    {
        APIManager.Instance.Get<Booking>(BOOKINGS_ROUTE + "/" + bookingId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetBookings(ResponseAction<List<Booking>> successAction, ResponseAction<List<Booking>> failAction = null)
    {
        APIManager.Instance.Get<List<Booking>>(BOOKINGS_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateBooking(Booking booking, int bookingId, ResponseAction<Booking> successAction, ResponseAction<Booking> failAction = null)
    {
        APIManager.Instance.Patch<Booking>(BOOKINGS_ROUTE + "/" + bookingId, booking, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteBooking(int bookingId, ResponseAction<Booking> successAction, ResponseAction<Booking> failAction = null)
    {
        APIManager.Instance.Delete<Booking>(BOOKINGS_ROUTE + "/" + bookingId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }
}
