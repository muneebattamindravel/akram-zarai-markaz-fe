using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MRContact
{
    public string name;
    public string phone;
}

public class MRContactsManager : MonoBehaviour
{
    public static MRContactsManager Instance = null;
    private void Awake()
    {
        Instance = this;
        contacts = new List<MRContact>();
    }

    private void Start()
    {
        LoadContacts();
    }

    public List<MRContact> contacts;

    public void AddToList(MRContact contactInfo)
    {
        contacts.Add(contactInfo);
    }

    public void LoadContacts()
    {
        for (int i = 0; i < 100; i++)
        {
            MRContact c = new MRContact();
            c.name = "Muneeb Atta";
            c.phone = "+923324405112";
            AddToList(c);

            MRContact d = new MRContact();
            d.name = "Muneeb Atta With 0";
            d.phone = "+9203324405112";
            AddToList(d);

            c = new MRContact();
            c.name = "Sumbal Rizwan";
            c.phone = "+923454035620";
            AddToList(c);

            c = new MRContact();
            c.name = "Umer Nasir";
            c.phone = "+923454283949";
            AddToList(c);

            c = new MRContact();
            c.name = "Ahmad Saleem";
            c.phone = "+923324964196";
            AddToList(c);

            c = new MRContact();
            c.name = "Majid Ibrahim";
            c.phone = "+923460347014";
            AddToList(c);

            c = new MRContact();
            c.name = "Irshad Ali";
            c.phone = "+923078304442";
            AddToList(c);
        }
    }
}
