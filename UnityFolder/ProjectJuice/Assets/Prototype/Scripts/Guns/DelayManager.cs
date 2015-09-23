using UnityEngine;
using System.Collections;

public class DelayManager : MonoBehaviour {

    private float m_CurrentDelay;
    private float m_ShieldDelay;

    public bool m_CanShoot { get { return m_CurrentDelay <= 0f; } }
    public bool m_CanShield { get { return m_ShieldDelay <= 0f; } }

    // Update is called once per frame
    void Update () {
        if (m_CurrentDelay > 0) m_CurrentDelay -= Time.deltaTime;
        else m_CurrentDelay = 0f;

        if (m_ShieldDelay > 0) m_ShieldDelay -= Time.deltaTime;
        else m_ShieldDelay = 0f;
    }

    public void AddDelay(float toAdd)
    {
        m_CurrentDelay += toAdd;
    }

    public void AddShieldDelay(float toAdd)
    {
        m_ShieldDelay += toAdd;
    }

    public void Reset()
    {
        m_CurrentDelay = 0f;
        m_ShieldDelay = 0f;
    }

    public void CoroutineDelay(float delay)
    {
        m_CurrentDelay = delay;
        StartCoroutine(DelayTime(delay));
    }

    IEnumerator DelayTime(float timer)
    {
        yield return new WaitForSeconds(timer);
        m_CurrentDelay = 0f;
    }
}
