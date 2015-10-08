using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;
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
    
    [HideInInspector] public string MenuOpenName;
    [HideInInspector] public string MenuCloseName;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        m_ListofPlayers = Utilities.GetAllPlayerData();
        m_maxSelection = m_ListOfButtones.Length;
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
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
                    m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
                }
            }
        }
        else if(GameManager.instance.IsPaused)
        {
            if (m_DelayManager.m_CanShoot && m_Controls._Start[m_ControllingPlayer])
            {
                SwitchPauseState();
                m_ControllingPlayer = 0;
            }

            if(m_DelayManager.m_CanShield && !m_inConfirm && m_Controls.Y[m_ControllingPlayer] != 0)
            {
                SetNextActive(m_Controls.Y[m_ControllingPlayer]);
            }

            if (m_DelayManager.m_CanShoot && m_Controls.Confirm[m_ControllingPlayer])
            {
                Activate();
            }

            if(m_DelayManager.m_CanShoot && m_inConfirm && m_Controls.Cancel[m_ControllingPlayer])
            {
                CancelConfirm();
            }
        }
	}

    public void SwitchPauseState()
    {
        m_isPaused = !m_isPaused;
        GameManager.instance.SetPaused(m_isPaused);
        m_PausePanel.SetActive(m_isPaused);
        if (m_isPaused)
        {
            SoundManager.PlaySFX(MenuOpenName);
            m_currentSelection = 0;
            m_ListOfButtones[m_currentSelection].Select();
            m_DelayManager.CoroutineDelay(Database.instance.LongMenuInputDelay, true);
        }
        else
            SoundManager.PlaySFX(MenuCloseName);
    }

    public void ReturnToMainMenu()
    {
        if(m_isPaused)
            SwitchPauseState();
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
        if(y > 0)
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
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void Activate()
    {
        SoundManager.PlaySFX(Database.instance.MenuClickName);
        m_ListOfButtones[m_currentSelection].onClick.Invoke();
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }
}
