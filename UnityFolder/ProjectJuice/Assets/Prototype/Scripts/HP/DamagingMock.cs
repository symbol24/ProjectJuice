using UnityEngine;
using System.Collections;

public class DamagingMock : MonoBehaviour, IDamaging {
    [SerializeField] 
    private float _damage;
    [SerializeField]
    private Vector2 _impactForce;
    [SerializeField]
    private bool _addImpactForce = false;
    [SerializeField]
    private Vector3 _preferredImpactPoint;
    [SerializeField]
    private bool _hasPreferredImpactPoint = false;

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

    public Vector2 ImpactForce
    {
        get { return _impactForce; }
        private set { _impactForce = value; }
    }
}
