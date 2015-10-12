﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shield : Gun {
    private bool m_CanShootBack { get { return m_CurrentCount >= m_MaxBullets; } }
    [Range(0,25)][SerializeField] private float m_ShotAngle = 10f;
    [Range(0,1)][SerializeField] private float m_ShotModifier = 0.9f;
    [Range(0,24)][SerializeField] private int m_MaxBullets = 9;
    [Range(0,3)][SerializeField] private float m_DelayToActivate = 0.25f;
    [Range(0,10)][SerializeField] private float m_DelayToShutOff = 3f;
    [Range(0, 1)][SerializeField] private float m_FullChargeSoundDelay = 0.3f;
    private int m_CurrentCount = 0;
    private bool m_FacingRight = true;
    private bool m_IsActive = false;
    public bool IsShieldActive { get { return m_IsActive && m_Gun.activeInHierarchy; } }
    //[Range(0,5)][SerializeField] private float m_ActiveTime = 1.0f;
    [SerializeField] private Light m_Light;

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
        m_Gun.SetActive(false);
        m_DelayManager.Reset();
    }

    // Update is called once per frame
    protected override void Update ()
    {
        if (GameManager.instance.IsPlaying)
        {
            if (m_DelayManager.CanShield && m_Controller.m_SpecialStay)
                ActivateShield();

            if (m_IsActive && !m_Controller.m_SpecialStay)
                DeactivateShield();

            if (m_Controller.m_FacingRight != m_FacingRight) FlipPosition();

            CheckLight();
        }
	}

    public void RoutedTriggerEnter(Collider2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        MeleeDamagingCollider melee = collision.gameObject.GetComponent<MeleeDamagingCollider>();
        var explosive = collision.gameObject.GetComponent<IDamaging>();
        if (bullet != null)
        {
            bullet.Consumed();
            if (m_CurrentCount < m_MaxBullets) m_CurrentCount += bullet.BulletsToGiveShield;
            SoundManager.PlaySFX(AbsorbBullet);
        }

        if(melee != null)
        {
            if (m_CurrentCount < m_MaxBullets) m_CurrentCount += melee.BulletsToGiveShield;
            melee.Consumed();
            SoundManager.PlaySFX(melee._meleeAttack.Clash);
        }
        if(explosive != null)
        {
            if (m_CurrentCount < m_MaxBullets) m_CurrentCount += explosive.BulletsToGiveShield;
            SoundManager.PlaySFX(AbrosbExplosion);
        }
    }

    private void CheckLight()
    {
        if (m_CanShootBack)
        {
            if (!m_Light.enabled)
                m_Light.enabled = true;

            if (m_DelayManager.SoundReady)
            {
                SoundManager.PlaySFX(FullCharge);
                m_DelayManager.AddSoundDelay(m_FullChargeSoundDelay);
            }
        }
        else
        {
            if(m_Light.enabled)
               m_Light.enabled = false;
        }
    }

    private void ActivateShield()
    {
        if (!m_IsActive && m_DelayManager.OtherReady)
        {
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
            SoundManager.PlaySFX(Activate);
            m_Gun.SetActive(true);
            if (m_CanShootBack) Fire();
            m_DelayManager.SetShieldOffDelay(m_DelayToShutOff);
            if (!m_HpScp.ShieldImunity) m_HpScp.SwitchShieldImunity();
        }
    }

    private void DeactivateShield()
    {
        if (IsShieldActive)
        {
            SoundManager.PlaySFX(CoolDown);
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
            Quaternion newRot = m_GunRef.transform.rotation;
            newRot = Quaternion.Euler( 0, 0, newRot.eulerAngles.z + Random.Range(-m_ShotAngle, m_ShotAngle));
            FireOneBullet(newRot);
        }
        m_CurrentCount = 0;
        SoundManager.PlaySFX(ShootingBullets);
        SoundManager.PlaySFX(Ricochet);
    }

    public void FireOneBullet(Quaternion newRotation)
    {
        Bullet newBullet = Instantiate(m_Bullet, m_GunRef.transform.position, newRotation) as Bullet;
        newBullet.ShootBullet(m_ShotModifier);
        newBullet.AddImmuneTarget(m_HpScp);
    }

    private void FlipPosition()
    {
        Vector3 thePosition = m_Gun.transform.localPosition;
        thePosition.x *= -1;
        m_Gun.transform.localPosition = thePosition;

        thePosition = m_GunRef.transform.localPosition;
        thePosition.x *= -1;
        m_GunRef.transform.localPosition = thePosition;

        Quaternion theRotation = m_GunRef.transform.localRotation;
        theRotation.z = -theRotation.z;
        m_GunRef.transform.localRotation = theRotation;

        m_FacingRight = !m_FacingRight;
    }
}
