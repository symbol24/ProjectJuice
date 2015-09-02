using UnityEngine;
using System.Collections;

public class SingleParticleGrounder : MonoBehaviour {

    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = 0.01f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded = false;            // Whether or not the player is grounded.
    private bool m_Immobile = false;
    private Rigidbody2D m_RigidBody;

    // Use this for initialization
    void Start () {
        m_GroundCheck = transform.FindChild("ParticleGroundCheck");
        m_RigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_Grounded) m_Grounded = CheckGrounded();
        if (m_Grounded && !m_Immobile) m_Immobile = SetImmobile();
	}

    private bool CheckGrounded()
    {
        bool isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                isGrounded = true;
        }

        return isGrounded;
    }

    private bool SetImmobile()
    {
        bool isImmobile = false;

        m_RigidBody.isKinematic = true;

        isImmobile = true;

        return isImmobile;
    }
}
