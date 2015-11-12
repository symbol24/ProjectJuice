using UnityEngine;
using System.Collections;
using System;

public class SappingDartGun : DartGunBase
{
    [SerializeField] private Dart _dartPrefabPrefab;

    [SerializeField] private float _dartSpeed;
    [SerializeField] private bool _isContiniousSucking = true;
    [Range(0, 5)] [SerializeField] private float _suckingInterval = 0.4f;
    [Range(0, 20)] [SerializeField] private float _dartCollTimerDisappear = 0f;

    [Range(0, 5)] [SerializeField] private float _lifetimeSinceHit = 1f;
    [SerializeField] private DartChainV2 _dartChainPrefab;
    [SerializeField] private DartChainV2 _dartChainStatic;
    [Range(0, 5)] [SerializeField] private float _crossSectionLength = 0.1f;
    [Range(0, 5)] [SerializeField] private float _crossSectionTolerance = 0.3f;
    [SerializeField] private int _crossSectionsLimit = 100;

    public float HoseCrossSectionLength
    {
        get { return _crossSectionLength; }
    }

    [SerializeField] private bool _enabledCrossSectionTolerance = false;

    public float HoseCrossSectionLengthTolerance
    {
        get { return _enabledCrossSectionTolerance ? _crossSectionTolerance : float.MaxValue; }
    }


    [Range(0, 5)] [SerializeField] private float m_ParticleLifetime = 0.3f;



    protected override void FireDart()
    {
        var dartGameObject =
            (Dart) Instantiate(_dartPrefabPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        var dart = dartGameObject.GetComponent<Dart>();

        #region SetProperties

        dart.IsContiniousSucking = _isContiniousSucking;
        dart.SuckingInterval = _suckingInterval;
        dart.HpSuckedPerSecond = Database.instance.DartBaseDmgPerSecond;
        dart.DartCollTimerDisappear = _dartCollTimerDisappear;
        dart.LifetimeSinceHit = _lifetimeSinceHit;
        dart.InputManager = _inputManager;
        dart.SourceHPScript = _hpScript;

        #endregion SetProperties

        #region SubscribeToEvents

        dart.JuiceSucked += Dart_JuiceSucked;
        dart.DartDestroyed += Dart_DartDestroyed;
        dart.DartCollision += Dart_DartCollision;

        #endregion SubscribeToEvents



        var currentCrossSection = InstantiateChain(dart.StaticCrossSection, dart);
        OnDartFired(new DartFiredEventArgs {Dart = dart,});
        currentCrossSection.IgnoreFloor = true;
        _currentCount = 0;
        _addCrossSection = AddCrossSection(currentCrossSection);
        StartCoroutine(_addCrossSection);


        //Debug.Break();
        dart.ShootBullet(_dartSpeed, _dartSpawnPoint.transform);
        _delayManager.AddDelay(float.MaxValue);
        SoundManager.PlaySFX(Fire);
        InstatiateParticle(m_firingParticle, _dartChainStatic.gameObject, true, m_ParticleLifetime);
    }

    public override DartGunSettings Settings
    {
        get
        {
            _settings = new DartGunSettings
            {
                HoseLength = _crossSectionLength,
                HoseLengthTolerance = _crossSectionTolerance
            };
            return _settings;
        }
        set { _settings = value; }
    }

    private void Dart_DartCollision(object sender, EventArgs e)
    {
        StopCoroutine(_addCrossSection);
        StartCoroutine(LastCoroutine());
        var lastDartChainCandidate = lastDartChainAdded;
        if (lastDartChainCandidate != null)
        {
            if (_dartChainStatic == null)
            {
                lastDartChainCandidate.transform.position = transform.position;
                lastDartChainCandidate.transform.parent = transform;
            }
            else
            {
                lastDartChainCandidate.transform.position = _dartChainStatic.transform.position;
                lastDartChainCandidate.transform.parent = _dartChainStatic.transform;
                lastDartChainCandidate.PreviousChain = _dartChainStatic;
            }
            lastDartChainCandidate.IgnoreFloor = true;
        }
    }

    private IEnumerator LastCoroutine()
    {
        yield return null;
    }

    private void Dart_DartDestroyed(object sender, EventArgs e)
    {
        StopCoroutine(_addCrossSection);
        if (_moveLastChainToRefPoint != null) StopCoroutine(_moveLastChainToRefPoint);
    }

    private void Dart_JuiceSucked(object sender, JuiceSuckedEventArgs e)
    {
        _hpScript.CurrentHp += e.HpSucked;
    }

    private IEnumerator AddHpOnTimer(HPScript hpScript, float hpToAdd, float timer)
    {
        yield return new WaitForSeconds(timer);
        SoundManager.PlaySFX(Transfering);
        hpScript.CurrentHp += hpToAdd;
    }


    private int _currentCount;
    private IEnumerator _addCrossSection;
    private DartChainV2 lastDartChainAdded;

    private IEnumerator AddCrossSection(DartChainV2 currentFirstChain)
    {
        float distance = Vector3.Distance(currentFirstChain.transform.position, _dartSpawnPoint.transform.position);
        /*while (distance <= _crossSectionLength)
        {
            distance = Vector3.Distance(currentFirstChain.transform.position, _dartSpawnPoint.transform.position);
            yield return null;
        }*/
        var crossSectionsNeeded = distance/_crossSectionLength;
        lastDartChainAdded = currentFirstChain;
        int numberOfIterations = Mathf.RoundToInt(crossSectionsNeeded);
        for (int i = 0; i < numberOfIterations; i++)
        {
            lastDartChainAdded = InstantiateChain(lastDartChainAdded);
        }

        yield return null;
        _addCrossSection = AddCrossSection(lastDartChainAdded);
        _currentCount ++;
        if (_currentCount < _crossSectionsLimit) StartCoroutine(_addCrossSection);
        else
        {
            lastDartChainAdded.transform.parent = _dartSpawnPoint.transform;
            lastDartChainAdded.MainRigidbody.isKinematic = true;
            _moveLastChainToRefPoint = MoveLastChainToRefPoint(lastDartChainAdded);
            StartCoroutine(_moveLastChainToRefPoint);
        }

    }

    private IEnumerator _moveLastChainToRefPoint;
    private DartGunSettings _settings;


    private IEnumerator MoveLastChainToRefPoint(DartChainV2 lastDartChainAdded)
    {
        while (true)
        {
            lastDartChainAdded.transform.position = _dartSpawnPoint.transform.position;
            yield return null;
        }
    }

    private DartChainV2 InstantiateChain(DartChainV2 toAttachTo, Dart dart = null)
    {
        dart = dart ?? toAttachTo.CurrentDart;
        var brandNewCrossSection =
            (DartChainV2)
                Instantiate(_dartChainPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        brandNewCrossSection.CurrentGun = this;
        brandNewCrossSection.CurrentDart = dart;
        brandNewCrossSection.NextChain = toAttachTo;
        brandNewCrossSection.transform.parent = dart.transform;
        
        dart.ListenToCrossSection(brandNewCrossSection);
        return brandNewCrossSection;
    }

}
