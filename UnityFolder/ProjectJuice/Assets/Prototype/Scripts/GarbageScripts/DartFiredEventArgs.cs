using UnityEngine;
using System.Collections;
using System;

public class DartFiredEventArgs : EventArgs
{
    public IDart Dart { get; set; }
}
