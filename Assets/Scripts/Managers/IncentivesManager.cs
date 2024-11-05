using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncentivesManager : MonoBehaviour
{
    public static IncentivesManager Instance;
    public delegate void IncentivesEvent();
    public static IncentivesEvent onIncentiveAdded, onIncentiveDeleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string INCENTIVES_ROUTE = "incentives";

    public void CreateIncentive(Incentive incentive, ResponseAction<Incentive> successAction, ResponseAction<Incentive> failAction = null)
    {
        APIManager.Instance.Post<Incentive>(INCENTIVES_ROUTE, incentive, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetIncentives(MRDateRange range, ResponseAction<List<Incentive>> successAction, ResponseAction<List<Incentive>> failAction = null)
    {
        APIManager.Instance.Get<List<Incentive>>(INCENTIVES_ROUTE + "?from=" + range.from + "&to=" + range.to, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetIncentive(int incentiveId, ResponseAction<Incentive> successAction, ResponseAction<Incentive> failAction = null)
    {
        APIManager.Instance.Get<Incentive>(INCENTIVES_ROUTE + "/" + incentiveId, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteIncentive(int incentiveId, ResponseAction<Incentive> successAction, ResponseAction<Incentive> failAction = null)
    {
        APIManager.Instance.Delete<Incentive>(INCENTIVES_ROUTE + "/" + incentiveId, (response) =>
        {
            successAction(response);
            onIncentiveDeleted?.Invoke();
        }, (response) =>
        {
            failAction(response);
        });
    }
}
