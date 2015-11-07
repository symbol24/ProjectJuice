using UnityEngine;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;

public class MenuControls : MonoBehaviour {
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();

    [SerializeField] private GamePad.Button m_ConfirmButton = GamePad.Button.A;
    [SerializeField] private GamePad.Button m_CancelButton = GamePad.Button.B;
    [SerializeField] private GamePad.Button m_StartButton = GamePad.Button.Start;
    [SerializeField] private GamePad.Axis m_DirectionJoystic = GamePad.Axis.LeftStick;

    private List<bool> m_Confirm = new List<bool>();
    private List<bool> m_Cancel = new List<bool>();
    private List<bool> m_Start = new List<bool>();
    private List<float> m_X = new List<float>();
    private List<float> m_Y = new List<float>();

    public List<bool> Confirm { get { return m_Confirm; } }
    public List<bool> Cancel { get { return m_Cancel; } }
    public List<bool> _Start { get { return m_Start; } }
    public List<float> X { get { return m_X; } }
    public List<float> Y { get { return m_Y; } }

    private LoadingScreen m_Fader;

    void Awake()
    {
    }

    private void M_Fader_FadeDone(object sender, System.EventArgs e)
    {
        m_Fader = FindObjectOfType<LoadingScreen>();
        m_Fader.FadeDone += M_Fader_FadeDone;
        m_ListofPlayers = Utilities.GetAllPlayerData();
    }

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
        for(int i = 0; i < m_ListofPlayers.Count; i++)
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
        m_Confirm[id] = GamePad.GetButtonDown(m_ConfirmButton, m_ListofPlayers[id].GamepadIndex);
        m_Cancel[id] = GamePad.GetButtonDown(m_CancelButton, m_ListofPlayers[id].GamepadIndex);
        m_Start[id] = GamePad.GetButtonDown(m_StartButton, m_ListofPlayers[id].GamepadIndex);
        m_X[id] = GamePad.GetAxis(m_DirectionJoystic, m_ListofPlayers[id].GamepadIndex).x;
        m_Y[id] = GamePad.GetAxis(m_DirectionJoystic, m_ListofPlayers[id].GamepadIndex).y;
    }
}
