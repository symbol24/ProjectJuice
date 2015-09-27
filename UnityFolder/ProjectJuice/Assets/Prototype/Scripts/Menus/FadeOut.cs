using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class FadeOut : MonoBehaviour {
    [Range(0,5)][SerializeField] float m_FadeInTime = 2f;
    Color m_SpriteColor;
    SpriteRenderer m_Sprite;
    private bool m_IsDone = false;
    public bool IsDone { get { return m_IsDone; } }
    // Use this for initialization
    void Start () {
        m_Sprite = GetComponent<SpriteRenderer>();
        m_SpriteColor = m_Sprite.color;
        StartCoroutine(Fade());
	}

    public event EventHandler FadeDone;

    protected virtual void OnFadeDone()
    {
        EventHandler handler = FadeDone;
        if (handler != null) handler(this, EventArgs.Empty);
    }

    private IEnumerator Fade()
    {
        float t = 1f;
        while (t > 0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t - Time.deltaTime / m_FadeInTime);
            m_SpriteColor.a = t;
            m_Sprite.color = m_SpriteColor;
        }
        OnFadeDone();
    }
}
