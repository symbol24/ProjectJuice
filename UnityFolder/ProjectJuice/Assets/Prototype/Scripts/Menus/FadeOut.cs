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
    [SerializeField] int m_LevelToLoad;
    [SerializeField] bool TestLoading = false;
    private PlayerSpawner m_spawner;
    // Use this for initialization
    void Start () {
        m_Sprite = GetComponent<SpriteRenderer>();
        m_SpriteColor = m_Sprite.color;
        if (TestLoading) OnLevelWasLoaded(Application.loadedLevel);
        m_spawner = FindObjectOfType<PlayerSpawner>();
        if(m_spawner != null) m_spawner.SpawnDone += M_spawner_SpawnDone;
	}

    void OnLevelWasLoaded(int level)
    {
        if (level == m_LevelToLoad)
        {
            OnLoadDone();
            if(m_spawner == null) //this means theres no spawner object so we can bypass the wait for spawning.
                StartCoroutine(FadeObject(false));
        }

    }

    public event EventHandler LoadDone;

    protected virtual void OnLoadDone()
    {
        EventHandler handler = LoadDone;
        if (handler != null) handler(this, EventArgs.Empty);
        print("OnLoadDone");
    }

    public event EventHandler FadeDone;

    protected virtual void OnFadeDone()
    {
        EventHandler handler = FadeDone;
        if (handler != null) handler(this, EventArgs.Empty);
        print("OnFadeDone");
    }

    private void M_spawner_SpawnDone(object sender, EventArgs e)
    {
        StartCoroutine(FadeObject(false));
    }

    private IEnumerator FadeObject(bool isFadeIn)
    {
        float target = 0f;
        if (isFadeIn) target = 1f;
        float current = 1f;
        while ((!isFadeIn && current > target) || (isFadeIn && current < target))
        {
            yield return new WaitForEndOfFrame();
            current = CalcFade(current, isFadeIn);
            m_SpriteColor.a = current;
            m_Sprite.color = m_SpriteColor;
        }
        OnFadeDone();
    }

    private float CalcFade(float current, bool isIn)
    {
        if(isIn)
            current = Mathf.Clamp01(current + Time.deltaTime / m_FadeInTime);
        else
            current = Mathf.Clamp01(current - Time.deltaTime / m_FadeInTime);

        return current;
    }
}
