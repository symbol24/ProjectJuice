using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using System;

public class PlayerSpawner : MonoBehaviour
{

    #region Instance Stuffs
    private static PlayerSpawner _instance;

    public static PlayerSpawner instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlayerSpawner>();

            return _instance;
        }
    }
    #endregion
    [SerializeField] private bool m_randomizePlayersOnSpawn = true;
    [SerializeField] GameObject[] m_Spawners;
    [SerializeField] GameObject m_PlayerPrefab;
    [SerializeField] private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    public List<PlayerData> ListOfPlayerDatas { get { return m_ListofPlayers; } }
    private List<GameObject> m_Players = new List<GameObject>();
    public List<GameObject> Players { get { return m_Players; } }
    [SerializeField]
    private int[] m_PlayerLayerIDs = { 15, 16, 17, 18 };
    private FadeOut m_Fader;
    private Activator _activator;

    // Use this for initialization
    void Awake()
    {
        GameManager.instance.SetGameState(GameState.Loading);
        _activator = FindObjectOfType<Activator>();
    }

    void Start()
    {
        m_Fader = FindObjectOfType<FadeOut>();
        if (m_Fader == null && _activator != null) m_Fader = _activator.Loader;
        m_Fader.FadeDone += M_Fader_FadeDone;
        m_Fader.LoadDone += M_Fader_LoadDone;
    }

    public void SubscribeToEvents(FadeOut toUse)
    {
        m_Fader = toUse;
        m_Fader.FadeDone += M_Fader_FadeDone;
        m_Fader.LoadDone += M_Fader_LoadDone;
    }

    private void M_Fader_LoadDone(object sender, EventArgs e)
    {
        Debug.LogWarning("in playerspawner fader_loaddone");
        m_ListofPlayers = Utilities.GetAllPlayerData(m_randomizePlayersOnSpawn);
        SpawnPlayers();
    }

    private void M_Fader_FadeDone(object sender, EventArgs e)
    {
        //GameManager.instance.SetGameState(GameState.Playing);
    }

    public event EventHandler SpawnDone;

    protected virtual void OnSpawnDone()
    {
        EventHandler handler = SpawnDone;
        if (handler != null) handler(this, EventArgs.Empty);
    }

    void SpawnPlayers()
    {
        int i = 0;
        foreach (PlayerData pd in m_ListofPlayers)
        {
            GameObject temp = Instantiate(m_PlayerPrefab, m_Spawners[i].transform.position, m_Spawners[i].transform.rotation) as GameObject;
            Platformer2DUserControl pUserControl = temp.GetComponent<Platformer2DUserControl>();
            PlatformerCharacter2D pc2d = temp.GetComponent<PlatformerCharacter2D>();
            ArcShooting gun = temp.GetComponent<ArcShooting>();
            pUserControl.PlayerID = pd.playerID;
            pc2d.SpawnCheckGround();
            SetAbility(temp, pd);
            temp.layer = m_PlayerLayerIDs[i];
            ScoreManager.instance.FollowScoreOf(pUserControl);
            if (m_Spawners[i].transform.position.x > 0)
            {
                pc2d.SpawnFlip();
                gun.SpawnRotation(pUserControl);
            }
            m_Players.Add(temp);
            i++;
        }
        OnSpawnDone();
    }

    public void DestroyRemainingSpawnedPlayers()
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i] != null)
            {
                Destroy(m_Players[i]);
            }
        }
        m_Players.Clear();
    }

    public void RespawnPlayers()
    {
        DestroyRemainingSpawnedPlayers();
        SpawnPlayers();
    }

    void SetAbility(GameObject toAbilitize, PlayerData pd)
    {
        Abilities myAbility = pd.PlayerAbility;

        shield leShield = toAbilitize.GetComponent<shield>();
        leShield.ChangeVisibilty(leShield.m_Gun);
        leShield.enabled = false;

        SappingDartGun leDart = toAbilitize.GetComponent<SappingDartGun>();
        leDart.enabled = false;

        MeleeAttack[] leMelees = toAbilitize.GetComponents<MeleeAttack>();
        MeleeAttack abilityMelee = null;
        MeleeAttack normalMelee = null;
        foreach (MeleeAttack ma in leMelees)
        {
            if (ma.isAbility) abilityMelee = ma;
            else normalMelee = ma;
        }
        if (abilityMelee != null) abilityMelee.enabled = false;

        switch (myAbility)
        {
            case Abilities.AbsorbShield:
                leShield.enabled = true;
                break;
            case Abilities.DoubleJump:
                break;
            case Abilities.Dart:
                leDart.enabled = true;
                break;
            case Abilities.Melee:
                normalMelee.enabled = false;
                abilityMelee.enabled = true;
                break;
            case Abilities.None:
                break;
        }
    }

    public void FlushAlivePlayerInputs()
    {
        foreach (GameObject pg in m_Players)
        {
            if (pg != null)
            {
                Platformer2DUserControl controls = pg.GetComponent<Platformer2DUserControl>();
                if (controls != null)
                    controls.FlushInputs();

                PlatformerCharacter2D charac = pg.GetComponent<PlatformerCharacter2D>();
                if (charac != null)
                    charac.FlushAnimState();

                DelayManager dm = pg.GetComponent<DelayManager>();
                if (dm != null)
                {
                    dm.AddDelay(Database.instance.MenuInputDelay);
                    dm.AddShieldDelay(Database.instance.MenuInputDelay);
                }
            }
        }
    }
}
