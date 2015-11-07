using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundStartTimer : MonoBehaviour {
    [SerializeField] Text m_TimerText;
    [SerializeField] PlayerSpawner m_spawner;
    [Range(0, 3)][SerializeField] float m_delayBetweenNumbers = 1f;
    [Range(0, 3)][SerializeField] float m_delayAfterGo = 0.5f;
    [Range(0, 10)][SerializeField] int m_amountOfSeconds = 5;
    public int AmountOfSeconds { get { return m_amountOfSeconds; } }
    private LoadingScreen m_fader;
    [SerializeField] private Animator m_PurpleAnimator;
    [SerializeField] private Animator m_panelAnimator;
    [HideInInspector] public string NumberClipName;
    [HideInInspector] public string GoClipName;
    [HideInInspector] public string GetReadyClipName;

    // Use this for initialization
    void Start () {
        if (m_fader == null) m_fader = FindObjectOfType<LoadingScreen>();
        if(m_fader != null) m_fader.LoadingAnimDone += EnterTimer;
        m_TimerText.text = Database.instance.GameTexts[16];
        GameManager.instance.InjectRoundStartTimer(this);
    }

    private void EnterTimer(object sender, System.EventArgs e)
    {
        m_panelAnimator.SetBool("Enter", true);
    }

    public void StartTimer()
    {
        StartCoroutine(DisplayTime());
    }

    public void SetBool(Animator anim, string with, bool what)
    {
        anim.SetBool(with, what);
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
        SetBool(m_PurpleAnimator, "Play", true);
        while (!displayed)
        {
            SetBool(m_PurpleAnimator, "Play", true);
            m_TimerText.text = time.ToString();
            time--;
            if (time == 0) displayed = true;
            yield return new WaitForSeconds(m_delayBetweenNumbers);
        }
        SetBool(m_PurpleAnimator, "Play", true);
        m_TimerText.text = Database.instance.GameTexts[17];
        yield return new WaitForSeconds(m_delayAfterGo/2);
        SetBool(m_panelAnimator, "Exit", true);
        yield return new WaitForSeconds(m_delayAfterGo/2);
        GameManager.instance.SetGameState(GameState.Playing);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
