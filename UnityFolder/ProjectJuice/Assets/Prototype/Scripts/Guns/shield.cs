using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shield : Gun {
    private bool m_CanShootBack { get { return m_CurrentCount >= m_MaxBullets; } }
    [Range(0,25)][SerializeField] private float m_ShotAngle = 10f;
    [Range(0,1)][SerializeField] private float m_ShotModifier = 0.9f;
    [Range(0,24)][SerializeField] private int m_MaxBullets = 9;
    [Range(0,3)][SerializeField] private float m_DelayToActivate = 0.25f;
    [Range(0,10)][SerializeField] private float m_DelayToShutOff = 3f;
    private int m_CurrentCount = 0;
    private bool m_FacingRight = true;
    private bool m_IsActive = false;
    public bool IsShieldActive { get { return m_IsActive && m_Gun.activeInHierarchy; } }
    //[Range(0,5)][SerializeField] private float m_ActiveTime = 1.0f;
    [SerializeField] private Light m_Light;


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
            if (m_DelayManager.m_CanShield && m_Controller.m_SpecialStay)
                ActivateShield();

            if (m_IsActive && !m_Controller.m_SpecialStay)
                DeactivateShield();

            //if (m_Controller.m_FacingRight != m_FacingRight) FlipPosition();

            CheckLight();
        }
	}

    public void RoutedTriggerEnter(Collider2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        MeleeDamagingCollider melee = collision.gameObject.GetComponent<MeleeDamagingCollider>();
        if (bullet != null)
        {
            bullet.Consumed();
            if (m_CurrentCount < m_MaxBullets) {
                m_CurrentCount++;
           }
        }

        if(melee != null)
        {
            print("melee hit detected");
            m_CurrentCount += melee.BulletsToGiveShield;
            melee.Consumed();
        }
    }

    private void CheckLight()
    {
        if (m_CanShootBack)
        {
            if (!m_Light.enabled)
                m_Light.enabled = true;
        }
        else
        {
            if(m_Light.enabled)
               m_Light.enabled = false;
        }
    }

    private void ActivateShield()
    {
        if (!m_IsActive && m_DelayManager.m_OtherReady)
        {
            m_DelayManager.AddOtherDelay(m_DelayToActivate);
            m_DelayManager.AddShieldOffDelay(float.MaxValue);
            m_DelayManager.AddDelay(float.MaxValue);
            m_IsActive = true;
        }
        else if (m_IsActive && m_DelayManager.m_TurnOffShield)
        {
            DeactivateShield();
        }
        else if (m_IsActive && !m_DelayManager.m_TurnOffShield && m_DelayManager.m_OtherReady)
        {
            DisplayShield();
        }
    }

    private void DisplayShield()
    {
        if (!m_Gun.activeInHierarchy)
        {
            m_Gun.SetActive(true);
            if (m_CanShootBack) Fire();
            m_DelayManager.SetShieldOffDelay(m_DelayToShutOff);
        }
    }

    private void DeactivateShield()
    {
        m_DelayManager.Reset();
        m_DelayManager.AddShieldDelay(m_Delay);
        m_DelayManager.AddDelay(m_Delay);
        m_IsActive = false;
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
