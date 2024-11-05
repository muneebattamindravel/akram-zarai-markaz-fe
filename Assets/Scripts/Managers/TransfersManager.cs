using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransfersManager : MonoBehaviour
{
    public static TransfersManager Instance;
    public delegate void TransfersEvent();
    public static TransfersEvent onTransferAdded, onTransferDeleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string TRANSFERS_ROUTE = "transfers";

    public void AddTransfer(Transfer transfer, ResponseAction<Transfer> successAction, ResponseAction<Transfer> failAction = null)
    {
        APIManager.Instance.Post<Transfer>(TRANSFERS_ROUTE, transfer, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetTransfers(MRDateRange range, ResponseAction<List<Transfer>> successAction, ResponseAction<List<Transfer>> failAction = null)
    {
        APIManager.Instance.Get<List<Transfer>>(TRANSFERS_ROUTE + "?from=" + range.from + "&to=" + range.to, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetTransfer(int transferId, ResponseAction<Transfer> successAction, ResponseAction<Transfer> failAction = null)
    {
        APIManager.Instance.Get<Transfer>(TRANSFERS_ROUTE + "/" + transferId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateTransfer(Transfer expense, int transferId, ResponseAction<Transfer> successAction, ResponseAction<Transfer> failAction = null)
    {
        APIManager.Instance.Patch<Transfer>(TRANSFERS_ROUTE + "/" + transferId, expense, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteTransfer(int transferId, ResponseAction<Transfer> successAction, ResponseAction<Transfer> failAction = null)
    {
        APIManager.Instance.Delete<Transfer>(TRANSFERS_ROUTE + "/" + transferId, (response) =>
        {
            successAction(response);
            onTransferAdded?.Invoke();
        }, (response) =>
        {
            failAction(response);
        });
    }
}
