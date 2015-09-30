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
    [Range(0,5)][SerializeField]
    float _suckingInterval = 0.4f;
    [SerializeField]
    float _hpToSuckPerSecond = 100f;
    [Range(0,5)][SerializeField]
    float _dartCollTimerDisappear = 0f;

    [Range(0,5)][SerializeField] private float _lifetimeSinceHit = 1f;
    [SerializeField] private DartChainV2 _dartChainPrefab;
    [Range(0,5)][SerializeField]
    private float _crossSectionLength = 0.1f;
    [SerializeField] private int _crossSectionsLimit = 100;

    public float HoseCrossSectionLength { get { return _crossSectionLength; } }

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
        if (GameManager.instance.IsPlaying)
        {
            if (_inputManager.m_Special && _delayManager.m_CanShoot)
            {
                FireDart();
            }
        }
    }
    

    private void FireDart()
    {
        var dartGameObject = (Dart)Instantiate(_dartPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
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
        _currentCount = 0;
        _addCrossSection = AddCrossSection(currentCrossSection);
        StartCoroutine(_addCrossSection);

        
        //Debug.Break();
        dart.ShootBullet(_dartSpeed, _dartSpawnPoint.transform);
        _delayManager.AddDelay(float.MaxValue);
    }

    private void Dart_DartCollision(object sender, EventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void Dart_DartDestroyed(object sender, EventArgs e)
    {
        StopCoroutine(_addCrossSection);
        if(_moveLastChainToRefPoint != null) StopCoroutine(_moveLastChainToRefPoint);
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


    private int _currentCount;
    private IEnumerator _addCrossSection;
    private IEnumerator AddCrossSection(DartChainV2 currentFirstChain)
    {
        //if (_inputManager.m_SpecialStay)
        {
            float distance = 0f;
            while (distance <= _crossSectionLength)
            {
                distance = Vector3.Distance(currentFirstChain.transform.position, _dartSpawnPoint.transform.position);
                yield return null;
            }
            var crossSectionsNeeded = distance / _crossSectionLength;
            DartChainV2 lastDartChainAdded = currentFirstChain;
            int numberOfIterations = Mathf.RoundToInt(crossSectionsNeeded);
            for (int i = 0; i < numberOfIterations; i++)
            {
                lastDartChainAdded = InstantiateChain(lastDartChainAdded);
            }

            _addCrossSection = AddCrossSection(lastDartChainAdded);
            _currentCount ++;
            if(_currentCount < _crossSectionsLimit) StartCoroutine(_addCrossSection);
            else
            {
                lastDartChainAdded.transform.parent = _dartSpawnPoint.transform;
                lastDartChainAdded.MainRigidbody.isKinematic = true;
                _moveLastChainToRefPoint = MoveLastChainToRefPoint(lastDartChainAdded);
                StartCoroutine(_moveLastChainToRefPoint);
            }
        }
    }
    private IEnumerator _moveLastChainToRefPoint;
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
        dart.ListenToCrossSection(brandNewCrossSection);
        return brandNewCrossSection;
    }

}
