using UnityEngine;
using System.Collections;
using System;

public class Feedback : MonoBehaviour {
    private DelayManager _delay;
    private bool _canShootSent = true;
    private bool _canShieldSent = true;

	// Use this for initialization
	void Start () {
        _delay = GetComponent<DelayManager>();
        if (_delay == null) Debug.LogError("Feedback manager cannot find the delay manager on " + gameObject.name);
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
        Debug.LogWarning("Missing visual feedback on ready to shoot/shield");
        EventHandler handler = CanShootFeedbackEvent;
        if (handler != null) handler(this, EventArgs.Empty);
        return true;
    }

    public event EventHandler CanShieldFeedbackEvent;

    protected virtual bool OnCanShieldFeedbackEvent()
    {
        EventHandler handler = CanShootFeedbackEvent;
        if (handler != null) handler(this, EventArgs.Empty);
        return true;
    }

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
}
