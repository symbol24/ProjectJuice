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
    public List<Sponsor> ListofSponsrs { get { return m_ListofSponsors; } }

    [SerializeField]
    List<Ability> m_ListofAbilities;
    public List<Ability> ListofAbilities { get { return m_ListofAbilities; } }

    [SerializeField]
    List<string> m_GameTexts;
    public List<string> GameTexts { get { return m_GameTexts; } }

    [SerializeField] private float m_MenuInputDelay = 0.4f;
    public float MenuInputDelay { get { return m_MenuInputDelay; } }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
