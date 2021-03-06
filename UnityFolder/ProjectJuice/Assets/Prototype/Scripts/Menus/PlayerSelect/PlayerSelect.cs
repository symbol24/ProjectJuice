﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;
using UnityEngine.UI;
using System;

public class PlayerSelect : Menu {
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    private int m_PlayerCount = 0;
    public int PlayerCount { get { return m_PlayerCount; } }
    private int m_ReadyCount = 0;
    public int ReadyCount { get { return m_ReadyCount; } }
    [SerializeField] private Text[] m_PlayerTexts;
    public Text[] HeaderText { get { return m_PlayerTexts; } set { m_PlayerTexts = value; } }
    [SerializeField] private GameObject[] m_PlayerSelectPanels;
    LoadingScreen m_Fader;
    [Range(0,1)][SerializeField] private float m_delayTransition = 0.5f;
    [HideInInspector] public int NextScene;
    [HideInInspector] public string StartSound;
    [HideInInspector] public string AllReadySound;

    void Awake()
    {
    }

    private void M_Fader_LoadDone(object sender, System.EventArgs e)
    {
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_Fader = FindObjectOfType<LoadingScreen>();
        m_Fader.LoadingAnimDone += M_Fader_FadeDone;
        m_Fader.LoadDone += M_Fader_LoadDone;
        GameManager.instance.SetGameState(GameState.Loading);
        ResetSponsors();
    }

    private void M_Fader_FadeDone(object sender, System.EventArgs e)
    {
        m_ListofPlayers = Utilities.GetAllPlayerData();
        foreach (PlayerData pd in m_ListofPlayers)
        {
            if (pd.isActivated)
                pd.isActivated = false;
        }
    }

    // Update is called once per frame
    protected override void Update () {
        for(int i = 0; i < m_ListofPlayers.Count; i++)
        {
            if (GameManager.instance.IsCharacterSelect && m_PlayerCount < 4 && m_Controls.Confirm[i])
                ActivatePlayer(m_ListofPlayers[i], i);

            if (GameManager.instance.IsCharacterSelect &&  m_PlayerCount > 1 && m_PlayerCount == m_ReadyCount && m_Controls._Start[i])
                StartCoroutine(GoToNextScene());
        }
	}

    void ActivatePlayer(PlayerData player, int controllerID)
    {
        if (!player.isActivated)
        {
            player.isActivated = true;

            GameObject toActivate = null;
            foreach(GameObject g in m_PlayerSelectPanels)
            {
                if (!g.activeInHierarchy)
                {
                    toActivate = g;
                    break;
                }
            }

            if (toActivate == null) print("No available player slots left");
            else
            {
                m_PlayerTexts[m_PlayerCount].text = Database.instance.GameTexts[1];
                int panelId = toActivate.GetComponent<SelectorMenu>().PanelId;

                switch (m_PlayerCount)
                {
                    case 0:
                        player.playerID = PlayerIDs.A;
                        break;
                    case 1:
                        player.playerID = PlayerIDs.B;
                        break;
                    case 2:
                        player.playerID = PlayerIDs.C;
                        break;
                    case 3:
                        player.playerID = PlayerIDs.D;
                        break;
                }


                ActivateScreen(player, panelId, controllerID);

                m_PlayerCount++;
            }
            SoundManager.PlaySFX(Database.instance.MenuClickName);
        }
    }

    IEnumerator GoToNextScene()
    {
        SoundManager.PlaySFX(StartSound);
        foreach(PlayerData pd in m_ListofPlayers)
        {
            pd.CheckActivated();
        }
        GameManager.instance.SetGameState(GameState.Loading);
        yield return new WaitForSeconds(m_delayTransition);
        Application.LoadLevel(NextScene);
    }

    private void ActivateScreen(PlayerData player, int panel, int ID)
    {
        m_PlayerSelectPanels[panel].SetActive(true);
        SelectorMenu menu = m_PlayerSelectPanels[panel].GetComponent<SelectorMenu>();
        menu.SetPlayer(player, this, ID);
        menu.isActive = true;
    }

    public void ReadyUp(bool isUp)
    {
        if (isUp) {
            m_ReadyCount++;
            if (m_ReadyCount == m_PlayerCount) SoundManager.PlaySFX(AllReadySound);
        }
        else m_ReadyCount--;
    }

    public void DeactivateScreen(int panel, PlayerData player)
    {
        player.isActivated = false;
        m_PlayerSelectPanels[panel].SetActive(false);
        m_PlayerCount--;
    }

    private void ResetSponsors()
    {
        foreach (Sponsor sp in Database.instance.ListofSponsors)
            sp.ReleaseSponsor();
    }
}
