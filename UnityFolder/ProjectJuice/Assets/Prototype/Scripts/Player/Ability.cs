using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {

	[SerializeField] private string m_AbilityName;
    public string AbilityName { get { return m_AbilityName; } }

    [SerializeField] private string m_Description;
    public string Description { get { return m_Description; } }

    [SerializeField] private Abilities m_AbilityEnum;
    public Abilities AbilityEnum { get { return m_AbilityEnum; } }

    [SerializeField] private Sprite m_Sprite;
    public Sprite mySprite { get { return m_Sprite; } }
}
