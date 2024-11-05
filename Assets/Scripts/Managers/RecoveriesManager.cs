using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveriesManager : MonoBehaviour
{
    public static RecoveriesManager Instance;
    public delegate void RecoveriesEvent();
    public static RecoveriesEvent onRecoveryAdded, onRecoveryDeleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string RECOVERIES_ROUTE = "recoveries";

    public void CreateRecovery(Recovery recovery, ResponseAction<Recovery> successAction, ResponseAction<Recovery> failAction = null)
    {
        APIManager.Instance.Post<Recovery>(RECOVERIES_ROUTE, recovery, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteRecovery(int recoveryId, ResponseAction<Recovery> successAction, ResponseAction<Recovery> failAction = null)
    {
        APIManager.Instance.Delete<Recovery>(RECOVERIES_ROUTE + "/" + recoveryId, (response) =>
        {
            successAction(response);
            onRecoveryDeleted?.Invoke();
        }, (response) =>
        {
            failAction(response);
        });
    }

    public void GetRecoveries(MRDateRange range, ResponseAction<List<Recovery>> successAction, ResponseAction<List<Recovery>> failAction = null)
    {
        APIManager.Instance.Get<List<Recovery>>(RECOVERIES_ROUTE + "?from=" + range.from + "&to=" + range.to, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetRecovery(int recoveryId, ResponseAction<Recovery> successAction, ResponseAction<Recovery> failAction = null)
    {
        APIManager.Instance.Get<Recovery>(RECOVERIES_ROUTE + "/" + recoveryId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
