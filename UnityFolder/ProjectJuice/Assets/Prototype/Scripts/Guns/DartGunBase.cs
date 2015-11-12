using System;
using UnityEngine;
using System.Collections;

public abstract class DartGunBase : ExtendedMonobehaviour, IDartGun
{
    //Generic Stuff used
    protected DelayManager _delayManager;
    protected IPlatformer2DUserControl _inputManager;
    protected HPScript _hpScript;
    protected LightFeedbackTemp _lightFeedback;

    //AudioWhenTransfering
    private AudioSource m_SappingAudioSource;

    //GenericProperties
    public GameObject _dartSpawnPoint;
    public GameObject DartSpawnPoint { get { return _dartSpawnPoint; } }

    // Use this for initialization
    protected virtual void Start()
    {
        _delayManager = GetComponent<DelayManager>();
        _inputManager = GetComponent<IPlatformer2DUserControl>();
        _hpScript = GetComponent<HPScript>();
        _lightFeedback = GetComponent<LightFeedbackTemp>();
        DartFired += OnDartFired;
    }

    private void OnDartFired(object sender, DartFiredEventArgs dartFiredEventArgs)
    {
        dartFiredEventArgs.Dart.JuiceSucked += DartOnJuiceSucked;
        dartFiredEventArgs.Dart.DartDestroyed += DartOnDartDestroyed;
    }
    private void DartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
        _delayManager.SetDelay(0);
        _delayManager.AddDelay(Settings._shootDelay);
        CoolDownEffects();
        if (m_SappingAudioSource != null && m_SappingAudioSource.isPlaying) m_SappingAudioSource.Stop();
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
    public string Fire { get; set; }
    public string Transfering { get; set; }
    public string CoolDown { get; set; }
    public ParticleSystem m_firingParticle { get; set; }
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



    private void CoolDownEffects()
    {
        SoundManager.PlaySFX(CoolDown);
        if (_lightFeedback != null) _lightFeedback.StartLightFeedback(Settings._shootDelay);
        else Debug.LogWarning("No LightFeedBack");
    }

}