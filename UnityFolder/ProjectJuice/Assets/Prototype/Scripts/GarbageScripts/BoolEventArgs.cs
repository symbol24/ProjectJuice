using UnityEngine;
using System.Collections;
using System;

public class BoolEventArgs : EventArgs
{
    public bool PreviousBoolValue { get; set; }
    public bool NewBoolValue { get; set; }
}
