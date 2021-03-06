﻿using System;
using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using GamePad = GamepadInput.GamePad;

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

    public static PlayerIndex? ToPlayerIndex(this GamePad.Index gamepadIndex)
    {
        if(gamepadIndex == GamePad.Index.Any) gamepadIndex = GamePad.Index.One;
        PlayerIndex? ret = null;// = (PlayerIndex) Enum.Parse(typeof (PlayerIndex), Enum.GetName(typeof (GamePad.Index), gamepadIndex)); //TOO SLOOOOOW
        switch(gamepadIndex)
        {
            case GamePad.Index.One:
                ret = PlayerIndex.One;
                break;
            case GamePad.Index.Two:
                ret = PlayerIndex.Two;
                break;
            case GamePad.Index.Three:
                ret = PlayerIndex.Three;
                break;
            case GamePad.Index.Four:
                ret = PlayerIndex.Four;
                break;
            default:
                break;
        }

        return ret;
    }

    public static Vector3 Clone(this Vector3 toClone)
    {
        return new Vector3(toClone.x, toClone.y, toClone.z);
    }
    public static Quaternion Clone(this Quaternion toClone)
    {
        return new Quaternion(toClone.x, toClone.y, toClone.z, toClone.w);
    }

    public static ButtonState GetButton(this GamePadState state, GamepadInput.GamePad.Button button)
    {
        ButtonState ret;
        switch (button)
        {
            case GamePad.Button.A:
                ret = state.Buttons.A;
                break;
            case GamePad.Button.B:
                ret = state.Buttons.B;
                break;
            case GamePad.Button.Back:
                ret = state.Buttons.Back;
                break;
            case GamePad.Button.LeftShoulder:
                ret = state.Buttons.LeftShoulder;
                break;
            case GamePad.Button.LeftStick:
                ret = state.Buttons.LeftStick;
                break;
            case GamePad.Button.RightShoulder:
                ret = state.Buttons.RightShoulder;
                break;
            case GamePad.Button.RightStick:
                ret = state.Buttons.RightStick;
                break;
            case GamePad.Button.Start:
                ret = state.Buttons.Start;
                break;
            case GamePad.Button.X:
                ret = state.Buttons.X;
                break;
            case GamePad.Button.Y:
                ret = state.Buttons.Y;
                break;
            default:
                ret = ButtonState.Released;
                Debug.LogError("ButtonNotFound");
                break;
        }
        return ret;

    }

}
