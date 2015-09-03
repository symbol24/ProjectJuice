﻿using UnityEngine;
using System.Collections;

public class LayerChecker : MonoBehaviour {

    public static bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((mask.value & (1 << obj.layer)) > 0);
    }
}