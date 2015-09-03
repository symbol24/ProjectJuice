﻿using UnityEngine;
using System.Collections;

public interface IDamaging : IConsumable
{
    float Damage { get; }
    bool HasPreferredImpactPoint { get; }
    Vector3 PreferredImpactPoint { get; }
    bool AddImpactForce { get; }
    Vector2 ImpactForce { get; }
}
