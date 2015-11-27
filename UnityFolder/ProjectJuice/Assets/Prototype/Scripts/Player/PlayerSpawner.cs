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
    [SerializeField] List<GameObject> m_Spawners;
    [SerializeField] GameObject m_PlayerPrefab;
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    public List<PlayerData> ListOfPlayerDatas { get { return m_ListofPlayers; } }
    private List<GameObject> m_Players = new List<GameObject>();
    public List<GameObject> Players { get { return m_Players; } }
    [SerializeField]
    private int[] m_PlayerLayerIDs = { 15, 16, 17, 18 };
    private LoadingScreen m_Fader;
    private Activator _activator;
    private bool _subscribed = false;
    [SerializeField] private List<Sprite> _playerIdSprites;
    [Range(0,5)][SerializeField] private float _flickerTimer = 1f;
    [Range(0,20)][SerializeField] private int _flickerFrameInterval = 2;
    private int _displayedIndicators = 0;
    private List<SpriteRenderer> _listOfActiveIndicators = new List<SpriteRenderer>();

    // Use this for initialization
    void Awake()
    {
        GameManager.instance.SetGameState(GameState.Loading);
        _activator = FindObjectOfType<Activator>();
    }

    void Start()
    {
        m_Fader = FindObjectOfType<LoadingScreen>();
        if (m_Fader == null && _activator != null) m_Fader = _activator.Loader;
        SubscribeToEvents(m_Fader);
    }

    public void SubscribeToEvents(LoadingScreen toUse)
    {
        if (!_subscribed)
        {
            m_Fader = toUse;
            m_Fader.LoadingAnimDone += M_Fader_FadeDone;
            m_Fader.LoadDone += M_Fader_LoadDone;
            _subscribed = true;
        }
    }

    private void M_Fader_LoadDone(object sender, EventArgs e)
    {
        //Debug.LogWarning("in playerspawner fader_loaddone");
        m_ListofPlayers = Utilities.GetAllPlayerData();
        SpawnPlayers();
    }

    private void M_Fader_FadeDone(object sender, EventArgs e)
    {
        if (_displayedIndicators > 0) StartAllFlickers();
    }

    public event EventHandler SpawnDone;

    protected virtual void OnSpawnDone()
    {
        //Debug.LogWarning("in playerspawner OnSpawnDone");
        EventHandler handler = SpawnDone;
        if (handler != null) handler(this, EventArgs.Empty);
    }

    void SpawnPlayers()
    {
        if(m_randomizePlayersOnSpawn)
            m_Spawners = Utilities.RandomizeList<GameObject>(m_Spawners);

        int i = 0;
        foreach (PlayerData pd in m_ListofPlayers)
        {
            GameObject temp = Instantiate(m_PlayerPrefab, m_Spawners[i].transform.position, m_Spawners[i].transform.rotation) as GameObject;
            IPlatformer2DUserControl pUserControl = temp.GetComponent<IPlatformer2DUserControl>();
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
            SetPlayerIndicator(m_Spawners, i, pd);
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
        _listOfActiveIndicators.Clear();
        _displayedIndicators = 0;
    }

    public void RespawnPlayers()
    {
        m_Fader.Respawn();
        DestroyRemainingSpawnedPlayers();
        SpawnPlayers();
    }

    void SetAbility(GameObject toAbilitize, PlayerData pd)
    {
        Abilities myAbility = pd.PlayerAbility;

        shield leShield = toAbilitize.GetComponent<shield>();
        leShield.ChangeVisibilty(leShield.m_Gun);
        leShield.enabled = false;

        IDartGun leDart = toAbilitize.GetComponent<IDartGun>();
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
                IPlatformer2DUserControl controls = pg.GetComponent<IPlatformer2DUserControl>();
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

    private void SetPlayerIndicator(List<GameObject> locations, int spawnLocation, PlayerData player)
    {
        int playerInt = GetIntFromPlayerData(player);
        SpriteRenderer toChange = locations[spawnLocation].GetComponentInChildren<SpriteRenderer>();
        if (playerInt > -1 && toChange != null && _playerIdSprites.Count > 0)
        {
            toChange.sprite = _playerIdSprites[playerInt];
            toChange.color = player.PlayerSponsor.SponsorColor;
            toChange.enabled = true;
            _displayedIndicators++;
            _listOfActiveIndicators.Add(toChange);
        }
    }

    private int GetIntFromPlayerData(PlayerData playerData)
    {
        int ret = -1;

        switch (playerData.playerID)
        {
            case PlayerIDs.A:
                ret = 0;
                break;
            case PlayerIDs.B:
                ret = 1;
                break;
            case PlayerIDs.C:
                ret = 2;
                break;
            case PlayerIDs.D:
                ret = 3;
                break;
        }

        return ret;
    }

    private void StartAllFlickers()
    {
        foreach(SpriteRenderer toChange in _listOfActiveIndicators)
            StartCoroutine(FlickerThenClose(toChange));
    }

    private IEnumerator FlickerThenClose(SpriteRenderer toFlicker)
    {
        float timer = Time.time + _flickerTimer;
        while(Time.time < timer)
        {
            for (int i = 0; i < _flickerFrameInterval; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            toFlicker.enabled = false;
            for (int i = 0; i < _flickerFrameInterval; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            toFlicker.enabled = true;
        }
        for (int i = 0; i < _flickerFrameInterval; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        toFlicker.enabled = false;
    }
}
