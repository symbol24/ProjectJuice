using System;
using UnityEngine;
using System.Collections;

public class Platformer2DUserControlKeyboard : MonoBehaviour, IPlatformer2DUserControl
{

    [SerializeField] 
    private KeyCode _m_jump = KeyCode.Space;
    public bool m_Jump { get; private set; }
    [SerializeField]
    private KeyCode _m_dash = KeyCode.C;
    public bool m_Dash { get; private set; }
    [SerializeField]
    private KeyCode _m_left = KeyCode.LeftArrow;
    [SerializeField]
    private KeyCode _m_right = KeyCode.RightArrow;
    public float m_XAxis { get; private set; }
    [SerializeField]
    private KeyCode _m_up = KeyCode.UpArrow;
    [SerializeField] 
    private KeyCode _m_down = KeyCode.DownArrow;
    public float m_YAxis { get; private set; }
    [SerializeField]
    private KeyCode _m_shoot = KeyCode.LeftControl;
    public bool m_Shoot { get; private set; }
    [SerializeField]
    private KeyCode _m_melee = KeyCode.X;
    public bool m_Melee { get; private set; }
    [SerializeField]
    private KeyCode _m_special = KeyCode.Z;
    public bool m_Special { get; private set; }
    [SerializeField]
    private KeyCode _m_imobilize = KeyCode.LeftAlt;

    private bool _mFacingRight = true;
    public bool m_Imobilize { get; private set; }

    public bool m_FacingRight
    {
        get { return _mFacingRight; }
        set { _mFacingRight = value; }
    }

    void Update()
    {
        m_Jump = Input.GetKeyDown(_m_jump);
        m_Dash = Input.GetKeyDown(_m_dash);
        m_XAxis = Convert.ToInt32(Input.GetKey(_m_right)) - (Input.GetKey(_m_left) ? 1 : 0);
        m_YAxis = Convert.ToInt32(Input.GetKey(_m_up)) - (Input.GetKey(_m_down) ?  1 : 0);
        m_Shoot = Input.GetKeyDown(_m_shoot);
        m_Melee = Input.GetKeyDown(_m_melee);
        m_Special = Input.GetKeyDown(_m_special);
        m_Imobilize = Input.GetKeyDown(_m_imobilize);
    }

}
