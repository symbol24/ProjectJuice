using UnityEngine;
using System.Collections;

public class LogoIntro : MonoBehaviour {
    [Range(0,5)][SerializeField] float m_FadeInTime = 2f;
    [Range(0,5)][SerializeField] float m_WaitForNextScene = 2f;
    [SerializeField] string m_NextScene = "multipadTest";
    Color m_SpriteColor;
    SpriteRenderer m_Sprite;
    // Use this for initialization
    void Start () {
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
        }
        StartCoroutine(GoToNextScene());
    }

    private IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(m_WaitForNextScene);
        Application.LoadLevel(m_NextScene);
    }
}
