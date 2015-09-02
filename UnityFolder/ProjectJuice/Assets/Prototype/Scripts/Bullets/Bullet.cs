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

    public bool IsAvailableForConsumption
    {
        get
        {
            return true;
        }
    }

    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector2.up * m_baseSpeed * Time.deltaTime);
	}

    public void Consumed()
    {
        Destroy(gameObject);
    }
}
