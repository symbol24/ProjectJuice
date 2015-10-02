using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundStartTimer : MonoBehaviour {
    [SerializeField] Text m_TimerText;
    [SerializeField] PlayerSpawner m_spawner;
    [Range(0, 3)][SerializeField] float m_delayBetweenNumbers = 1f;
    [Range(0, 3)][SerializeField] float m_delayAfterGo = 0.5f;
    [Range(0, 10)][SerializeField] int m_amountOfSeconds = 5;
    private FadeOut m_fader;

	// Use this for initialization
	void Start () {
        if (m_fader == null) m_fader = FindObjectOfType<FadeOut>();
        if(m_fader != null) m_fader.FadeDone += StartCountdown;
        m_TimerText.text = Database.instance.GameTexts[16];
        GameManager.instance.InjectRoundStartTimer(this);
	}

    private void StartCountdown(object sender, System.EventArgs e)
    {
        StartCoroutine(DisplayTime());
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void Reset()
    {
        m_TimerText.text = Database.instance.GameTexts[16];
    }

    IEnumerator DisplayTime()
    {
        bool displayed = false;
        int time = m_amountOfSeconds;
        while (!displayed)
        {
            m_TimerText.text = time.ToString();
            time--;
            if (time == 0) displayed = true;
            yield return new WaitForSeconds(m_delayBetweenNumbers);
        }
        m_TimerText.text = Database.instance.GameTexts[17];
        yield return new WaitForSeconds(m_delayAfterGo);
        GameManager.instance.SetGameState(GameState.Playing);
        gameObject.SetActive(false);
    }
}
