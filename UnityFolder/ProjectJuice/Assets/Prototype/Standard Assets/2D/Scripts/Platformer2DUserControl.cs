using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        public bool m_FacingRight = true;  // For determining which way the player is currently facing.

        //Gamepad values
        public bool m_Jump { get; private set; }
        public bool m_Dash { get; private set; }
        public float m_XAxis { get; private set; }
        public float m_YAxis { get; private set; }
        public bool m_Shoot { get; private set; } 
        public bool m_Melee { get; private set; }
        public bool m_Special { get; private set; }
        public bool m_Imobilize { get; private set; }

        //Gamepad buttons used
        [SerializeField]
        private GamePad.Button m_JumpButton = GamePad.Button.A;
        [SerializeField]
        private GamePad.Button m_DashButton = GamePad.Button.B;
        [SerializeField]
        private GamePad.Axis m_DirectionalButton = GamePad.Axis.LeftStick;
        [SerializeField]
        private GamePad.Button m_ShootButton = GamePad.Button.X;
        [SerializeField]
        private GamePad.Button m_MeleeButton = GamePad.Button.Y;
        [SerializeField]
        private GamePad.Button m_SpecialButton = GamePad.Button.RightShoulder;
        [SerializeField]
        private GamePad.Button m_ImobilizeButton = GamePad.Button.LeftShoulder;

        //Gamepad ID
        GamePad.Index controller = GamePad.Index.Any;


        private void Awake()
        {
            controller = GamePad.Index.One; //Get the index of the gamepad from the playerdata.
        }


        private void Update()
        {
            // Read the inputs.
            m_Jump = GamePad.GetButtonDown(m_JumpButton, controller);
            m_Dash = GamePad.GetButtonDown(m_DashButton, controller);
            m_XAxis = GamePad.GetAxis(m_DirectionalButton, controller).x;
            m_YAxis = GamePad.GetAxis(m_DirectionalButton, controller).y;
            m_Shoot = GamePad.GetButtonDown(m_ShootButton, controller);
            m_Melee = GamePad.GetButtonDown(m_MeleeButton, controller);
            m_Special = GamePad.GetButtonDown(m_SpecialButton, controller);
            m_Imobilize = GamePad.GetButtonDown(m_ImobilizeButton, controller);
        }


        private void FixedUpdate()
        {
            // Resets the jump button so that you have to press again to jump
            //m_Jump = false;
            //m_Dash = false;
        }
    }
}
