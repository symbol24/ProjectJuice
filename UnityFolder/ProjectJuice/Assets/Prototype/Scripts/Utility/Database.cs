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

    [SerializeField] private float m_MenuInputDelay = 0.4f;
    public float MenuInputDelay { get { return m_MenuInputDelay; } }

    [SerializeField] Color m_TakenColorForMenu;
    public Color TakenColor { get { return m_TakenColorForMenu; } }

    [SerializeField] Color m_NormalTextColorForMenu;
    public Color NormalTextColor { get { return m_TakenColorForMenu; } }

    [SerializeField] Font m_BaseFont;
    public Font BaseFont { get { return m_BaseFont; } }

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
