using UnityEngine;
using System.Collections;

public class SingleParticleGrounder : MonoBehaviour {

    private bool m_Grounded = false;            // Whether or not the player is grounded.
    private bool m_Immobile = false;
    private Rigidbody2D m_RigidBody;

    // Use this for initialization
    void Start () {
        m_RigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Grounded && !m_Immobile) m_Immobile = SetImmobile();
	}

    private bool SetImmobile()
    {
        bool isImmobile = false;

        m_RigidBody.isKinematic = true;

        isImmobile = true;

        return isImmobile;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Ground ground = collider.GetComponent<Ground>();
        if(ground != null)
        {
            m_Grounded = true;
        }
    }
}
