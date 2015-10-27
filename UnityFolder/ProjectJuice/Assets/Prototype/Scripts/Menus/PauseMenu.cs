using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenu : Menu {
    [SerializeField] private GameObject m_PausePanel;
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    private int m_ControllingPlayer = 0;

    [SerializeField] private Button[] m_ListOfButtones;
    private int m_currentSelection = 0;
    private int m_maxSelection = 0;

    private DelayManager m_DelayManager;

    protected static bool m_isPaused = false;
    private bool m_inConfirm = false;

    [Range(0,1)][SerializeField] private float m_PauseMusicVolume = 0.5f;
    [SerializeField] private bool m_IsFullSoundPause = false;
    [Range(0,10)][SerializeField] private float m_delayToReturn = 2f;

    [SerializeField] private float m_UpperYPosition;
    [SerializeField] private float m_CenterYPosition;
    [Range(0,3)][SerializeField] private float m_scrollInTime = 0.5f;
    [SerializeField] private RectTransform m_PausePanelToScroll;
    [SerializeField] private Animator m_PauseMenuAnimator;
    
    [HideInInspector] public string MenuOpenName;
    [HideInInspector] public string MenuCloseName;
    [HideInInspector] public string ReturnToCS;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        m_ListofPlayers = Utilities.GetAllPlayerData();
        m_maxSelection = GetListOfActiveButtons();
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();

        Vector3 temp = new Vector3(m_PausePanelToScroll.localPosition.x, m_PausePanelToScroll.localPosition.y, m_PausePanelToScroll.localPosition.z);
        temp.y = m_UpperYPosition;
        //m_PausePanelToScroll.localPosition = temp;

        m_PausePanel.SetActive(false);
    }

    private int GetListOfActiveButtons()
    {
        int ret = 0;

        foreach (Button b in m_ListOfButtones) if (b.IsActive()) ret++;

        return ret;
    }

    // Update is called once per frame
    protected override void Update () {
        if (GameManager.instance.IsPlaying)
        {
            for (int i = 0; i < m_ListofPlayers.Count; i++)
            {
                if (m_Controls._Start[i])
                {
                    SwitchPauseState();
                    m_ControllingPlayer = i;
                    m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, true);
                }
            }
        }
        else if(GameManager.instance.IsPaused)
        {
            if (m_DelayManager.CanShoot && m_Controls._Start[m_ControllingPlayer])
            {
                SwitchPauseState();
                m_ControllingPlayer = 0;
            }

            if(m_DelayManager.CanShield && !m_inConfirm && m_Controls.Y[m_ControllingPlayer] != 0)
            {
                SetNextActive(m_Controls.Y[m_ControllingPlayer]);
            }

            if (m_DelayManager.CanShoot && m_Controls.Confirm[m_ControllingPlayer])
            {
                Activate();
            }

            if(m_DelayManager.CanShoot && m_inConfirm && m_Controls.Cancel[m_ControllingPlayer])
            {
                CancelConfirm();
            }
        }
	}

    public void SwitchPauseState()
    {
        m_isPaused = !m_isPaused;
        m_PausePanel.SetActive(m_isPaused);
        if (m_isPaused)
        {
            GameManager.instance.SetPaused(m_isPaused, m_PauseMusicVolume, m_IsFullSoundPause);
            SoundManager.PlaySFX(MenuOpenName);
            m_currentSelection = 0;
            m_ListOfButtones[m_currentSelection].Select();
            m_DelayManager.CoroutineDelay(Database.instance.LongMenuInputDelay, true);
            //StartScrollCoroutine(false, m_scrollInTime);
        }
        else
        {
            SoundManager.PlaySFX(MenuCloseName);
            GameManager.instance.SetPaused(m_isPaused, 1, m_IsFullSoundPause);
        }
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(SoundThenReturn());
    }

    private IEnumerator SoundThenReturn()
    {
        SoundManager.PlaySFX(ReturnToCS);
        float endTimer = Time.unscaledTime + m_delayToReturn;
        while (endTimer > Time.unscaledTime)
        {
            yield return 0;
        }
        Application.LoadLevel(Database.instance.MainMenuID);
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void Confirm(bool closeApp)
    {
        m_inConfirm = true;
        m_ListOfButtones[m_currentSelection].onClick.RemoveAllListeners();
        Text buttonText = m_ListOfButtones[m_currentSelection].GetComponentInChildren<Text>();
        buttonText.text = Database.instance.GameTexts[8];
        if (closeApp)
        {
            m_ListOfButtones[m_currentSelection].onClick.AddListener(() => CloseApp());
        }
        else
        {
            m_ListOfButtones[m_currentSelection].onClick.AddListener(() => ReturnToMainMenu());
        }
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void CancelConfirm()
    {
        int textID = 11;
        bool isCloseApp = false;
        if (m_currentSelection == 2) {
            textID = 12;
            isCloseApp = true;
        }

        Text buttonText = m_ListOfButtones[m_currentSelection].GetComponentInChildren<Text>();
        buttonText.text = Database.instance.GameTexts[textID];
        m_ListOfButtones[m_currentSelection].onClick.RemoveAllListeners();
        m_ListOfButtones[m_currentSelection].onClick.AddListener(() => Confirm(isCloseApp));
        m_inConfirm = false;

        SoundManager.PlaySFX(Database.instance.MenuCancelName);
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void SetNextActive(float y)
    {
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
        if (y > 0)
        {
            m_currentSelection--;
            if (m_currentSelection < 0) m_currentSelection = m_maxSelection - 1;
        }
        else if(y < 0)
        {
            m_currentSelection++;
            if (m_currentSelection == m_maxSelection) m_currentSelection = 0;
        }
        SoundManager.PlaySFX(Database.instance.MenuSlideName);
        m_ListOfButtones[m_currentSelection].Select();
    }

    private void Activate()
    {
         SoundManager.PlaySFX(Database.instance.MenuClickName);
        m_ListOfButtones[m_currentSelection].onClick.Invoke();
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void StartScrollCoroutine(bool isUp = false, float timeToScroll = 0.5f)
    {
        StartCoroutine(ScrollInOut(isUp, timeToScroll));
    }

    private IEnumerator ScrollInOut(bool isUp, float timeToScroll)
    {
        print("in ScrollInOut");
        float target = m_CenterYPosition;
        if (isUp) target = m_UpperYPosition;
        float current = m_PausePanelToScroll.localPosition.y;
        Vector3 temp = new Vector3(m_PausePanelToScroll.localPosition.x, m_PausePanelToScroll.localPosition.y, m_PausePanelToScroll.localPosition.z);
        while ((!isUp && target > m_CenterYPosition) || (isUp && target < m_UpperYPosition))
        {
            yield return new WaitForEndOfFrame();
            current = CalcCurve(current, isUp, timeToScroll);
            temp.y = current;
            m_PausePanelToScroll.localPosition = temp;
            print(current);
        }
    }

    private float CalcCurve(float current, bool isUP, float timer = 0.5f)
    {
        if (isUP)
            current = Mathf.Clamp01(current + Time.unscaledDeltaTime / timer);
        else
            current = Mathf.Clamp01(current - Time.unscaledDeltaTime / timer);

        return current;
    }
}
