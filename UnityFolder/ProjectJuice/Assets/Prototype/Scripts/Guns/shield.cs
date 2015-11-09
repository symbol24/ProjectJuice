using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class shield : Gun {
    public bool m_CanShootBack { get { return m_CurrentCount >= m_MaxBullets; } }
    [Range(0,25)][SerializeField] private float m_ShotAngle = 10f;
    [Range(0,1)][SerializeField] private float m_ShotModifier = 0.9f;
    [Range(0,24)][SerializeField] private int m_MaxBullets = 9;
    [Range(0,3)][SerializeField] private float m_DelayToActivate = 0.25f;
    [Range(0,10)][SerializeField] private float m_DelayToShutOff = 3f;
    [Range(0, 1)][SerializeField] private float m_FullChargeSoundDelay = 0.3f;
    private int m_CurrentCount = 0;
    private bool m_FacingRight = true;
    private bool m_IsActive = false;
    [SerializeField] private bool m_DebugIsActive = false;
    public bool IsShieldActive { get { return m_IsActive && m_Gun.activeInHierarchy; } }
    private AudioSource m_ActiveDeactiveAudioSource = new AudioSource();
    private AudioSource m_AbsorbAudioSource = new AudioSource();
    private AudioSource m_FullChargeAudioSource = new AudioSource();
    [SerializeField] private Light m_Light;
    [SerializeField]
    private bool m_isDebugFullTest = false;
    private int _meleeColliderLimiter = 0;

    [HideInInspector] public string Activate;
    [HideInInspector] public string AbsorbBullet;
    [HideInInspector] public string ShootingBullets;
    [HideInInspector] public string FullCharge;
    [HideInInspector] public string Ricochet;
    [HideInInspector] public string CoolDown;
    [HideInInspector] public string AbrosbExplosion;

    protected override void Start()
    {
        base.Start();
        m_DelayManager.Reset();
        m_Gun.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update ()
    {
        if (GameManager.instance.IsPlaying)
        {
            if (m_DelayManager.CanShield && (m_Controller.m_SpecialStay || m_DebugIsActive))
                ActivateShield();

            if (m_IsActive && !m_Controller.m_SpecialStay)
                DeactivateShield();

            if (m_Controller.m_FacingRight != m_FacingRight) FlipPosition();

            CheckReload();
            
            if (m_isDebugFullTest && m_CurrentCount == 0 && m_DelayManager.CanShield) m_CurrentCount = 10;
        }
	}

    public event EventHandler BulletAbsorbed;

    protected virtual void OnBulletAbsorbed()
    {
        try
        {
            EventHandler handler = BulletAbsorbed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }
    }

    public void RoutedTriggerEnter(Collider2D collision)
    {
        var damaging = collision.gameObject.GetComponent<IDamaging>();

        if (damaging != null)
        {
            switch (damaging.TypeOfDamage)
            {
                case DamageType.Melee:
                    _meleeColliderLimiter++;
                    if (_meleeColliderLimiter == 1)
                    {
                        AbsordBullets(damaging);
                        StartCoroutine(ResetMeleeLimiter());
                        MeleeDamagingCollider melee = damaging as MeleeDamagingCollider;
                        m_AbsorbAudioSource = PlayNewSound(m_AbsorbAudioSource, melee._meleeAttack.Clash);
                    }
                    break;
                case DamageType.Explosive:
                    AbsordBullets(damaging);
                    m_AbsorbAudioSource = PlayNewSound(m_AbsorbAudioSource, AbrosbExplosion);
                    break;
                default:
                    AbsordBullets(damaging);
                    m_AbsorbAudioSource = PlayNewSound(m_AbsorbAudioSource, AbsorbBullet);
                    break;
            }
            damaging.Consumed();
        }
        
    }

    private void AbsordBullets(IDamaging damaging)
    {
        if (m_CurrentCount < m_MaxBullets) m_CurrentCount += damaging.BulletsToGiveShield;
        OnBulletAbsorbed();
    }

    public IEnumerator ResetMeleeLimiter()
    {
        yield return new WaitForSeconds(1);
        _meleeColliderLimiter = 0;
    }

    private void CheckReload()
    {
        if (m_CanShootBack)
        {
            if (m_DelayManager.SoundReady)
            {
                m_FullChargeAudioSource = PlayNewSound(m_FullChargeAudioSource, FullCharge);
                m_DelayManager.AddSoundDelay(m_FullChargeSoundDelay);
            }
        }
        else
        {
            //I dont remember what this was for damn...
            //if(m_FullChargeAudioSource != null && m_FullChargeAudioSource.isPlaying) m_FullChargeAudioSource.Stop();
        }
    }

    private void ActivateShield()
    {
        if (!m_IsActive && m_DelayManager.OtherReady)
        {
            if (m_CanShootBack) Fire();
            m_DelayManager.AddOtherDelay(m_DelayToActivate);
            m_DelayManager.AddShieldOffDelay(float.MaxValue);
            m_DelayManager.AddDelay(float.MaxValue);
            m_IsActive = true;
        }
        else if (m_IsActive && m_DelayManager.CanTurnOffShield)
        {
            DeactivateShield();
        }
        else if (m_IsActive && !m_DelayManager.CanTurnOffShield && m_DelayManager.OtherReady)
        {
            DisplayShield();
        }
    }

    private void DisplayShield()
    {
        if (!m_Gun.activeInHierarchy)
        {
            m_ActiveDeactiveAudioSource = PlayNewSound(m_ActiveDeactiveAudioSource, Activate);
            m_Gun.SetActive(true);
            m_DelayManager.SetShieldOffDelay(m_DelayToShutOff);
            if (!m_HpScp.ShieldImunity) m_HpScp.SwitchShieldImunity();
        }
    }

    private void DeactivateShield()
    {
        if (IsShieldActive)
        {
            m_ActiveDeactiveAudioSource = PlayNewSound(m_ActiveDeactiveAudioSource, CoolDown);
            m_DelayManager.SetShieldDelay(m_Delay);
            m_DelayManager.SetDelay(m_Delay);
        }
        else
        {
            m_DelayManager.SetShieldDelay(0);
            m_DelayManager.SetDelay(0);
        }
        m_IsActive = false;
        if (m_HpScp.ShieldImunity) m_HpScp.SwitchShieldImunity();
        m_Gun.SetActive(false);
    }

    public override void Fire()
    {
        for(int i = 0; i < m_CurrentCount; i++)
        {
            Quaternion newRot = GunBulletReference.transform.rotation;
            newRot = Quaternion.Euler( 0, 0, newRot.eulerAngles.z + Random.Range(-m_ShotAngle, m_ShotAngle));
            FireOneBullet(newRot);
        }
        OnShieldFired();
        m_CurrentCount = 0;
        ShieldShotFX();
    }

    public event EventHandler ShieldFired;

    protected virtual void OnShieldFired()
    {
        try
        {
            EventHandler handler = ShieldFired;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }
    }

    private void ShieldShotFX()
    {
        DisplayParticles();
        SoundManager.PlaySFX(ShootingBullets);
        SoundManager.PlaySFX(Ricochet);
    }

    public void FireOneBullet(Quaternion newRotation)
    {
        Bullet newBullet = Instantiate(m_Bullet, GunBulletReference.transform.position, newRotation) as Bullet;
        newBullet.ShootBullet(m_ShotModifier);
        newBullet.AddImmuneTarget(m_HpScp);
    }

    private void FlipPosition()
    {
        Vector3 thePosition = m_Gun.transform.localPosition;
        thePosition.x *= -1;
        m_Gun.transform.localPosition = thePosition;

        thePosition = GunBulletReference.transform.localPosition;
        thePosition.x *= -1;
        GunBulletReference.transform.localPosition = thePosition;

        Quaternion theRotation = GunBulletReference.transform.localRotation;
        theRotation.z = -theRotation.z;
        GunBulletReference.transform.localRotation = theRotation;

        m_FacingRight = !m_FacingRight;
    }
}
