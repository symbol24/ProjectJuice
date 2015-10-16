using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ImpactEventArgs : EventArgs
{
    public float Damage { get; set; }
    public Color color { get; set; }
    public DamageType type { get; set; }
    /// <summary>
    /// This point is in worldCoordinates
    /// </summary>
    public Vector2 PointOfCollision { get; set; }
}
