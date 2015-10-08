using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class SplashScreen : MonoBehaviour {
    MenuControls m_MenuControls;
    List<PlayerData> m_AllDatas;
    [HideInInspector] public int m_NextLevel;
	// Use this for initialization
	void Start () {
        m_MenuControls = FindObjectOfType<MenuControls>();
        m_AllDatas = Utilities.GetAllPlayerData();
        GameManager.instance.SetGameState(GameState.Intro);
	}
	
	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < m_AllDatas.Count; i++)
        {
            if (m_MenuControls.Confirm[i] || m_MenuControls._Start[i])
            {
                Application.LoadLevel(m_NextLevel);
                GameManager.instance.SetGameState(GameState.Loading);
            }
        }
	}
}
