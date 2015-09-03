using UnityEngine;
using System.Collections;

public class DamagingMock : MonoBehaviour, IDamaging {
    [SerializeField] private float _damage;

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

    public bool HasPreferredImpactPoint { get { return false; } }
    public Vector3 PreferredImpactPoint { get; private set; }
}
