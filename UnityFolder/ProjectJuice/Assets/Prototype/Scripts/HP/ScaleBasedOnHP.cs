using System;
using UnityEngine;
using System.Collections;

public class ScaleBasedOnHP : MonoBehaviour
{
    public GameObject _gameObjectToScale;
    public HPScript _hpScript;
    private Transform _originalTransform;
    //private SpriteRenderer m_Sprite;
    
    void Awake()
    {
        //m_Sprite = GetComponent<SpriteRenderer>();
    }

	// Use this for initialization
	void Start ()
    {
        if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        _hpScript.HpChanged += HpScriptOnHpChanged;
        //_gameObjectToScale.transform.localScale = _gameObjectToScale.transform.localScale.SetX(1);
        _originalTransform = _gameObjectToScale.transform;
	}


    private void HpScriptOnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        float newScalePercent = _originalTransform.localScale.x * (hpChangedEventArgs.NewHp / _hpScript.MaxHp);
        _gameObjectToScale.transform.localScale =
            _gameObjectToScale.transform.localScale.SetX(newScalePercent);

    }

    // Update is called once per frame
	void Update () {
	
	}
}
