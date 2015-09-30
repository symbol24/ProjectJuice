using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Database : MonoBehaviour
{

    private static Database _instance;

    public static Database instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Database>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    [SerializeField]
    List<Sponsor> m_ListofSponsors;
    public List<Sponsor> ListofSponsors { get { return m_ListofSponsors; } }

    [SerializeField]
    List<Ability> m_ListofAbilities;
    public List<Ability> ListofAbilities { get { return m_ListofAbilities; } }

    [SerializeField]
    List<string> m_GameTexts;
    public List<string> GameTexts { get { return m_GameTexts; } }

    [Range(0,2)][SerializeField] private float m_MenuInputDelay = 0.4f;
    public float MenuInputDelay { get { return m_MenuInputDelay; } }

    [Range(0,5)][SerializeField] private float m_LongMenuInputDelay = 1.5f;
    public float LongMenuInputDelay { get { return m_LongMenuInputDelay; } }

    [SerializeField] Color m_TakenColorForMenu;
    public Color TakenColor { get { return m_TakenColorForMenu; } }

    [SerializeField] Color m_NormalTextColorForMenu;
    public Color NormalTextColor { get { return m_TakenColorForMenu; } }

    [SerializeField] Font m_BaseFont;
    public Font BaseFont { get { return m_BaseFont; } }

    [SerializeField] float m_PlayerBaseHP;
    public float PlayerBaseHP { get { return m_PlayerBaseHP; } }

    [SerializeField] float m_BulletBaseDamage;
    public float BulletBaseDamage { get { return m_BulletBaseDamage; } }

    [SerializeField] float m_DartBaseDmgPerSecond;
    public float DartBaseDmgPerSecond { get { return m_DartBaseDmgPerSecond; } }

    [SerializeField] float m_MeleeBaseDamage;
    public float MeleeBaseDamage { get { return m_MeleeBaseDamage; } }

    [SerializeField] float m_MeleeAbilityDamage;
    public float MeleeAbilityDamage { get { return m_MeleeAbilityDamage; } }

    [Range(0,10)][SerializeField] float m_BackDamageMultiplier = 1.4f;
    public float BackDamageMultiplier { get { return m_BackDamageMultiplier; } }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        CleanSponsors();
    }

    private void CleanSponsors()
    {
        foreach (Sponsor sp in m_ListofSponsors)
            sp.ReleaseSponsor();
    }
}
