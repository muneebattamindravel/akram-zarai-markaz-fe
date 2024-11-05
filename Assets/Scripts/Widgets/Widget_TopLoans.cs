using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Widget_TopLoans : MonoBehaviour
{
    public GameObject itemPrefab, container;
    public GameObject loader, body;
    List<Account> customerAccounts;

    private void OnEnable()
    {
        body.SetActive(false);
        loader.SetActive(true);

        ClearContent();

        DashboardManager.Instance.GetTopLoans(
        (response) =>
        {
            customerAccounts = response.data;
            customerAccounts = customerAccounts.OrderByDescending(p => p.balance).ToList();

            for (int i = 0; i < 7; i++)
            {
                GameObject obj = Instantiate(itemPrefab, container.transform);
                obj.SetActive(true);
                obj.GetComponent<TMP_Text>().text = customerAccounts[i].name.Replace("Account","");
                obj.transform.Find("Amount").GetComponent<TMP_Text>().text = customerAccounts[i].balance.ToCommaSeparatedNumbers();
            }

            body.SetActive(true);
            loader.SetActive(false);
        },
        (response) =>
        {
        });
    }

    void ClearContent()
    {
        foreach (Transform t in container.transform)
            Destroy(t.gameObject);
    }
}
