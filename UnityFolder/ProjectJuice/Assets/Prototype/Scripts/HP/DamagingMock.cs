using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamagingMock : MonoBehaviour, IDamaging {
    [SerializeField] 
    private float _damage;
    [SerializeField]
    private bool _addImpactForce = false;
    [SerializeField]
    private Vector3 _preferredImpactPoint;
    [SerializeField]
    private bool _hasPreferredImpactPoint = false;
    [SerializeField] 
    private List<HPScript> _immuneTargets;
    [SerializeField] 
    private bool _hasImmuneTargets;
    [SerializeField]
    private ImpactForceSettings _impactForceSettings;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    public bool IsAvailableForConsumption { get { return true; }}
    public void Consumed()
    {
    }

    public float Damage
    {
        get { return _damage; }
        private set { _damage = value; }
    }

    public bool HasPreferredImpactPoint
    {
        get { return _hasPreferredImpactPoint; }
        private set { _hasPreferredImpactPoint = value; }
    }

    public Vector3 PreferredImpactPoint
    {
        get { return _preferredImpactPoint; }
        private set { _preferredImpactPoint = value; }
    }

    public bool AddImpactForce
    {
        get { return _addImpactForce; }
        private set { _addImpactForce = value; }
    }

    public ImpactForceSettings ImpactForceSettings
    {
        get { return _impactForceSettings; }
        private set { _impactForceSettings = value; }
    }

    public IEnumerable<HPScript> ImmuneTargets
    {
        get { return _immuneTargets; }
    }

    public bool HasImmuneTargets
    {
        get { return _hasImmuneTargets; }
        private set { _hasImmuneTargets = value; }
    }
}
