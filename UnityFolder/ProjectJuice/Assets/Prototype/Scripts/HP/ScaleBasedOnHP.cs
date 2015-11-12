using System;
using UnityEngine;
using System.Collections;

public class ScaleBasedOnHP : MonoBehaviour
{
    public GameObject _gameObjectToScale;
    public HPScript _hpScript;
    protected float _originalX;
    private SpriteRenderer _sprite;
    [SerializeField] private bool _isLowHpFlash = true;
    [SerializeField] private Color _lowHpColor;
    [Range(0, 100)][SerializeField] private float _lowHpValue;
    [Range(0, 5)][SerializeField] private float _fadeTime = 0.01f;
    private Color _originalColor;
    private Color _currentColor;
    private Color _targetColor;
    private bool _isFlashing = false;
    private ChangeToPlayerColor _changer;
    
    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

	// Use this for initialization
	void Start ()
    {
        if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        _hpScript.HpChanged += HpScriptOnHpChanged;
        _originalX = _gameObjectToScale.transform.localScale.x;
        _sprite = _gameObjectToScale.GetComponent<SpriteRenderer>();
        if (_sprite == null) Debug.LogError("ScaleBasedOnHP cannot find the sprite render on " + _gameObjectToScale.name);
        else
        {
            if (_isLowHpFlash)
            {
                _changer = GetComponent<ChangeToPlayerColor>();
                if (_changer == null) Debug.LogError("ScaleBasedOnHP cannot find the ChangeToPlayerColor on " + _gameObjectToScale.name);
                else
                {
                    _originalColor = _changer.Controller.m_PlayerData.PlayerSponsor.SponsorColor;
                    _currentColor = _changer.Controller.m_PlayerData.PlayerSponsor.SponsorColor;
                }
            }
        }
	}


    protected virtual void HpScriptOnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        float newScalePercent = _originalX * (_hpScript.CurrentHp / _hpScript.MaxHp);
        _gameObjectToScale.transform.localScale =
            _gameObjectToScale.transform.localScale.SetX(newScalePercent);

        if (_isLowHpFlash && !_isFlashing && _hpScript.CurrentHp <= _lowHpValue) _isFlashing = true;
        else if (_isLowHpFlash && _isFlashing && _hpScript.CurrentHp > _lowHpValue) _isFlashing = false;
    }

    // Update is called once per frame
	void Update () {
        if (_isLowHpFlash && _isFlashing)
        {
            if(_currentColor == _originalColor)
            {
                _targetColor = _lowHpColor;
            }
            else if(_currentColor == _lowHpColor)
            {
                _targetColor = _originalColor;
            }

            _currentColor = Color.Lerp(_currentColor, _targetColor, _fadeTime);
            _sprite.color = _currentColor;
        }
        else if(_isLowHpFlash && !_isFlashing && _currentColor != _originalColor)
        {
            _currentColor = _originalColor;
            _sprite.color = _currentColor;
        }
	}
}
