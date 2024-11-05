using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class Screen_ContactsView_Add : MonoBehaviour
{
    public TMP_InputField input_name, input_businessName, input_number, input_email, input_address, input_notes;
    public TMP_Dropdown dropdown_type;
    public TMP_Text text_title;
    public TMP_InputField input_openingBalance;
    ViewMode mode;
    Contact contact;

    private void OnEnable()
    {
        input_openingBalance.gameObject.SetActive(false);
        input_name.text = "";
        dropdown_type.value = 0;
        input_businessName.text = "";
        input_number.text = "";
        input_email.text = "";
        input_address.text = "";
        input_notes.text = "";
        KeyboardManager.enterPressed += OnEnterPressed;

        if (dropdown_type.value == 0)
            input_openingBalance.gameObject.SetActive(true);
        else
            input_openingBalance.gameObject.SetActive(false);

        input_openingBalance.text = "";
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterPressed;
    }

    public void ShowView()
    {
        mode = ViewMode.ADD;
        text_title.text = Constants.Add + " " + Constants.Contact;
        input_openingBalance.enabled = true;
        dropdown_type.enabled = true;
    }

    public void ShowView(int contactId, bool viewOnly = false)
    {
        mode = ViewMode.EDIT;
        text_title.text = Constants.Edit + " " + Constants.Contact;
        dropdown_type.enabled = false;

        input_openingBalance.enabled = false;

        GetContact(contactId);
    }

    void GetContact(int contactId)
    {
        Preloader.Instance.ShowFull();
        ContactsManager.Instance.GetContact(contactId,
            (response) => {
                Preloader.Instance.HideFull();
                contact = response.data;
                input_name.text = contact.name;
                dropdown_type.value = dropdown_type.options.FindIndex(p => p.text == contact.type);
                input_businessName.text = contact.businessName;
                input_number.text = contact.number;
                input_email.text = contact.email;
                input_address.text = contact.address;
                input_notes.text = contact.notes;
                input_openingBalance.text = contact.openingBalance.ToString();

                if (dropdown_type.value == 0)
                    input_openingBalance.gameObject.SetActive(true);
                else
                    input_openingBalance.gameObject.SetActive(false);
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

    public void TypeChanged()
    {
        if (dropdown_type.value == 0)
            input_openingBalance.gameObject.SetActive(true);
        else
            input_openingBalance.gameObject.SetActive(false);
    }

    public void Button_SaveClicked()
    {
        if (string.IsNullOrEmpty(input_name.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.ContactNameEmpty, false);
            return;
        }

        if (dropdown_type.value == 0)
        {
            if (string.IsNullOrEmpty(input_openingBalance.text))
            {
                GUIManager.Instance.ShowToast(Constants.Error, Constants.OpeningBalanceEmpty, false);
                return;
            }
        }
        else
        {
            input_openingBalance.text = "0";
        }

        Preloader.Instance.ShowFull();
        if (mode == ViewMode.ADD)
        {
            ContactsManager.Instance.AddContact(
                new Contact(
                    input_name.text,
                    dropdown_type.options[dropdown_type.value].text,
                    input_businessName.text,
                    input_number.text,
                    input_email.text,
                    input_address.text,
                    input_notes.text,
                    float.Parse(input_openingBalance.text)
                    ),
            (response) => {
                Preloader.Instance.HideFull();
                GUIManager.Instance.ShowToast(Constants.Success, Constants.ContactAdded);
                if (ContactsManager.onContactAdded != null) ContactsManager.onContactAdded();
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
            contact.name = input_name.text;
            contact.type = dropdown_type.options[dropdown_type.value].text;
            contact.businessName = input_businessName.text;
            contact.number = input_number.text;
            contact.email = input_email.text;
            contact.address = input_address.text;
            contact.notes = input_notes.text;
            contact.openingBalance = float.Parse(input_openingBalance.text);

            ContactsManager.Instance.UpdateContact(contact, contact.id,
                (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.ContactUpdated);
                    if (ContactsManager.onContactUpdated != null) ContactsManager.onContactUpdated();
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
