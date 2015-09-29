﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour, IDamaging {
    [SerializeField] private float m_baseSpeed;
    [SerializeField] private float m_Damage;
    private Rigidbody2D m_RigidBody;
    public float Damage
    {
        get
        {
            return Database.instance.BulletBaseDamage;
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

    public ImpactForceSettings ImpactForceSettings
    {
        get { return _impactForceSettings; }
        private set { _impactForceSettings = value; }
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
    [SerializeField] private bool _addImpactForce = false;
    [SerializeField] private bool _hasPreferredImpactPoint = false;
    [SerializeField] private Vector3 _preferredImpactPoint;
    [SerializeField] private ImpactForceSettings _impactForceSettings;
    

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
    }

	// Update is called once per frame
	void Update () {
        //transform.Translate(Vector2.up * m_baseSpeed * Time.deltaTime);
        //m_RigidBody.MovePosition(transform.position.ToVector2() + (Vector2.right * m_baseSpeed * Time.deltaTime));
        
	}

    public void Consumed()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
            Ground ground = collision.gameObject.GetComponent<Ground>();
            if (ground != null && !ground.IsPassThrough)
                Consumed();
    }

    public void AddImmuneTarget(HPScript hpScript)
    {
        _immuneTargets.Push(hpScript);
    }

    public void RemoveImmuneTarget()
    {
        _immuneTargets.Pop();
    }

    public void ShootBullet()
    {
        m_RigidBody.AddForce(transform.up * m_baseSpeed);
    }

    public void ShootBullet(float speed)
    {
        if (speed < 1)
            speed = m_baseSpeed * Random.Range(speed,1);
        m_RigidBody.AddForce(transform.up * speed);
    }

}
