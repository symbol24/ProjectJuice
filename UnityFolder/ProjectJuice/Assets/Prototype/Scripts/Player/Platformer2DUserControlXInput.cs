using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Platformer2DUserControlXInput : ExtendedMonobehaviour, IPlatformer2DUserControl {

	// Use this for initialization
	void Awake() {
        m_FacingRight = true;
    }
	
	// Update is called once per frame
	void Update () {
	    if (GameManager.instance.IsPlaying)
	    {
	        var state = GamePad.GetState(m_PlayerData.GamepadIndex.ToPlayerIndex());

            var currentJump = state.GetButton(m_JumpButton);
            var currentDash = state.GetButton(m_DashButton);
            var currentSpecial = state.GetButton(m_SpecialButton);

            m_Jump = currentJump == ButtonState.Pressed && previousJumpButton != currentJump;
	        m_Dash = currentDash == ButtonState.Pressed && previousDashButton != currentDash;
	        m_Shoot = state.GetButton(m_ShootButton) == ButtonState.Pressed;
	        m_Melee = state.GetButton(m_MeleeButton) == ButtonState.Pressed;
            m_Special = currentSpecial == ButtonState.Pressed && currentDash != previousSpecial;
	        m_SpecialStay = currentSpecial == ButtonState.Pressed;
	        m_Imobilize = state.GetButton(m_ImobilizeButton) == ButtonState.Pressed;
	        m_XAxis = state.ThumbSticks.Left.X;
	        m_YAxis = state.ThumbSticks.Left.Y;

            previousDashButton = currentDash;
            previousJumpButton = currentJump;
            previousSpecial = currentSpecial;
        }
	}
    //jump dash and special



    //Gamepad buttons used
    
    private ButtonState previousJumpButton = ButtonState.Released;
    private ButtonState previousDashButton = ButtonState.Released;
    private ButtonState previousSpecial = ButtonState.Released;

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
            if (_m_playerData == null && m_PlayerID != PlayerIDs.X)
            {
                _m_playerData = Utilities.GetPlayerData(m_PlayerID);
            }
            return _m_playerData;
        }
    }
    public bool m_SpecialStay { get; private set; }
    private PlayerIDs _playerId = PlayerIDs.X;
    public PlayerIDs PlayerID { get { return _playerId; } set
        {
            if(_playerId != value)
            {
                _playerId = value;
                _m_playerData = Utilities.GetPlayerData(_playerId);
            }
        }
        }
    public void FlushInputs()
    {
        m_Jump = false;
        m_Dash = false;
        m_XAxis = 0.0f;
        m_YAxis = 0.0f;
        m_Shoot = false;
        m_Melee = false;
        m_Special = false;
        m_Imobilize = false;
        m_SpecialStay = false;
    }
}
