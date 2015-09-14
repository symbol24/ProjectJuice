using UnityEngine;
using System.Collections;
using System;

public class SappingDartGun : ExtendedMonobehaviour {
    [SerializeField]
    private Dart _dartPrefab;
    [SerializeField]
    private GameObject _dartSpawnPoint;
    //[SerializeField]
    //private DartChain _dartChainPrefab;

    HPScript _hpScript;
    [SerializeField] 
    private float _dartSpeed;
    [SerializeField]
    bool _isContiniousSucking = true;
    [SerializeField]
    float _suckingInterval = 0.4f;
    [SerializeField]
    float _hpToSuckPerSecond = 100f;
    [SerializeField]
    float _dartCollTimerDisappear = 0f;

    [SerializeField] private float _lifetimeSinceHit = 1f;

    DelayManager _delayManager;
    IPlatformer2DUserControl _inputManager;

    void Start()
    {
        _delayManager = GetComponent<DelayManager>();
        _inputManager = GetComponent<IPlatformer2DUserControl>();
        _hpScript = GetComponent<HPScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_inputManager.m_Special && _delayManager.m_CanShoot)
        {
            FireDart();
        }
    }

    private void FireDart()
    {
        var dartGameObject = (Dart)Instantiate(_dartPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        var dart = dartGameObject.GetComponent<Dart>();
        dart.IsContiniousSucking = _isContiniousSucking;
        dart.SuckingInterval = _suckingInterval;
        dart.HpSuckedPerSecond = _hpToSuckPerSecond;
        dart.DartCollTimerDisappear = _dartCollTimerDisappear;
        dart.LifetimeSinceHit = _lifetimeSinceHit;

        //SubscribeToEvents
        dart.JuiceSucked += Dart_JuiceSucked;
        dart.DartDestroyed += Dart_DartDestroyed;
        dart.DartCollision += Dart_DartCollision;

        dart.ShootBullet(_dartSpeed);
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
