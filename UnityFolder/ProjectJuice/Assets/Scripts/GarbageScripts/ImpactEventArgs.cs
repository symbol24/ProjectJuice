using System;
using UnityEngine;
using System.Collections;

public class ImpactEventArgs : EventArgs 
{
    public float Damage { get; set; }
    /// <summary>
    /// This point is in worldCoordinates
    /// </summary>
    public Vector2 PointOfCollision { get; set; }
}
