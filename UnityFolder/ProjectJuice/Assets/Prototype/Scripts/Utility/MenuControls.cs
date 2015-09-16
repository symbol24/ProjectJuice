using UnityEngine;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;
using Utility;

public class MenuControls : MonoBehaviour {
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();

    [SerializeField] public GamePad.Button m_ConfirmButton = GamePad.Button.A;
    [SerializeField] public GamePad.Button m_CancelButton = GamePad.Button.B;
    [SerializeField] public GamePad.Button m_StartButton = GamePad.Button.Start;
    [SerializeField] public GamePad.Axis m_DirectionalButton = GamePad.Axis.LeftStick;


    public List<bool> m_Confirm = new List<bool>();
    public List<bool> m_Cancel = new List<bool>();
    public List<bool> m_Start = new List<bool>();
    public List<float> m_X = new List<float>();
    public List<float> m_Y = new List<float>();

    // Use this for initialization
    void Start () {
        m_ListofPlayers = Utilities.GetAllPlayerData();
        SetInputs();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < m_ListofPlayers.Count; i++)
            ReadInputs(i);
	}

    void SetInputs()
    {
        foreach(PlayerData pd in m_ListofPlayers)
        {
            m_Confirm.Add(false);
            m_Cancel.Add(false);
            m_Start.Add(false);
            m_X.Add(0);
            m_Y.Add(0);
        }
    }

    void ReadInputs(int id)
    {
        //m_Confirm[i] = GamePad.GetButtonDown(GamePad.Button.A, m_ListofPlayers.)
    }
}
