using UnityEngine;
using System.Collections;

public class DelayManager : MonoBehaviour {

    private float m_CurrentDelay;

    public bool m_CanShoot { get { return m_CurrentDelay <= 0f; } }
	
	// Update is called once per frame
	void Update () {
        if (m_CurrentDelay > 0) m_CurrentDelay -= Time.deltaTime;
        else m_CurrentDelay = 0f;
	}

    public void AddDelay(float toAdd)
    {
        m_CurrentDelay += toAdd;
    }
}
