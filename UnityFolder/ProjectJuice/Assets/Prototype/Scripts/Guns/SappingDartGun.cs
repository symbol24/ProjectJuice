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
    [SerializeField] private DartChainV2 _dartChainPrefab;
    [SerializeField]
    private float _crossSectionLength;

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
        if (_inputManager.m_Special && _delayManager.m_CanShoot)
        {
            FireDart();
        }
    }
    

    private void FireDart()
    {
        var dartGameObject = (Dart)Instantiate(_dartPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        var dart = dartGameObject.GetComponent<Dart>();
        #region SetProperties
        dart.IsContiniousSucking = _isContiniousSucking;
        dart.SuckingInterval = _suckingInterval;
        dart.HpSuckedPerSecond = _hpToSuckPerSecond;
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
            StartCoroutine(_addCrossSection);
        }
    }

    private DartChainV2 InstantiateChain(DartChainV2 toAttachTo, Dart dart = null)
    {
        var brandNewCrossSection =
            (DartChainV2)
                Instantiate(_dartChainPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        brandNewCrossSection.CurrentGun = this;
        brandNewCrossSection.CurrentDart = dart ?? toAttachTo.CurrentDart;
        brandNewCrossSection.NextChain = toAttachTo;
        return brandNewCrossSection;
    }

}
