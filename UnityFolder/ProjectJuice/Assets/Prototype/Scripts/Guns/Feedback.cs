using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Feedback : MonoBehaviour {
    private DelayManager _delay;
    private bool _canShootSent = true;
    private bool _canShieldSent = true;
    [SerializeField] private List<SpriteRenderer> _toFlash;
    [Range(0,5)][SerializeField] private int _amountOfFlashes = 2;
    [Range(0,1)][SerializeField] private float _flashDelay = 0.2f;
    [SerializeField] private bool _useNewColor = true;
    [SerializeField] private Color _newColor;

	// Use this for initialization
	void Start () {
        _delay = GetComponent<DelayManager>();
        if (_delay == null) Debug.LogError("Feedback manager cannot find the delay manager on " + gameObject.name);

        if (_toFlash.Count > 0)
        {
            if (_useNewColor)
                ColorChange();

            if(_toFlash[0].enabled == true)
                SwitchEnables();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!_canShootSent && _delay.CanShoot)
            _canShootSent = OnCanShootFeedbackEvent();

        if (!_canShieldSent && _delay.CanShield)
            _canShieldSent = OnCanShootFeedbackEvent();
    }

    public event EventHandler CanShootFeedbackEvent;

    protected virtual bool OnCanShootFeedbackEvent()
    {
        StartCoroutine(FlashPlayer());
        EventHandler handler = CanShootFeedbackEvent;
        if (handler != null) handler(this, EventArgs.Empty);
        return true;
    }

    /*
    public event EventHandler CanShieldFeedbackEvent;

    protected virtual bool OnCanShieldFeedbackEvent()
    {
        EventHandler handler = CanShootFeedbackEvent;
        if (handler != null) handler(this, EventArgs.Empty);
        return true;
    }
    */

    public void SetBool(int toChange = 0, bool newValue = false)
    {
        switch (toChange)
        {
            case 0:
                _canShootSent = newValue;
                break;
            case 1:
                _canShieldSent = newValue;
                break;
        }
    }

    private IEnumerator FlashPlayer()
    {
        for(int i = 0; i < _amountOfFlashes; i++)
        {
            SwitchEnables();
            yield return new WaitForSeconds(_flashDelay); //on
            SwitchEnables();
            yield return new WaitForSeconds(_flashDelay); //off
            SwitchEnables();
        }
    }

    private void SwitchEnables()
    {
        foreach (SpriteRenderer SP in _toFlash)
            SP.enabled = !SP.enabled;
    }

    private void ColorChange()
    {
        foreach (SpriteRenderer SP in _toFlash)
            SP.color = _newColor;
    }
}
