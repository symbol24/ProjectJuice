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

    [SerializeField] private Animator m_PauseMenuAnimator;
    [SerializeField]
    private float _animSafeTimer = 2f;
    private float _currentanimSafeTimer = 0;

    private MenuAnimState _animState = MenuAnimState.NotUsable;

    private bool BackPressed { get { return m_Controls._Start[m_ControllingPlayer] || (!m_inConfirm && m_Controls.Cancel[m_ControllingPlayer]); } }
    
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
            _currentanimSafeTimer = 0f;
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
        else if (GameManager.instance.IsPaused && _animState == MenuAnimState.Usable)
        {
            if (m_DelayManager.CanShoot && BackPressed)
            {
                PauseResume();
            }

            if (m_DelayManager.CanShield && !m_inConfirm && m_Controls.Y[m_ControllingPlayer] != 0)
            {
                SetNextActive(m_Controls.Y[m_ControllingPlayer]);
            }

            if (m_DelayManager.CanShoot && m_Controls.Confirm[m_ControllingPlayer])
            {
                Activate();
            }

            if (m_DelayManager.CanShoot && m_inConfirm && m_Controls.Cancel[m_ControllingPlayer])
            {
                CancelConfirm();
            }
        }
        else if (GameManager.instance.IsPaused)
        {
            _currentanimSafeTimer += Time.unscaledDeltaTime;
            if (_currentanimSafeTimer > _animSafeTimer)
            {
                ResetAnimatorBool("GoUp", false);
                ChangePanelState();
            }
        }
        else
        {
            _currentanimSafeTimer = 0f;
        }

    }

    public void PauseResume()
    {
        SwitchPauseState();
        m_ControllingPlayer = 0;
        SetAnimState(MenuAnimState.NotUsable);
    }

    public void SwitchPauseState()
    {
        m_isPaused = !m_isPaused;
        if (m_isPaused)
        {
            _animState = MenuAnimState.NotUsable;
            m_PausePanel.SetActive(m_isPaused);
            GameManager.instance.SetPaused(m_isPaused, m_PauseMusicVolume, m_IsFullSoundPause);
            SoundManager.PlaySFX(MenuOpenName);
            m_PauseMenuAnimator.SetBool("GoDown", true);
            m_currentSelection = 0;
            m_ListOfButtones[m_currentSelection].Select();
            m_DelayManager.CoroutineDelay(Database.instance.LongMenuInputDelay, true);
        }
        else
        {
            m_PauseMenuAnimator.SetBool("GoUp", true);
            SoundManager.PlaySFX(MenuCloseName);
        }
    }

    public void ChangePanelState()
    {
        if (m_isPaused)
            m_isPaused = !m_isPaused;

        m_PausePanel.SetActive(m_isPaused);
        GameManager.instance.SetPaused(m_isPaused, 1, m_IsFullSoundPause);

    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(SoundThenReturn());
    }

    private IEnumerator SoundThenReturn()
    {
        SetAnimState(MenuAnimState.NotUsable);
        SoundManager.PlaySFX(ReturnToCS);
        m_PauseMenuAnimator.SetBool("GoUp", true);
        float endTimer = Time.unscaledTime + m_delayToReturn;
        while (endTimer > Time.unscaledTime)
        {
            yield return 0;
        }
        ChangePanelState();
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

    public void ResetAnimatorBool(string boolName, bool boolValue)
    {
        m_PauseMenuAnimator.SetBool(boolName, boolValue);
    }

    public void SetAnimState(MenuAnimState newState)
    {
        _animState = newState;
    }
}
