using UnityEngine;
using System.Collections;
using System;

public class SappingDartGun : ExtendedMonobehaviour {

    [SerializeField]
    HPScript _hpScript;
    [SerializeField]
    GameObject _dartPrefab;
    [SerializeField]
    GameObject _dartSpawnPoint;
    [SerializeField]
    float _dartSpeed = 10;
    [SerializeField]
    bool _isContiniousSucking = true;
    [SerializeField]
    float _suckingInterval = 0.4f;
    [SerializeField]
    float _hpSuckedPerSecond = 100f;
    [SerializeField]
    float _dartCollTimerDisappear = 0f;

    DelayManager _delayManager;
    IPlatformer2DUserControl _inputManager;

    // Use this for initialization
    void Start()
    {
        if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        if (_hpScript == null) _hpScript = GetComponentInParent<HPScript>();
        if (_hpScript == null) Debug.LogError("No HPScriptFound");
        if (_inputManager == null) _inputManager = GetComponent<IPlatformer2DUserControl>();
        if (_inputManager == null) _inputManager = GetComponentInParent<IPlatformer2DUserControl>();
        if (_inputManager == null) Debug.LogError("No IPlatformer2DUserControl Found");
        if (_delayManager == null) _delayManager = GetComponent<DelayManager>();
        if (_delayManager == null) _delayManager = GetComponentInParent<DelayManager>();
        if (_delayManager == null) Debug.LogError("NoDelayManagerFound");

    }

    // Update is called once per frame
    void Update()
    {
        if (_inputManager.m_Special)
        {
            FireDart(GetRotation(_inputManager));
        }
    }

    private void FireDart(Vector2 _direction)
    {
        var dartGameObject = (GameObject)Instantiate(_dartPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        var dart = dartGameObject.GetComponent<Dart>();
        dart.Speed = _dartSpeed;
        dart.Angle = _direction;
        dart.IsContiniousSucking = _isContiniousSucking;
        dart.SuckingInterval = _suckingInterval;
        dart.HpSuckedPerSecond = _hpSuckedPerSecond;
        dart.DartCollTimerDisappear = _dartCollTimerDisappear;

        dart.JuiceSucked += Dart_JuiceSucked;
        dart.DartDestroyed += Dart_DartDestroyed;
        dart.DartCollision += Dart_DartCollision;
        _delayManager.AddDelay(float.MaxValue);
    }

    private void Dart_DartCollision(object sender, EventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void Dart_DartDestroyed(object sender, EventArgs e)
    {
        _delayManager.Reset();
    }

    private void Dart_JuiceSucked(object sender, JuiceSuckedEventArgs e)
    {
        _hpScript.CurrentHp += e.HpSucked;
    }

    IEnumerator AddHpOnTimer(HPScript hpScript, float hpToAdd, float timer)
    {
        yield return new WaitForSeconds(timer);
        hpScript.CurrentHp += hpToAdd;
    }

}
