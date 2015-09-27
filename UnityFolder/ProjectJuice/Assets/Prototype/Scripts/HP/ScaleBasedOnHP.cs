using System;
using UnityEngine;
using System.Collections;

public class ScaleBasedOnHP : MonoBehaviour
{
    public GameObject _gameObjectToScale;
    public HPScript _hpScript;
    private SpriteRenderer m_Sprite;
    
    void Awake()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
        PlayerSpawner.instance.ChangeColor += Scaler_ChangeColor;
    }

	// Use this for initialization
	void Start ()
    {
        if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        _hpScript.HpChanged += HpScriptOnHpChanged;
        _gameObjectToScale.transform.localScale = _gameObjectToScale.transform.localScale.SetX(1);
	}

    private void Scaler_ChangeColor(object sender, PlayerColorEventArgs e)
    {
        if(_hpScript.inputController.m_PlayerData.playerID == e.player.playerID)
            SetColor(e.player.PlayerSponsor.SponsorColor);
    }

    private void HpScriptOnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        _gameObjectToScale.transform.localScale =
            _gameObjectToScale.transform.localScale.SetX(hpChangedEventArgs.NewHp/_hpScript.MaxHp);

    }

    // Update is called once per frame
	void Update () {
	
	}

    private void SetColor(Color color)
    {
        m_Sprite.color = color;
    }
}
