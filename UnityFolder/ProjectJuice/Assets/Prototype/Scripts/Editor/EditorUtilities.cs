using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public static class EditorUtilities {

    public static string[] GetSceneNames()
    {
        string[] ret = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            string temp = EditorBuildSettings.scenes[i].path.Substring(EditorBuildSettings.scenes[i].path.LastIndexOf('/') + 1);
            temp = temp.Substring(0, temp.Length - 6);
            ret[i] = temp;
        }
        //Array.Sort(ret, StringComparer.InvariantCulture);
        return ret;
    }

    public static string[] GetListOfAudioClips()
    {

        List<AudioClip> clips = SoundManager.Instance.storedSFXs;
        string[] ret = null;

        if (clips != null)
        {
            ret = new string[clips.Count];

            for (int i = 0; i < clips.Count; i++)
            {
                ret[i] = clips[i].name;
            }
        }
        //Array.Sort(ret, StringComparer.InvariantCulture);
        return ret;
    }

    public static int GetSelectedClip(string[] clipList, string clip)
    {
        int ret = 0;

        for(int i = 0; i < clipList.Length; i++)
        {
            if (clipList[i] == clip) ret = i;
        }

        return ret;
    }

    public static string[] ConvertToArray(List<string> listofstring)
    {
        string[] ret = new string[listofstring.Count];

        for (int i = 0; i < listofstring.Count; i++)
            ret[i] = listofstring[i];

        return ret;
    }

    public static string[] GetListOfSoundConnections()
    {
        string[] ret = new string[SoundManager.Instance.soundConnections.Count];

        List<SoundConnection> temp = SoundManager.Instance.soundConnections;

        for(int i = 0; i < ret.Length; i++)
        {
            ret[i] = temp[i].ToString();
        }
        //Array.Sort(ret, StringComparer.InvariantCulture);
        return ret;
    }

    public static string[] GetListOfParticles()
    {
        string[] ret = new string[Database.instance.Particles.Length];

        for(int i = 0; i < ret.Length; i++)
        {
            ret[i] = Database.instance.Particles[i].name;
        }
        //Array.Sort(ret, StringComparer.InvariantCulture);
        return ret;
    }

    public static int GetSelectedParticle(ParticleSystem toCheck)
    {
        int ret = 0;

        if (toCheck != null)
        {
            for (int i = 0; i < Database.instance.Particles.Length; i++)
            {
                if (toCheck.name == Database.instance.Particles[i].name) ret = i;
            }
        }

        return ret;
    }
}
