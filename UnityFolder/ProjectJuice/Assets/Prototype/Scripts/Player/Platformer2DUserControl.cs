using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : ExtendedMonobehaviour, IPlatformer2DUserControl
    {
        public PlayerData m_PlayerData { get; private set; }
        
        [SerializeField] private PlayerIDs m_PlayerID;
        public PlayerIDs PlayerID { get { return m_PlayerID; } set { m_PlayerID = value; } }

        private bool _m_facingRight = true;

        public bool m_FacingRight
        {
            get { return _m_facingRight; }
            set { _m_facingRight = value; }
        } // For determining which way the player is currently facing.

        //Gamepad values
        public bool m_Jump { get; private set; }
        public bool m_Dash { get; private set; }
        public float m_XAxis { get; private set; }
        public float m_YAxis { get; private set; }
        public bool m_Shoot { get; private set; } 
        public bool m_Melee { get; private set; }
        public bool m_Special { get; private set; }
        public bool m_Imobilize { get; private set; }
        public bool m_SpecialStay { get; private set; }

        public void Vibrate(float leftSide, float rightSide)
        {
            XInputDotNetPure.GamePad.SetVibration(m_PlayerData.GamepadIndex.ToPlayerIndex(), leftSide, rightSide);
        }

        //Gamepad buttons used
        [SerializeField] private GamePad.Button m_JumpButton = GamePad.Button.A;
        [SerializeField] private GamePad.Button m_DashButton = GamePad.Button.B;
        [SerializeField] private GamePad.Axis m_DirectionalButton = GamePad.Axis.LeftStick;
        [SerializeField] private GamePad.Button m_ShootButton = GamePad.Button.X;
        [SerializeField] private GamePad.Button m_MeleeButton = GamePad.Button.Y;
        [SerializeField] private GamePad.Button m_SpecialButton = GamePad.Button.RightShoulder;
        [SerializeField] private GamePad.Button m_ImobilizeButton = GamePad.Button.LeftShoulder;

        //Gamepad ID
        GamePad.Index controller = GamePad.Index.Any;


        private void Awake()
        {
            //m_PlayerData = GetPlayerData();
            //controller = m_PlayerData.GamepadIndex; //Get the index of the gamepad from the playerdata.
        }

        void Start()
        {
            m_PlayerData = Utilities.GetPlayerData(m_PlayerID);
            controller = m_PlayerData.GamepadIndex; //Get the index of the gamepad from the playerdata.
        }

        private void Update()
        {
            // Read the inputs.
            /*m_Jump = GamePad.GetButtonDown(m_JumpButton, controller);
            m_Dash = GamePad.GetButtonDown(m_DashButton, controller);
            m_XAxis = GamePad.GetAxis(m_DirectionalButton, controller).x;
            m_YAxis = GamePad.GetAxis(m_DirectionalButton, controller).y;
            m_Shoot = GamePad.GetButtonDown(m_ShootButton, controller);
            m_Melee = GamePad.GetButtonDown(m_MeleeButton, controller);
            m_Special = GamePad.GetButtonDown(m_SpecialButton, controller);
            m_Imobilize = GamePad.GetButton(m_ImobilizeButton, controller);
            m_SpecialStay = GamePad.GetButton(m_SpecialButton, controller);
            */
            if (GameManager.instance.IsPlaying)
            {
                // Read the inputs.
                m_Jump = GamePad.GetButtonDown(m_JumpButton, controller);
                m_Dash = GamePad.GetButtonDown(m_DashButton, controller);
                m_XAxis = GamePad.GetAxis(m_DirectionalButton, controller).x;
                m_YAxis = GamePad.GetAxis(m_DirectionalButton, controller).y;
                m_Shoot = GamePad.GetButton(m_ShootButton, controller);
                m_Melee = GamePad.GetButton(m_MeleeButton, controller);
                m_Special = GamePad.GetButtonDown(m_SpecialButton, controller);
                m_Imobilize = GamePad.GetButton(m_ImobilizeButton, controller);
                m_SpecialStay = GamePad.GetButton(m_SpecialButton, controller);
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
}
