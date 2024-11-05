using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ContactsManager : MonoBehaviour
{
    public static ContactsManager Instance;
    public delegate void ContactsEvent();

    public static ContactsEvent onContactAdded, onContactUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string CONTACTS_ROUTE = "contacts";
    string SUPPLIERS_ROUTE = "contacts/suppliers";
    string CUSTOMERS_ROUTE = "contacts/customers";

    public void AddContact(Contact contact, ResponseAction<Contact> successAction, ResponseAction<Contact> failAction = null)
    {
        APIManager.Instance.Post<Contact>(CONTACTS_ROUTE, contact, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetContact(int contactId, ResponseAction<Contact> successAction, ResponseAction<Contact> failAction = null)
    {
        APIManager.Instance.Get<Contact>(CONTACTS_ROUTE + "/" + contactId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetContacts(ResponseAction<List<Contact>> successAction, ResponseAction<List<Contact>> failAction = null)
    {
        APIManager.Instance.Get<List<Contact>>(CONTACTS_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetAllSuppliers(ResponseAction<List<Contact>> successAction, ResponseAction<List<Contact>> failAction = null)
    {
        APIManager.Instance.Get<List<Contact>>(SUPPLIERS_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetAllCustomers(ResponseAction<List<Contact>> successAction, ResponseAction<List<Contact>> failAction = null)
    {
        APIManager.Instance.Get<List<Contact>>(CUSTOMERS_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateContact(Contact contact, int contactId, ResponseAction<Contact> successAction, ResponseAction<Contact> failAction = null)
    {
        APIManager.Instance.Patch<Contact>(CONTACTS_ROUTE + "/" + contactId, contact, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    //public void DeleteContact(int contactId, ResponseAction<Contact> successAction, ResponseAction<Contact> failAction = null)
    //{
    //    APIManager.Instance.Delete<Contact>(CONTACTS_ROUTE + "/" + contactId, (response) =>
    //    {
    //        successAction(response);
    //    }, (response) =>
    //    {
    //        failAction(response);
    //    });
    //}
}
