using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;

        public bool m_Jump { get; private set; }
        public bool m_Dash { get; private set; }
        public float m_Directional { get; private set; }
        public bool m_Shoot { get; private set; } 
        public bool m_Melee { get; private set; }
        public bool m_Special { get; private set; }
        public bool m_Imobilize { get; private set; }

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

        GamePad.Index controller = GamePad.Index.Any;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            m_Jump = GamePad.GetButton(m_JumpButton, controller);
            m_Dash = GamePad.GetButton(m_DashButton, controller);
            m_Directional = GamePad.GetAxis(m_DirectionalButton, controller).x;
            m_Shoot = GamePad.GetButton(m_ShootButton, controller);
            m_Melee = GamePad.GetButton(m_MeleeButton, controller);
            m_Special = GamePad.GetButton(m_SpecialButton, controller);
            m_Imobilize = GamePad.GetButton(m_ImobilizeButton, controller);
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            // Pass all parameters to the character control script.
            m_Character.Move(m_Directional, m_Dash, m_Jump);
            m_Jump = false;
        }
    }
}
