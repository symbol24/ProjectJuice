using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;
using UnityEngine.UI;

public class PauseMenu : Menu {
    [SerializeField] private GameObject m_PausePanel;
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    private int m_ControllingPlayer = -1;

    [SerializeField] private Button[] m_ListOfButtones;
    private int m_currentSelection = 0;
    private int m_maxSelection = 0;

    private DelayManager m_DelayManager;
    [SerializeField] float m_DelayForInput = 0.3f;

    protected static bool m_isPaused = false;

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
        if (GameManager.instance.CheckIfPlaying())
        {
            for (int i = 0; i < m_ListofPlayers.Count; i++)
            {
                if (m_Controls._Start[i])
                {
                    SwitchPauseState();
                    m_ControllingPlayer = i;
                }
            }
        }
        else if(GameManager.instance.CheckIfPlaying())
        {
            if (m_Controls._Start[m_ControllingPlayer] && m_DelayManager.m_CanShoot)
            {
                SwitchPauseState();
                m_ControllingPlayer = -1;
            }

            if(m_Controls.Y[m_ControllingPlayer] != 0 && m_DelayManager.m_CanShoot)
            {
                SetNextActive(m_Controls.Y[m_ControllingPlayer]);
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
            m_ListOfButtones[m_currentSelection].Select();
            Time.timeScale = 0f;
        }
        else
        {
            m_currentSelection = 0;
            Time.timeScale = 1f;
        }
    }

    private void SetNextActive(float y)
    {
        print("in setnextactive;");
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

        m_ListOfButtones[m_currentSelection].Select();
        m_DelayManager.CoroutineDelay(m_DelayForInput);
    }
}
