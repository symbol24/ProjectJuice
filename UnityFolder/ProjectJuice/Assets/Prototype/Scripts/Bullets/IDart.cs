using UnityEngine;
using System.Collections;
using System;

public interface IDart : IGameObject
{
    event EventHandler DartCollision;
    event EventHandler<JuiceSuckedEventArgs> JuiceSucked;
    event EventHandler DartDestroyed;
    HPScript Target { get; }
}
