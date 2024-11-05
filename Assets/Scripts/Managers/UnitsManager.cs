using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;
    public delegate void UnitsEvent();

    public static UnitsEvent onUnitAdded, onUnitUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string UNITS_ROUTE = "units";

    public void AddUnit(Unit unit, ResponseAction<Unit> successAction, ResponseAction<Unit> failAction = null)
    {
        APIManager.Instance.Post<Unit>(UNITS_ROUTE, unit, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetUnit(int unitId, ResponseAction<Unit> successAction, ResponseAction<Unit> failAction = null)
    {
        APIManager.Instance.Get<Unit>(UNITS_ROUTE + "/" + unitId, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void GetUnits(ResponseAction<List<Unit>> successAction, ResponseAction<List<Unit>> failAction = null)
    {
        APIManager.Instance.Get<List<Unit>>(UNITS_ROUTE, (response) => {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void UpdateUnit(Unit unit, int unitId, ResponseAction<Unit> successAction, ResponseAction<Unit> failAction = null)
    {
        APIManager.Instance.Patch<Unit>(UNITS_ROUTE + "/" + unitId, unit, (response) =>
        {
            successAction(response);
        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }

    public void DeleteUnit(int unitId, ResponseAction<Unit> successAction, ResponseAction<Unit> failAction = null)
    {
        APIManager.Instance.Delete<Unit>(UNITS_ROUTE + "/" + unitId, (response) =>
        {
            successAction(response);
        }, (response) =>
        {
            failAction(response);
        });
    }
}
