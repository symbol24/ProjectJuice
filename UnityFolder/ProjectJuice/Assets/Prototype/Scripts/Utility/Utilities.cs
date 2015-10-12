using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{

    public static PlayerData GetPlayerData(PlayerIDs playerID)
    {

        PlayerData[] list = MonoBehaviour.FindObjectsOfType<PlayerData>();
        PlayerData temp = null;

        foreach (PlayerData pd in list)
        {
            if (playerID == pd.playerID)
                temp = pd;
        }

        return temp;
    }

    public static List<PlayerData> GetAllPlayerData()
    {
        List<PlayerData> ListOfPlayers = new List<PlayerData>();
        PlayerData[] list = MonoBehaviour.FindObjectsOfType<PlayerData>();
        foreach (PlayerData pd in list)
        {
            ListOfPlayers.Add(pd);
        }

        return ListOfPlayers;
    }
}