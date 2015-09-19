using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;
using UnityEngine.UI;
using Utility;

public class PlayerSelect : MonoBehaviour {
    private MenuControls m_Controls;
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    private int m_PlayerCount = 0;
    [SerializeField] private Text[] m_PlayerTexts; 
    [SerializeField] private GameObject[] m_PlayerSelectPanels;
    [SerializeField] private string m_SponsorMessage = "Select your Sponsor!";
    [SerializeField] private string m_AbilityMessage = "Select your Ability!";
    private bool[,] m_SponsorValidate;
    private int m_AmountofColors = 4;

	// Use this for initialization
	void Start () {
        m_ListofPlayers = Utilities.GetAllPlayerData();
        m_Controls = FindObjectOfType<MenuControls>();
        int i = System.Enum.GetValues(typeof(Sponsors)).Length;
        m_SponsorValidate = new bool[i,m_AmountofColors];
    }
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < m_ListofPlayers.Count; i++)
        {
            if (m_PlayerCount < 4 && m_Controls.Confirm[i])
                ActivatePlayer(m_ListofPlayers[i], i);
        }
	}

    void ActivatePlayer(PlayerData player, int controllerID)
    {
        if (!player.isActivated)
        {
            player.isActivated = true;

            switch (m_PlayerCount)
            {
                case 0:
                    player.playerID = PlayerIDs.A;
                    m_PlayerTexts[m_PlayerCount].text = m_SponsorMessage;
                    ActivateScreen(player, 0, controllerID);
                    break;
                case 1:
                    player.playerID = PlayerIDs.B;
                    m_PlayerTexts[m_PlayerCount].text = m_SponsorMessage;
                    ActivateScreen(player, 1, controllerID);
                    break;
                case 2:
                    player.playerID = PlayerIDs.C;
                    m_PlayerTexts[m_PlayerCount].text = m_SponsorMessage;
                    ActivateScreen(player, 2, controllerID);
                    break;
                case 3:
                    player.playerID = PlayerIDs.D;
                    m_PlayerTexts[m_PlayerCount].text = m_SponsorMessage;
                    ActivateScreen(player, 3, controllerID);
                    break;
            }

            m_PlayerCount++;
        }
    }

    void NextScene()
    {
        foreach(PlayerData pd in m_ListofPlayers)
        {
            pd.CheckActivated();
        }

        Application.LoadLevel("gameplayTest");
    }

    private void ActivateScreen(PlayerData player, int panel, int ID)
    {
        m_PlayerSelectPanels[panel].SetActive(true);
        SelectorMenu menu = m_PlayerSelectPanels[panel].GetComponent<SelectorMenu>();
        menu.SetPlayer(player, this, ID);
    }
}
