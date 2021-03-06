﻿using UnityEngine;
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

    void Awake()
    {
        if (FindObjectsOfType<Database>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    [Range(0,5)][SerializeField] private int m_AmountOfRounds = 3;
    public int AmountOfRounds { get { return m_AmountOfRounds; } }

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

    [SerializeField] float m_MeleeAbilityAerialDamage;
    public float MeleeAbilityAerialDamage { get { return m_MeleeAbilityAerialDamage; } }

    [SerializeField] float m_ExplosionBaseDamage;
    public float ExplosionBaseDamage { get { return m_ExplosionBaseDamage; } }

    [Range(0,10)][SerializeField] float m_BackDamageMultiplier = 1.4f;
    public float BackDamageMultiplier { get { return m_BackDamageMultiplier; } }

    [Range(0,1)][SerializeField] float m_MenuNavigationDeadZone = 0.5f;
    public float MenuNavigationDeadZone { get { return m_MenuNavigationDeadZone; } }

    [SerializeField] private ParticleSystem[] m_Particles;
    public ParticleSystem[] Particles { get { return m_Particles; } }

    [HideInInspector] public int MainMenuID;
    [HideInInspector] public string MenuSlideName;
    [HideInInspector] public string MenuClickName;
    [HideInInspector] public string MenuCancelName;
    [HideInInspector] public string MenuErrorName;
    [HideInInspector] public string RobotDeath;
    [HideInInspector] public string RobotDeathCrowd;
    [HideInInspector] public string Jump;
    [HideInInspector] public string Landing;
    [HideInInspector] public string Dash;
    [HideInInspector] public string DashMetalGrind;
    [HideInInspector] public string JuicePickup;

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
