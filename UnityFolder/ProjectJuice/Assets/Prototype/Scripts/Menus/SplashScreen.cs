using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class SplashScreen : MonoBehaviour {
    MenuControls m_MenuControls;
    List<PlayerData> m_AllDatas;
    [Range(0,5)][SerializeField] private float m_DelayForTransition;
    [HideInInspector] public int m_NextLevel;
    [HideInInspector] public string CrowdClip; 
    [HideInInspector] public string PressClip;
	// Use this for initialization
	void Start ()
    {
        SoundManager.PlaySFX(CrowdClip, true);
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
                StartCoroutine(GotoNextScene());
            }
        }
	}

    IEnumerator GotoNextScene()
    {
        SoundManager.PlaySFX(PressClip);
        yield return new WaitForSeconds(m_DelayForTransition);
        Application.LoadLevel(m_NextLevel);
    }
}
