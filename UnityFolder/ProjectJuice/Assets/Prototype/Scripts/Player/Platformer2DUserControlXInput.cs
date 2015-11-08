using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Platformer2DUserControlXInput : ExtendedMonobehaviour, IPlatformer2DUserControl {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (GameManager.instance.IsPlaying)
	    {
	        var state = GamePad.GetState(m_PlayerData.GamepadIndex.ToPlayerIndex());
	        m_Jump = state.GetButton(m_JumpButton) == ButtonState.Pressed;
	        m_Dash = state.GetButton(m_DashButton) == ButtonState.Pressed;
	        m_Shoot = state.GetButton(m_ShootButton) == ButtonState.Pressed;
	        m_Melee = state.GetButton(m_MeleeButton) == ButtonState.Pressed;
	        m_SpecialStay = m_Special = state.GetButton(m_SpecialButton) == ButtonState.Pressed;
	        m_Imobilize = state.GetButton(m_ImobilizeButton) == ButtonState.Pressed;
	        m_XAxis = state.ThumbSticks.Left.X;
	        m_YAxis = state.ThumbSticks.Right.Y;
	    }
	}
    

    //Gamepad buttons used
    
    [SerializeField]
    private GamepadInput.GamePad.Button m_JumpButton = GamepadInput.GamePad.Button.A;
    [SerializeField]
    private GamepadInput.GamePad.Button m_DashButton = GamepadInput.GamePad.Button.B;
    [SerializeField]
    private GamepadInput.GamePad.Button m_ShootButton = GamepadInput.GamePad.Button.X;
    [SerializeField]
    private GamepadInput.GamePad.Button m_MeleeButton = GamepadInput.GamePad.Button.Y;
    [SerializeField]
    private GamepadInput.GamePad.Button m_SpecialButton = GamepadInput.GamePad.Button.RightShoulder;
    [SerializeField]
    private GamepadInput.GamePad.Button m_ImobilizeButton = GamepadInput.GamePad.Button.LeftShoulder;
    
    public bool m_Jump { get; private set; }
    public bool m_Dash { get; private set; }
    public float m_XAxis { get; private set; }
    public float m_YAxis { get; private set; }
    public bool m_Shoot { get; private set; }
    public bool m_Melee { get; private set; }
    public bool m_Special { get; private set; }
    public bool m_Imobilize { get; private set; }
    public bool m_FacingRight { get; set; }
    private PlayerData _m_playerData;
    [SerializeField] private PlayerIDs m_PlayerID;

    public PlayerData m_PlayerData
    {
        get
        {
            if (_m_playerData == null)
            {
                _m_playerData = Utilities.GetPlayerData(m_PlayerID);
            }
            return _m_playerData;
        }
    }
    public bool m_SpecialStay { get; private set; }
    public PlayerIDs PlayerID { get; set; }
    public void FlushInputs()
    {
        //throw new System.NotImplementedException();
    }
}
