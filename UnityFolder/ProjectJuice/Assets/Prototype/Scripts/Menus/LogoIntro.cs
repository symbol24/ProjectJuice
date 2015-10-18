using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LogoIntro : ExtendedMonobehaviour {
    [Range(0,5)][SerializeField] float m_FadeInTime = 2f;
    [Range(0,5)][SerializeField] float m_WaitForNextScene = 2f;
    [HideInInspector] public int NextScene;
    [HideInInspector] public string ClipName;
    private bool m_clipPlayaed = false;
    private string m_clipName;

    Color m_SpriteColor;
    SpriteRenderer m_Sprite;
    private AudioClip[] m_logoClips;


    // Use this for initialization
    void Start ()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
        m_SpriteColor = m_Sprite.color;
        StartCoroutine(Fade());
    }


    private IEnumerator Fade()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t + Time.deltaTime / m_FadeInTime);
            m_SpriteColor.a = t;
            m_Sprite.color = m_SpriteColor;
            if (!m_clipPlayaed && t > 0.5f)
            {
                m_clipPlayaed = true;
                SoundManager.PlaySFX(ClipName);
            }
        }
        StartCoroutine(GoToNextScene());
    }

    private IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(m_WaitForNextScene);
        Application.LoadLevel(NextScene);
    }


}
