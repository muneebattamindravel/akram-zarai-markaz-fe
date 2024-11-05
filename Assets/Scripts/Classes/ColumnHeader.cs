using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColumnHeader : MonoBehaviour
{
    [HideInInspector]
    public string dataField;
    ColumnState state = ColumnState.NONE;
    TMP_InputField filter;

    Sprite ascending, descending;
    Image image_state;

    public bool isViewLink, isCustomValue;

    private void Start()
    {
        dataField = gameObject.name;
        ascending = Resources.Load<Sprite>("ascending");
        descending = Resources.Load<Sprite>("descending");
        if (transform.Find("Image_State"))
            image_state = transform.Find("Image_State").GetComponent<Image>();
        if (transform.Find("InputField_Filter"))
            filter = transform.Find("InputField_Filter").GetComponent<TMP_InputField>();
        ResetState();
    }

    public void ResetState()
    {
        if (image_state)
            image_state.enabled = false;
        state = ColumnState.NONE;
    }

    public string GetFilterValue()
    {
        return filter.text;
    }

    public ColumnState SetNextState()
    {
        if (state == ColumnState.NONE)
        {
            state = ColumnState.ASCENDING;
            if (image_state)
            {
                image_state.sprite = ascending;
                image_state.enabled = true;
            }
        }
        else if (state == ColumnState.ASCENDING)
        {
            state = ColumnState.DESCENDING;
            if (image_state)
            {
                image_state.sprite = descending;
                image_state.enabled = true;
            }
        }
        else if (state == ColumnState.DESCENDING)
        {
            state = ColumnState.NONE;
            if (image_state)
            {
                image_state.enabled = false;
            }
        }

        return state;
    }

    public void SetState(ColumnState pState)
    {
        if (pState == ColumnState.ASCENDING)
        {
            state = ColumnState.ASCENDING;
            if (image_state)
            {
                image_state.sprite = ascending;
                image_state.enabled = true;
            }
        }
        else if (pState == ColumnState.DESCENDING)
        {
            state = ColumnState.DESCENDING;
            if (image_state)
            {
                image_state.sprite = descending;
                image_state.enabled = true;
            }
        }
        else if (pState == ColumnState.NONE)
        {
            state = ColumnState.NONE;
            if (image_state)
            {
                image_state.enabled = false;
            }
        }
    }
}

public enum ColumnState { ASCENDING, DESCENDING, NONE }
