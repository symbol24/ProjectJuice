using System;
using UnityEngine;
using System.Collections;

public class HpChangedEventArgs : EventArgs
{
    public float PreviousHp { get; set; }
    public float NewHp { get; set; }
}
