using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashScreen : MonoBehaviour {
    MenuControls m_MenuControls;
    List<PlayerData> m_AllDatas;
    [Range(0,5)][SerializeField] private float m_DelayForTransition;
    [SerializeField] private Animator _pressStartAnimator;
    [HideInInspector] public int m_NextLevel;
    [HideInInspector] public string CrowdClip; 
    [HideInInspector] public string PressClip;
	// Use this for initialization
	void Start ()
    {
        SoundManager.PlaySFX(CrowdClip, true);
        m_MenuControls = FindObjectOfType<MenuControls>();
        m_AllDatas = Utilities.GetAllPlayerData();
        GameManager.instance.SetGameState(GameState.Loading);
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.instance.CurrentState == GameState.Intro)
        {
            for (int i = 0; i < m_AllDatas.Count; i++)
            {
                if (m_MenuControls.Confirm[i] || m_MenuControls._Start[i])
                {
                    StartCoroutine(GotoNextScene());
                }
            }
        }
	}

    IEnumerator GotoNextScene()
    {
        _pressStartAnimator.SetBool("Pressed", true);
        SoundManager.PlaySFX(PressClip);
        yield return new WaitForSeconds(m_DelayForTransition);
        Application.LoadLevel(m_NextLevel);
    }
}
