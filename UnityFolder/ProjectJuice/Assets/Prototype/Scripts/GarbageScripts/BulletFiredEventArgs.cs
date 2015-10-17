using UnityEngine;
using System.Collections;
using System;

public class BulletFiredEventArgs : EventArgs {
    public Bullet BulletFired { get; set; }
}
