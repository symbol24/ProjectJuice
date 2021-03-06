﻿using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public static List<PlayerData> GetAllPlayerData(bool randomize = false)
    {
        List<PlayerData> ListOfPlayers = new List<PlayerData>();
        PlayerData[] list = MonoBehaviour.FindObjectsOfType<PlayerData>();

        if (randomize)
        {
            PlayerIDs pi = (PlayerIDs)UnityEngine.Random.Range(0, 3);
            
            while (ListOfPlayers.Count != list.Length)
            {
                foreach (var pd in list)
                {

                    if (!ListOfPlayers.Contains(pd) && pi == pd.playerID)
                        ListOfPlayers.Add(pd);
                     
                }
                pi = (PlayerIDs)UnityEngine.Random.Range(0, 3);
            }
        }
        else
        {
            PlayerIDs pi = PlayerIDs.A;
            while(ListOfPlayers.Count != list.Length) { 
                foreach (var pd in list)
                {
                    if (pi == pd.playerID) ListOfPlayers.Add(pd);
                }
                pi++;
            }
        }
        return ListOfPlayers;
    }

    public static List<T> RandomizeList<T>(List<T> toRandom)
    {
        List<T> newList = toRandom.OrderBy(c => Guid.NewGuid()).ToList();

        return newList;
    }

    public static void Log(this Exception ex, string comment = "")
    {
        if (Debug.isDebugBuild)
        {
            if (ex != null)
            {
                using (var fs = File.AppendText(Path.Combine(Environment.CurrentDirectory, "Exceptions.log")))
                {
                    fs.WriteLine(ex.GetType().Name + " occurred at " + DateTime.Now.ToString() + ":");
                    if(string.IsNullOrEmpty(comment)) fs.WriteLine(comment);
                    fs.WriteLine("Message: " + ex.Message);
                    fs.WriteLine("Stack Trace:");
                    fs.WriteLine(ex.StackTrace);
                    fs.WriteLine("-----------");
                }
            }
        }
    }

}