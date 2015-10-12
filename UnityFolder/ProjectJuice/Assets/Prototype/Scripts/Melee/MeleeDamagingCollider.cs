using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class MeleeDamagingCollider : MonoBehaviour, IDamaging
{
    public Vector2 _directionOfClashing = new Vector2(1, 0);
    public float _magnitudeOfClashing = 1f;
    [Range(0, 1)] public float _deacceleration = 0.9f;
    [Range(0,1)] public float _timeOfClashing;
    public MeleeAttack _meleeAttack;
    public Transform _preferredImpactPoint;

    [Range(0,10)][SerializeField] private int _bulletsToGive = 3;
    public int BulletsToGiveShield
    {
        get { return _bulletsToGive; }
        private set { _bulletsToGive = value; }
    }

	// Use this for initialization
	void Start ()
	{
	    if (!_meleeAttack.enabled)
	    {
	        _meleeAttack = _meleeAttack.gameObject.GetComponents<MeleeAttack>().First(c => c.enabled);
	    }
	}
	
	// Update is called once per frame
	void Update () {
	


	}

    public bool IsAvailableForConsumption { get { return _meleeAttack.IsAvailableForConsumption; } }
    bool IConsumable.IsAvailableForConsumption(object sender)
    {
        return IsAvailableForConsumption;
    }
    public void Consumed()
    {
        _meleeAttack.Consumed();
    }

    public float Damage { get { return _meleeAttack.Damage; } }
    public bool HasPreferredImpactPoint { get { return true; }}
    public Vector3 PreferredImpactPoint { get { return _preferredImpactPoint.position; }}
    public bool AddImpactForce { get { return _meleeAttack.AddImpactForce; } }
    public ImpactForceSettings ImpactForceSettings { get { return _meleeAttack.ImpactForceSettings; }}
    public IEnumerable<HPScript> ImmuneTargets { get { return _meleeAttack.ImmuneTargets; } }
    public bool HasImmuneTargets { get { return _meleeAttack.HasImmuneTargets; } }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var checkForOtherMelee = collider.gameObject.GetComponent<MeleeDamagingCollider>();
        if (checkForOtherMelee != null)
        {
            _meleeAttack.ClashedWithOtherMelee(checkForOtherMelee);
        }
    }

    public void UpdateImpactForceSetting(ImpactForceSettings toUpdate)
    {
    }
}
