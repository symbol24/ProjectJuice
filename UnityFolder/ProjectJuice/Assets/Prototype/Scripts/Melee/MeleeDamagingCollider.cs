using UnityEngine;
using System.Collections;

public class MeleeDamagingCollider : MonoBehaviour, IDamaging
{

    public MeleeAttack _meleeAttack;
    public Transform _preferredImpactPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	


	}

    public bool IsAvailableForConsumption { get { return _meleeAttack.IsAvailableForConsumption; } }
    public void Consumed()
    {
        _meleeAttack.Consumed();
    }

    public float Damage { get { return _meleeAttack.Damage; } }
    public bool HasPreferredImpactPoint { get { return true; }}
    public Vector3 PreferredImpactPoint { get { return _preferredImpactPoint.position; }}
    public bool AddImpactForce { get { return _meleeAttack.AddImpactForce; } }
    public Vector2 ImpactForce { get { return _meleeAttack.ImpactForce; } }
}
