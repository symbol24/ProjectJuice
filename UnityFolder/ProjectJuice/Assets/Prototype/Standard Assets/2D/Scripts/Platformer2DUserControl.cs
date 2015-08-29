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
        private bool m_Jump;
        GamePad.Index controller = GamePad.Index.Any;
        GamepadState state;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            state = GamePad.GetState(controller);
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = GamePad.GetButtonDown(GamePad.Button.A, controller);
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = GamePad.GetButton(GamePad.Button.B, controller);
            float h = GamePad.GetAxis(GamePad.Axis.LeftStick, controller).x;
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
