using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string DATA_BACKUP = "data/backup";

    public void BackUpAndUpload(Backup backup, ResponseAction<Backup> successAction, ResponseAction<Backup> failAction = null)
    {
        APIManager.Instance.Post<Backup>(DATA_BACKUP, backup, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
