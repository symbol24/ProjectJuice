﻿using UnityEngine;
using System.Collections;

public class StraightCameraShakeButton : MonoBehaviour
{
    public CameraShakeSettings settings;
    private CameraShaker _shaker;
    [SerializeField] private KeyCode _buttonForShake = KeyCode.A;
	// Use this for initialization
	void Start ()
	{
	    _shaker = FindObjectOfType<CameraShaker>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(_buttonForShake))
	    {
	        if(_shaker != null) _shaker.DoShake(settings);
	    }
	}
}
