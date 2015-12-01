using System;
using UnityEngine;
using System.Collections;

public abstract class DartGunBase : ExtendedMonobehaviour, IDartGun
{
    //Generic Stuff used
    protected DelayManager _delayManager;
    protected IPlatformer2DUserControl _inputManager;
    protected HPScript _hpScript;
    protected Feedback _feedback;

    //AudioWhenTransfering
    private AudioSource m_SappingAudioSource;
    private AudioSource m_FireSound;

    //GenericProperties
    public GameObject _dartSpawnPoint;
    public GameObject DartSpawnPoint { get { return _dartSpawnPoint; } }
    public GameObject _particlesSpawningPoint;
    public GameObject ParticlesSpawningPoint
    {
        get
        {
            var ret = _particlesSpawningPoint ?? _dartSpawnPoint;
            return ret;
        }
    }
    

    // Use this for initialization
    protected virtual void Start()
    {
        _delayManager = GetComponent<DelayManager>();
        _inputManager = GetComponent<IPlatformer2DUserControl>();
        _hpScript = GetComponent<HPScript>();
        _hpScript.Dead += _hpScript_Dead;
        DartFired += OnDartFired;
        _feedback = GetComponent<Feedback>();
    }

    private void OnDartFired(object sender, DartFiredEventArgs dartFiredEventArgs)
    {
        dartFiredEventArgs.Dart.JuiceSucked += DartOnJuiceSucked;
        dartFiredEventArgs.Dart.DartDestroyed += DartOnDartDestroyed;
//        Debug.Log("OnDartFired");
        m_FireSound = SoundManager.PlaySFX(Fire);        
    }
    private void DartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
        _delayManager.SetDelay(0);
        _delayManager.AddDelay(Settings._shootDelay);
        CoolDownEffects();
        if (m_SappingAudioSource != null && m_SappingAudioSource.isPlaying)
        {
            m_SappingAudioSource.Stop();
        }
        if (m_FireSound.isPlaying) m_FireSound.Stop();
        SoundManager.PlaySFX(WireSnapping);
    }
    private void DartOnJuiceSucked(object sender, JuiceSuckedEventArgs juiceSuckedEventArgs)
    {
        _hpScript.CurrentHp += juiceSuckedEventArgs.HpSucked;
        if (_delayManager.OtherReady)
        {
            m_SappingAudioSource = PlayNewSound(m_SappingAudioSource, Transfering);
            //SoundManager.PlaySFX(Transfering);
            _delayManager.AddOtherDelay(Settings.m_transferSoundDelay);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.instance.IsPlaying)
        {
            if (_inputManager.m_Special && _delayManager.CanShoot)
            {
                _delayManager.AddDelay(9999);
                FireDart();
            }
        }
    }

    protected abstract void FireDart();

    public abstract DartGunSettings Settings { get; set; }
    [HideInInspector]
    public string _fire;
    public string Fire { get { return _fire; } set { _fire = value; } }
    [HideInInspector]
    public string _transfering;
    public string Transfering { get { return _transfering; } set { _transfering = value; } }
    [HideInInspector]
    public string _cooldown;
    public string CoolDown { get { return _cooldown; } set { _cooldown = value; } }
    [HideInInspector]
    public string _wireSnapping;
    public string WireSnapping { get { return _wireSnapping; } set { _wireSnapping = value; } }

    [HideInInspector]
    public ParticleSystem _m_firingParticle;
    public ParticleSystem m_firingParticle { get { return _m_firingParticle; } set { _m_firingParticle = value; } }
    public event EventHandler<DartFiredEventArgs> DartFired;

    protected virtual void OnDartFired(DartFiredEventArgs e)
    {
        try
        {
            EventHandler<DartFiredEventArgs> handler = DartFired;
            if (handler != null) handler(this, e);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }
    }



    AudioSource _cooldownSound;
    private void CoolDownEffects()
    {
        _delayManager.SetDelay(Settings._shootDelay);
        _feedback.SetBool();
        _cooldownSound = SoundManager.PlaySFX(CoolDown, Settings.LoopCooldown);
        _feedback.CanShootFeedbackEvent += _feedback_CanShootFeedbackEvent;
    }

    private void _feedback_CanShootFeedbackEvent(object sender, EventArgs e)
    {
        if (_cooldownSound != null)
        {
            _cooldownSound.Stop();
        }
        _feedback.CanShootFeedbackEvent -= _feedback_CanShootFeedbackEvent;
    }


    private void _hpScript_Dead(object sender, EventArgs e)
    {
        if (m_SappingAudioSource != null && m_SappingAudioSource.isPlaying)
        {
            m_SappingAudioSource.Stop();
        }
        if (_cooldownSound != null && _cooldownSound.isPlaying)
        {
            _cooldownSound.Stop();
        }
    }
}