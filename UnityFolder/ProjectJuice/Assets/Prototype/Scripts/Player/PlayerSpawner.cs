using UnityEngine;
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
            m_Players.Add(temp);
            i++;
            p++;
        }
    }
}
