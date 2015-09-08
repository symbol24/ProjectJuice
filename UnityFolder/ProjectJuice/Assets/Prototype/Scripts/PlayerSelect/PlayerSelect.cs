using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;
using UnityEngine.UI;
using Utility;

public class PlayerSelect : MonoBehaviour {

    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    private int m_PlayerCount = 0;
    [SerializeField] private Text[] m_PlayerTexts; 

	// Use this for initialization
	void Start () {
        m_ListofPlayers = Utilities.GetAllPlayerData();
    }
	
	// Update is called once per frame
	void Update () {
        foreach (PlayerData pd in m_ListofPlayers)
        {
            if (m_PlayerCount < 4 && GamePad.GetButtonDown(GamePad.Button.A, pd.GamepadIndex))
                ActivatePlayer(pd);

            if (m_PlayerCount > 0 && GamePad.GetButtonDown(GamePad.Button.Start, pd.GamepadIndex))
                NextScene();
        }
	}

    void ActivatePlayer(PlayerData player)
    {
        if (!player.isActivated)
        {
            player.isActivated = true;

            switch (m_PlayerCount)
            {
                case 0:
                    player.playerID = PlayerIDs.A;
                    m_PlayerTexts[m_PlayerCount].text = "Press Start to continue";
                    break;
                case 1:
                    player.playerID = PlayerIDs.B;
                    m_PlayerTexts[m_PlayerCount].text = "Press Start to continue";
                    break;
                case 2:
                    player.playerID = PlayerIDs.C;
                    m_PlayerTexts[m_PlayerCount].text = "Press Start to continue";
                    break;
                case 3:
                    player.playerID = PlayerIDs.D;
                    m_PlayerTexts[m_PlayerCount].text = "Press Start to continue";
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

}
