using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MRExtensionMethods
{
    /// <summary>
    /// Resets Transform To (Position 0, Scale 1, Rotation 0)
    /// </summary>
    /// <param name="trans"></param>
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static string ToCommaSeparatedNumbers(this float number)
    {
        return number.ToString("#,##0.00");
    }
}
