using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System;

public class Bullet : MonoBehaviour, IDamaging {
    [SerializeField] private float m_baseSpeed;
    [SerializeField] private float m_Damage;
    public float Damage
    {
        get
        {
            return m_Damage;
        }
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

    public Vector2 ImpactForce { get; private set; }

    public float TimeToApplyForce
    {
        get { return _timeToApplyForce; }
        private set { _timeToApplyForce = value; }
    }

    private Stack<HPScript> _immuneTargets = new Stack<HPScript>();
    public IEnumerable<HPScript> ImmuneTargets { get { return _immuneTargets; } }
    public bool HasImmuneTargets { get { return _immuneTargets.Any(); } }

    public bool IsAvailableForConsumption
    {
        get
        {
            return true;
        }
    }

    public LayerMask m_WhatIsGround; // A mask determining what is ground 
    [SerializeField]
    private float _timeToApplyForce;
    [SerializeField]
    private bool _addImpactForce = false;
    [SerializeField]
    private bool _hasPreferredImpactPoint = false;
    [SerializeField]
    private Vector3 _preferredImpactPoint;

    void Start()
    {
    }

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector2.up * m_baseSpeed * Time.deltaTime);
	}

    public void Consumed()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayerChecker.IsInLayerMask(collision.gameObject, m_WhatIsGround)) Consumed();
    }

    public void AddImmuneTarget(HPScript hpScript)
    {
        _immuneTargets.Push(hpScript);
    }

}
