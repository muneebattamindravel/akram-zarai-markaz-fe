using System.Collections.Generic;
using UnityEngine;

public class ADM : MonoBehaviour
{
    public static ADM I;
    //public bool isTestMode = false;
    public bool isViewOnlyMode = false;

    private void Awake()
    {
        if (I == null) I = this;
        else if (I != this) Destroy(gameObject);
    }
}
