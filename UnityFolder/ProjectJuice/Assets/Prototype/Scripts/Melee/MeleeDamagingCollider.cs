using System.Collections.Generic;
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
    private Rigidbody2D _rigidbody;
	// Use this for initialization
	void Start ()
	{
	    _rigidbody = GetComponent<Rigidbody2D>();
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
    public float TimeToApplyForce { get { return _meleeAttack.TimeToApplyForce; } }
    public IEnumerable<HPScript> ImmuneTargets { get { return _meleeAttack.ImmuneTargets; } }
    public bool HasImmuneTargets { get { return _meleeAttack.HasImmuneTargets; } }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var checkForOtherMelee = collider.gameObject.GetComponent<MeleeDamagingCollider>();
        if (checkForOtherMelee != null)
        {
            if (!_isClashingOnGoing)
            {
                _clashingCoroutine = ClashingCoroutine();
                StartCoroutine(_clashingCoroutine);
            }
        }
    }

    private bool _isClashingOnGoing = false;
    private IEnumerator _clashingCoroutine;
    IEnumerator ClashingCoroutine()
    {
        _isClashingOnGoing = true;
        float currentTimer = 0f;
        while (currentTimer < _timeOfClashing)
        {
            var direction = _directionOfClashing.normalized*(_meleeAttack.InputManager.m_FacingRight ? 1 : -1);
            var force = direction*_magnitudeOfClashing;
            _rigidbody.velocity = force*_deacceleration;
            currentTimer += Time.deltaTime;
            yield return null;
        }
        _isClashingOnGoing = false;
    }

}
