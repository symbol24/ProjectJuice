using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

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
        return ret;
    }

    public static string[] GetListOfAudioClipsFromGroup(string groupName)
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

        return ret;
    }
}
