using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using Utility;
using System;

public class PlayerSpawner : MonoBehaviour {

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

    [SerializeField] GameObject[] m_Spawners;
    [SerializeField] GameObject m_PlayerPrefab;
    [SerializeField] private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    public List<PlayerData> ListOfPlayerDatas { get { return m_ListofPlayers; } }
    private List<GameObject> m_Players = new List<GameObject>();
    public List<GameObject> Players { get { return m_Players; } }
    [SerializeField] private int[] m_PlayerLayerIDs = {15,16,17,18};
    private FadeOut m_Fader;

	// Use this for initialization
	void Start () {
        GameManager.instance.SetGameState(GameState.Loading);
        m_Fader = FindObjectOfType<FadeOut>();
        m_Fader.FadeDone += M_Fader_FadeDone;
    }

    private void M_Fader_FadeDone(object sender, EventArgs e)
    {
        GameManager.instance.SetGameState(GameState.Playing);

        m_ListofPlayers = Utilities.GetAllPlayerData();
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        int i = 0;
        foreach(PlayerData pd in m_ListofPlayers)
        {
            GameObject temp = Instantiate(m_PlayerPrefab, m_Spawners[i].transform.position, m_Spawners[i].transform.rotation) as GameObject;
            Platformer2DUserControl pUserControl = temp.GetComponent<Platformer2DUserControl>();
            pUserControl.PlayerID = pd.playerID;
            SetAbility(temp, pd);
            temp.layer = m_PlayerLayerIDs[i];
            ScoreManager.instance.FollowScoreOf(pUserControl);
            m_Players.Add(temp);
            StartCoroutine(DelayedEvent(0.2f, pd));
            i++;
        }
    }

    public event EventHandler<PlayerColorEventArgs> ChangeColor;

    protected virtual void OnChangeColor(PlayerData player)
    {
        PlayerColorEventArgs e = new PlayerColorEventArgs();
        e.player = player;

        EventHandler<PlayerColorEventArgs> handler = ChangeColor;
        if (handler != null) handler(this, e);
    }

    public void DestroyRemainingSpawnedPlayers()
    {
        for(int i = 0; i < m_Players.Count; i++)
        {
            if(m_Players[i] != null)
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

    IEnumerator DelayedEvent(float timer, PlayerData pd)
    {
        yield return new WaitForSeconds(timer);
        OnChangeColor(pd);
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
        foreach(MeleeAttack ma in leMelees)
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
}
