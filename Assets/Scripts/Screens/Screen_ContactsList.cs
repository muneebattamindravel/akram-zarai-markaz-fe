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

public class Screen_ContactsList : OSA<BaseParamsWithPrefab, ContactsListViewHolder>
{
    public GameObject contentRoot;
    public List<Contact> contacts, contactsFiltered;
    public List<ColumnHeader> columnHeaders;
    public TMP_Dropdown dropdown_contactType;

    public SimpleDataHelper<Contact> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Contact>(this);
        base.Start();
    }

    protected override ContactsListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new ContactsListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(ContactsListViewHolder newOrRecycled)
    {
        Contact contact = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.text = contact.id.ToString();
        newOrRecycled.name.text = contact.name.ToString();
        newOrRecycled.type.text = contact.type.ToString();
        newOrRecycled.number.text = contact.number.ToString();
        newOrRecycled.notes.text = contact.notes.ToString();

        newOrRecycled.name.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.name.GetComponent<MRButton>().onClicked.AddListener(() => {
            ViewContact(contact.id);
        });

        newOrRecycled.edit.onClicked.RemoveAllListeners();
        newOrRecycled.edit.onClicked.AddListener(() => {
            ViewContact(contact.id);
        });

        if (contact.accountId != null)
        {
            newOrRecycled.viewStatement.gameObject.SetActive(true);
            newOrRecycled.viewStatement.onClicked.RemoveAllListeners();
            newOrRecycled.viewStatement.onClicked.AddListener(() =>
            {
                ViewAccountStatement(contact.accountId);
            });
        }
        else
            newOrRecycled.viewStatement.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GetContacts();
        ContactsManager.onContactAdded += GetContacts;
        ContactsManager.onContactUpdated += GetContacts;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        ContactsManager.onContactAdded -= GetContacts;
        ContactsManager.onContactUpdated -= GetContacts;

        contacts = null;
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
                FieldInfo fieldInfo = typeof(Contact).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    contacts = contacts.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    contacts = contacts.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    contacts = contacts.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Contact item in contacts) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Contact).GetField(header.dataField);
                foreach (Contact filtered in contacts.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetContacts()
    {
        Preloader.Instance.ShowWindowed();
        ContactsManager.Instance.GetContacts((response) => {
            contacts = response.data;
            contactsFiltered = contacts.FindAll(p => p.type.ToString() == dropdown_contactType.options[dropdown_contactType.value].text);

            dropdown_contactType.onValueChanged.AddListener((changedValue) => {
                contactsFiltered = contacts.FindAll(p => p.type.ToString() == dropdown_contactType.options[dropdown_contactType.value].text);
                PopulateData();
            });

            columnHeaders[0].ResetState();
            columnHeaders[0].SetNextState();
            PopulateData();
        });
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();
        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, contactsFiltered.FindAll(p => p.IsEnabledOnGrid));
        Preloader.Instance.HideWindowed();
    }

    void ViewAccountStatement(int? accountId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.AccountStatement);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_AccountStatement>().ShowView(accountId);
    }

    void ViewContact(int contactId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Contacts_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ContactsView_Add>().ShowView(contactId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Contacts_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ContactsView_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetContacts();
    }
}

public class ContactsListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, name, type, number, notes;
    public MRButton edit, viewStatement;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("name", out name);
        root.GetComponentAtPath("type", out type);
        root.GetComponentAtPath("number", out number);
        root.GetComponentAtPath("notes", out notes);
        root.GetComponentAtPath("Button_Edit", out edit);
        root.GetComponentAtPath("Button_ViewStatement", out viewStatement);
    }
}

