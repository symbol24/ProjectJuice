using System;
using UnityEngine;
using System.Collections;

public abstract class DartBase : ExtendedMonobehaviour, IDart
{

    private Rigidbody2D _mainRigidbody;
    [HideInInspector] public DartGunSettings _settings;
    protected IPlatformer2DUserControl _inputManager;
    protected DelayManager _delayManager;

    protected Rigidbody2D MainRigidbody2D
    {
        get
        {
            if (_mainRigidbody == null)
            {
                _mainRigidbody = GetComponent<Rigidbody2D>();
            }
            return _mainRigidbody;
        }
    }

    public abstract HPScript Target { get; }

    //FX
    [HideInInspector]
    public string PlayerImpact;
    [HideInInspector]
    public string GroundImpact;
    [HideInInspector]
    public string Severed;
    
    #region Events
    public event EventHandler DartCollision;
    protected virtual void OnDartCollision()
    {
        EventHandler handler = DartCollision;
        if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<JuiceSuckedEventArgs> JuiceSucked;
    protected virtual void OnJuiceSucked(JuiceSuckedEventArgs e)
    {
        EventHandler<JuiceSuckedEventArgs> handler = JuiceSucked;
        if (handler != null) handler(this, e);
    }

    public virtual event EventHandler DartDestroyed;
    private bool _dartDestroyedCalled = false;
    protected virtual void OnDartDestroyed()
    {
        //Debug.Log("OnDartDestroyed");
        if (!_dartDestroyedCalled)
        {
//            Debug.Log("DestroyingDart");
            _dartDestroyedCalled = true;
            EventHandler handler = DartDestroyed;
            if (handler != null) handler(this, EventArgs.Empty);
            //StartCoroutine(DestroyNextFrame());
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyNextFrame()
    {
        yield return null;
        Destroy(gameObject);
    }
    #endregion

    // Use this for initialization
    void Start () {

        if (_settings == null) StartCoroutine(InitializeNextFrame());
        else Initialize();
        
	}
    private IEnumerator InitializeNextFrame()
    {
        yield return null;
        Initialize();
    }
    protected virtual void Initialize()
    {
        var triggerToUse = gameObject.AddComponent<TriggerOnDistance>();
        triggerToUse.DistanceTravelled += Dart_DistanceTravelled;
        triggerToUse._maxDistance = _settings.DartMaxDistanceTravelled;
    }

    protected virtual void Dart_DistanceTravelled(object sender, EventArgs e)
    {
        OnDartDestroyed();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PushConfig(DartGunSettings settings, IPlatformer2DUserControl inputManager, DelayManager delayManager)
    {
        _settings = settings;
        _inputManager = inputManager;
        _delayManager = delayManager;
    }

    public void ShootBullet(float force, Transform sourceTransform)
    {
        if (force < 1f) force = 1;
        MainRigidbody2D.AddForce(transform.up * force);
    }
    
}
