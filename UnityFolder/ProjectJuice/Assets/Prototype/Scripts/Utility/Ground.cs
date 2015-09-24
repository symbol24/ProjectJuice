using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {
	[SerializeField] private bool m_IsPassThrough = false;
    public bool IsPassThrough { get { return m_IsPassThrough; } }
    private bool m_SetToPassing = false;
    private Collider2D[] m_LastCollider;
    private Collider2D[] m_MyCollider;
    public Collider2D[] Colliders { get { return m_MyCollider; } }
    [SerializeField] private float m_timer = 10f;
    private float m_CurrentDelay = 0;

    void Start()
    {
        m_MyCollider = transform.parent.GetComponentsInChildren<Collider2D>();
    }

    void Update()
    {
        if (m_CurrentDelay > 0) m_CurrentDelay -= Time.deltaTime;
        else m_CurrentDelay = 0f;
        
        if (m_CurrentDelay == 0 && m_SetToPassing)
            TimerCheck();
            
    }

    public void SetPassing(Collider2D[] collider)
    {
        if (!m_SetToPassing)
        {
            m_SetToPassing = true;
            m_LastCollider = collider;
            m_CurrentDelay = m_timer;
        }
    }

    private void TimerCheck()
    {
        foreach (Collider2D myc2d in m_MyCollider)
        {
            foreach (Collider2D c2d in m_LastCollider)
            {
                if (Physics2D.GetIgnoreCollision(myc2d, c2d))
                    Physics2D.IgnoreCollision(myc2d, c2d, false);
            }
        }
        m_SetToPassing = false;
    }
}
