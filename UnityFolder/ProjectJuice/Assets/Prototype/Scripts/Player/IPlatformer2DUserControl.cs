using UnityEngine;
using System.Collections;

public interface IPlatformer2DUserControl : IGameObject
{
    bool m_Jump { get; }
    bool m_Dash { get; }
    float m_XAxis { get; }
    float m_YAxis { get; }
    bool m_Shoot { get; }
    bool m_Melee { get; }
    bool m_Special { get; }
    bool m_Imobilize { get; }
    bool m_FacingRight { get; set; }
    PlayerData m_PlayerData { get; }
    bool m_SpecialStay { get; }
}
