﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using Utility;

public class PlayerSpawner : MonoBehaviour {
    [SerializeField] GameObject[] m_Spawners;
    [SerializeField] GameObject m_PlayerPrefab;
    private List<PlayerData> m_ListofPlayers = new List<PlayerData>();
    private List<GameObject> m_Players = new List<GameObject>();

	// Use this for initialization
	void Start () {
        m_ListofPlayers = Utilities.GetAllPlayerData();
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        int i = 0;
        PlayerIDs p = PlayerIDs.A;
        foreach(PlayerData pd in m_ListofPlayers)
        {
            GameObject temp = Instantiate(m_PlayerPrefab, m_Spawners[i].transform.position, m_Spawners[i].transform.rotation) as GameObject;
            temp.GetComponent<Platformer2DUserControl>().PlayerID = p;
            SetAbility(temp, pd);
            m_Players.Add(temp);
            i++;
            p++;
        }
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
