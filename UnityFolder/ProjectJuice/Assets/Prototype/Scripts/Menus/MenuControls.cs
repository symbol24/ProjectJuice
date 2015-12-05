using UnityEngine;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;

public class MenuControls : MonoBehaviour {
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();

    [SerializeField] private GamePad.Button m_ConfirmButton = GamePad.Button.A;
    [SerializeField] private GamePad.Button m_CancelButton = GamePad.Button.B;
    [SerializeField] private GamePad.Button m_StartButton = GamePad.Button.Start;

    private List<XInputDotNetPure.ButtonState> m_previousConfirm = new List<XInputDotNetPure.ButtonState>();
    private List<bool> m_Confirm = new List<bool>();
    private List<XInputDotNetPure.ButtonState> m_previousCancel = new List<XInputDotNetPure.ButtonState>();
    private List<bool> m_Cancel = new List<bool>();
    private List<XInputDotNetPure.ButtonState> m_previousStart = new List<XInputDotNetPure.ButtonState>();
    private List<bool> m_Start = new List<bool>();
    private List<float> m_X = new List<float>();
    private List<float> m_Y = new List<float>();

    public List<bool> Confirm { get { return m_Confirm; } }
    public List<bool> Cancel { get { return m_Cancel; } }
    public List<bool> _Start { get { return m_Start; } }
    public List<float> X { get { return m_X; } }
    public List<float> Y { get { return m_Y; } }

    private LoadingScreen _loader;

    private bool _loadDoneReceived = false;

    void Awake()
    {
    }

    private void LoadDone(object sender, System.EventArgs e)
    {
        GetPlayers();
    }

    private void GetPlayers()
    {
        m_ListofPlayers = Utilities.GetAllPlayerData();
        SetInputs();
        _loadDoneReceived = true;
    }

    // Use this for initialization
    void Start ()
    {
        _loader = FindObjectOfType<LoadingScreen>();
        if (_loader != null) _loader.LoadDone += LoadDone;
        else
        {
            m_ListofPlayers = Utilities.GetAllPlayerData();
            SetInputs();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!_loadDoneReceived) GetPlayers();

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
            m_previousConfirm.Add(XInputDotNetPure.ButtonState.Released);
            m_previousCancel.Add(XInputDotNetPure.ButtonState.Released);
            m_previousStart.Add(XInputDotNetPure.ButtonState.Released);
            m_X.Add(0);
            m_Y.Add(0);
        }
    }

    void ClearInputs()
    {
        m_Confirm.Clear();
        m_Cancel.Clear();
        m_Start.Clear();
        m_X.Clear();
        m_Y.Clear();
    }

    void ReadInputs(int id)
    {
        var gamepad = m_ListofPlayers[id].GamepadIndex.ToPlayerIndex();
        if (gamepad == null) return;
        var state = XInputDotNetPure.GamePad.GetState((XInputDotNetPure.PlayerIndex)gamepad);
        var currentConfirmState = state.GetButton(m_ConfirmButton);
        var currentCancelState = state.GetButton(m_CancelButton);
        var currentStartState = state.GetButton(m_StartButton);

        m_Confirm[id] = currentConfirmState == XInputDotNetPure.ButtonState.Pressed && currentConfirmState != m_previousConfirm[id];
        m_Cancel[id] = currentCancelState == XInputDotNetPure.ButtonState.Pressed && currentCancelState != m_previousCancel[id];
        m_Start[id] = currentStartState == XInputDotNetPure.ButtonState.Pressed && currentStartState != m_previousStart[id];

        m_previousConfirm[id] = currentConfirmState;
        m_previousCancel[id] = currentCancelState;
        m_previousStart[id] = currentStartState;

        m_X[id] = state.ThumbSticks.Left.X;
        m_Y[id] = state.ThumbSticks.Left.Y;


        /*
        m_Confirm[id] = GamePad.GetButtonDown(m_ConfirmButton, m_ListofPlayers[id].GamepadIndex);
        m_Cancel[id] = GamePad.GetButtonDown(m_CancelButton, m_ListofPlayers[id].GamepadIndex);
        m_Start[id] = GamePad.GetButtonDown(m_StartButton, m_ListofPlayers[id].GamepadIndex);
        m_X[id] = GamePad.GetAxis(m_DirectionJoystic, m_ListofPlayers[id].GamepadIndex).x;
        m_Y[id] = GamePad.GetAxis(m_DirectionJoystic, m_ListofPlayers[id].GamepadIndex).y;*/
    }

    public void SetPlayerDatas(List<PlayerData> newPlayerDatas)
    {
        m_ListofPlayers = newPlayerDatas;
    }

    public void Restart()
    {
        ClearInputs();
        Start();
    }
}
