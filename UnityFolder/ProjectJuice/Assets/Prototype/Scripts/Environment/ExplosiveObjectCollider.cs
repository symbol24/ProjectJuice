using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ExplosiveObjectCollider : ExtendedMonobehaviour, IDamaging
{

    public ExplosiveObject _explosiveObject;

    
    public int BulletsToGiveShield
    {
        get { return _explosiveObject._bulletsToGive; }
    }

    public bool IsAvailableForConsumption
    {
        get { return _explosiveObject.IsAvailableForConsumption; }
    }

    void Start()
    {
        _consumerObjects = new List<Component>();
    }

    private List<Component> _consumerObjects;
    bool IConsumable.IsAvailableForConsumption(object sender)
    {
        var mitigateRet = false;
        var senderAsGameobject = sender as Component;
        if (!_consumerObjects.Any(c => c.GetInstanceID() == senderAsGameobject.GetInstanceID()))
        {
            _consumerObjects.Add(senderAsGameobject);
            mitigateRet = true;
        }
        return IsAvailableForConsumption && mitigateRet;
    }
    public void Consumed()
    {
        _explosiveObject.Consumed();
    }

    public void UpdateImpactForceSetting(ImpactForceSettings toUpdate)
    {
        
    }

    public float Damage
    {
        get { return _explosiveObject.Damage; }
    }
    
    public bool HasPreferredImpactPoint
    {
        get { return _explosiveObject.HasPreferredImpactPoint; }
    }
    public Vector3 PreferredImpactPoint
    {
        get { return _explosiveObject.PreferredImpactPoint; }
    }
    public bool AddImpactForce
    {
        get { return _explosiveObject.AddImpactForce; }
    }
    public ImpactForceSettings ImpactForceSettings
    {
        get { return _explosiveObject.ImpactForceSettings; }
    }
    public IEnumerable<HPScript> ImmuneTargets
    {
        get { return _explosiveObject.ImmuneTargets; }
    }
    public bool HasImmuneTargets
    {
        get { return _explosiveObject.HasImmuneTargets; }
    }
}