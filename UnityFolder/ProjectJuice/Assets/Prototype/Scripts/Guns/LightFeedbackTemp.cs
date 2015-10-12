using UnityEngine;
using System.Collections;
using System;

public class LightFeedbackTemp : ExtendedMonobehaviour {
    
    [SerializeField] private Light m_GunLight;
    [Range(0,3)][SerializeField] float m_LightOn = 0.1f;
    [Range(0,3)][SerializeField] float m_LightOff = 0.1f;
    [Range(0, 10)][SerializeField] private int m_AmountOfFlashes = 3;
    protected bool m_HasDisplayed = true;

    public void StartLightFeedback(float delay = 1f)
    {
        StartCoroutine(DisplayGunLight(delay));
    }

    private IEnumerator DisplayGunLight(float delay = 1f)
    {
        yield return new WaitForSeconds(delay);
        m_HasDisplayed = true;
        for (int i = 0; i < m_AmountOfFlashes; i++)
        {
            m_GunLight.enabled = true;
            yield return new WaitForSeconds(m_LightOn);
            m_GunLight.enabled = false;
            yield return new WaitForSeconds(m_LightOff);
        }
        OnLightDone();
    }

    public event EventHandler LightDone;

    protected virtual void OnLightDone()
    {
        EventHandler handler = LightDone;
        if (handler != null) handler(this, EventArgs.Empty);
    }
}
