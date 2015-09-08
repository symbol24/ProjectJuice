using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    public class Utilities : MonoBehaviour
    {

        public static PlayerData GetPlayerData(PlayerIDs playerID)
        {

            PlayerData[] list = FindObjectsOfType<PlayerData>();
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
            PlayerData[] list = FindObjectsOfType<PlayerData>();
            foreach (PlayerData pd in list)
            {
                ListOfPlayers.Add(pd);
            }

            return ListOfPlayers;
        }
    }
}
