using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public interface IDamaging : IConsumable
{
    float Damage { get; }
    DamageType TypeOfDamage { get; }
    bool HasPreferredImpactPoint { get; }
    Vector3 PreferredImpactPoint { get; }
    bool AddImpactForce { get; }
    //Vector2 ImpactForce { get; }
    ImpactForceSettings ImpactForceSettings { get; }
    //float TimeToApplyForce { get; }
    IEnumerable<HPScript> ImmuneTargets { get; }
    bool HasImmuneTargets { get; }
    void UpdateImpactForceSetting(ImpactForceSettings toUpdate);
    int BulletsToGiveShield { get; }
}
