using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : MonoBehaviour {
	[SerializeField] private bool m_IsPassThrough = false;
    public bool IsPassThrough { get { return m_IsPassThrough; } }
    private Collider2D[] m_MyCollider;
    public Collider2D[] Colliders { get { return m_MyCollider; } }

    void Start()
    {
        if (transform.parent != null)
        {
            m_MyCollider = transform.parent.GetComponentsInChildren<Collider2D>();
        }
        else
        {
            m_MyCollider = GetComponentsInChildren<Collider2D>();
        }
    }
}


