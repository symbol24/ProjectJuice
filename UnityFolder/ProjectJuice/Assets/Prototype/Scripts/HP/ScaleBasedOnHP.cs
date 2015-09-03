﻿using System;
using UnityEngine;
using System.Collections;

public class ScaleBasedOnHP : MonoBehaviour
{
    public GameObject _gameObjectToScale;
    public HPScript _hpScript;

	// Use this for initialization
	void Start ()
	{
	    if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        _hpScript.HpChanged += HpScriptOnHpChanged;
	    _gameObjectToScale.transform.localScale = _gameObjectToScale.transform.localScale.SetX(1);
	}

    private void HpScriptOnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        _gameObjectToScale.transform.localScale =
            _gameObjectToScale.transform.localScale.SetX(hpChangedEventArgs.NewHp/_hpScript.MaxHp);

    }

    // Update is called once per frame
	void Update () {
	
	}
}