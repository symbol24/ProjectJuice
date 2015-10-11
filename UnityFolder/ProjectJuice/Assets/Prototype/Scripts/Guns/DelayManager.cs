using UnityEngine;
using System.Collections;

public class DelayManager : MonoBehaviour {

    private float m_CurrentDelay;
    private float m_ShieldDelay;
    private float m_OtherDelay;

    public bool m_CanShoot { get { return m_CurrentDelay <= 0f; } }
    public bool m_CanShield { get { return m_ShieldDelay <= 0f; } }
    public bool m_OtherReady { get { return m_OtherDelay <= 0f; } }

    // Update is called once per frame
    void Update () {
        if (m_CurrentDelay > 0) m_CurrentDelay -= Time.deltaTime;
        else m_CurrentDelay = 0f;

        if (m_ShieldDelay > 0) m_ShieldDelay -= Time.deltaTime;
        else m_ShieldDelay = 0f;

        if (m_OtherDelay > 0) m_OtherDelay -= Time.deltaTime;
        else m_OtherDelay = 0;
    }

    public void AddDelay(float toAdd)
    {
        m_CurrentDelay += toAdd;
    }

    public void AddShieldDelay(float toAdd)
    {
        m_ShieldDelay += toAdd;
    }

    public void AddOtherDelay(float toAdd)
    {
        m_OtherDelay += toAdd;
    }

    public void Reset()
    {
        m_CurrentDelay = 0f;
        m_ShieldDelay = 0f;
        m_OtherDelay = 0f;
    }

    public void CoroutineDelay(float delay, bool isShoot)
    {
        if (isShoot)
            m_CurrentDelay = delay;
        else
            m_ShieldDelay = delay;
        StartCoroutine(DelayTime(delay, isShoot));
    }

    IEnumerator DelayTime(float timer, bool isShoot)
    {
        float endTiemr = Time.realtimeSinceStartup + timer;
        while(endTiemr > Time.realtimeSinceStartup)
        {
            yield return 0;
        }
        if (isShoot)
            m_CurrentDelay = 0f;
        else
            m_ShieldDelay = 0;
    }
}
