using UnityEngine;
using System.Collections;

public class DelayManager : MonoBehaviour {

    private float m_CurrentDelay;
    private float m_ShieldDelay;
    private float m_ShieldOffDelay;
    private float m_OtherDelay;
    private float m_SoundDelay;

    public bool CanShoot { get { return m_CurrentDelay <= 0f; } }
    public bool CanShield { get { return m_ShieldDelay <= 0f; } }
    public bool CanTurnOffShield { get { return m_ShieldOffDelay <= 0f; } }
    public bool OtherReady { get { return m_OtherDelay <= 0f; } }
    public bool SoundReady { get { return m_SoundDelay <= 0f; } }

    // Update is called once per frame
    void Update () {
        if (m_CurrentDelay > 0) m_CurrentDelay -= Time.deltaTime;
        else m_CurrentDelay = 0f;

        if (m_ShieldDelay > 0) m_ShieldDelay -= Time.deltaTime;
        else m_ShieldDelay = 0f;

        if (m_OtherDelay > 0) m_OtherDelay -= Time.deltaTime;
        else m_OtherDelay = 0;

        if (m_ShieldOffDelay > 0) m_ShieldOffDelay -= Time.deltaTime;
        else m_ShieldOffDelay = 0;

        if (m_SoundDelay > 0) m_SoundDelay -= Time.deltaTime;
        else m_SoundDelay = 0;
    }

    public void AddDelay(float toAdd)
    {
        m_CurrentDelay += toAdd;
    }

    public void SetDelay(float toSet)
    {
        m_CurrentDelay = toSet;
    }

    public void AddShieldDelay(float toAdd)
    {
        m_ShieldDelay += toAdd;
    }

    public void SetShieldDelay(float toSet)
    {
        m_ShieldDelay = toSet;
    }

    public void AddShieldOffDelay(float toAdd)
    {
        m_ShieldOffDelay += toAdd;
    }

    public void SetShieldOffDelay(float toSet)
    {
        m_ShieldOffDelay = toSet;
    }

    public void AddOtherDelay(float toAdd)
    {
        m_OtherDelay += toAdd;
    }

    public void SetOtherDelay(float toSet)
    {
        m_OtherDelay = toSet;
    }

    public void AddSoundDelay(float toAdd)
    {
        m_SoundDelay += toAdd;
    }

    public void SetSoundDelay(float toSet)
    {
        m_SoundDelay = toSet;
    }

    public void Reset() //use wisely as this resets ALL TIMERS!!!
    {
        m_CurrentDelay = 0f;
        m_ShieldDelay = 0f;
        m_ShieldOffDelay = 0f;
        m_OtherDelay = 0f;
        m_SoundDelay = 0f;
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
        float endTimer = Time.unscaledTime + timer;
        while(endTimer > Time.unscaledTime)
        {
            yield return 0;
        }
        if (isShoot)
            m_CurrentDelay = 0f;
        else
            m_ShieldDelay = 0;
    }
}
