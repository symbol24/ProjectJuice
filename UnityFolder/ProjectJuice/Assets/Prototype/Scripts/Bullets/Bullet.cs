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

    public bool HasPreferredImpactPoint { get { return false; } }
    public Vector3 PreferredImpactPoint { get; private set; }
    public bool AddImpactForce { get { return false; }}
    public Vector2 ImpactForce { get; private set; }

    public bool IsAvailableForConsumption
    {
        get
        {
            return true;
        }
    }

    public LayerMask m_WhatIsGround; // A mask determining what is ground 

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

}
