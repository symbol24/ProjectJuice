using UnityEngine;
using System.Collections;

public static class Extensions
{
    /// <summary>
    /// Returns the angle normalized to -180/180 degrees
    /// </summary>
    /// <param name="angleToNormalize"></param>
    /// <returns></returns>
    public static float ToNormalizedAngle(this float angleToNormalize)
    {
        var three60Angle = angleToNormalize%360f;
        var ret = three60Angle > 180 ? three60Angle - 360 : three60Angle;
        return ret;
    }

    public static void SetRotationEulerZ(this GameObject gameObjectToRotate, float zValue)
    {
        gameObjectToRotate.transform.rotation = Quaternion.Euler(gameObjectToRotate.transform.rotation.eulerAngles.x, gameObjectToRotate.transform.rotation.eulerAngles.y, zValue);
    }

    public static Vector3 SetX(this Vector3 toSet, float xValue)
    {
        return new Vector3(xValue, toSet.y, toSet.z);
    }

    public static Vector2 ToVector2(this Vector3 toChange)
    {
        return new Vector2(toChange.x, toChange.y);
    }

    public static Vector3 Midpoint(this Vector3 source, Vector3 otherVector)
    {
        var midlePoint = source/2 + otherVector/2;
        return midlePoint;
    }

}
