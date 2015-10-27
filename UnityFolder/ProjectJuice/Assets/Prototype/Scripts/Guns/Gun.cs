using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;
using System;

public class Gun : ExtendedMonobehaviour {
    public IPlatformer2DUserControl m_Controller { get; private set; }
    public DelayManager m_DelayManager { get; private set; }
    
    [SerializeField] private GameObject m_GunReference; //used for flipping maybe
    public GameObject m_GunRef { get { return m_GunReference; } }
    [SerializeField] private GameObject m_GunObject;
    public GameObject m_Gun { get { return m_GunObject; } }
    [SerializeField] private Bullet m_BulletPrefab;
    public Bullet m_Bullet { get { return m_BulletPrefab; } }
    [Range(0,5)][SerializeField] protected float m_ShotDelay = 0.1f;
    public float m_Delay { get { return m_ShotDelay; } }
    [SerializeField] private HPScript m_HpScript;
    public HPScript m_HpScp { get { return m_HpScript; } }
    protected bool m_HasDisplayed = true;

    [HideInInspector] public string GunShot;
    [HideInInspector] public string GunReloaded;

    [HideInInspector] public ParticleSystem m_MuzzleFlash;
    [Range(0, 5)][SerializeField] private float m_MuzzleFlashLifeTime = 0.1f;
    [HideInInspector] public ParticleSystem m_MuzzleSmoke;
    [Range(0, 5)][SerializeField] private float m_MuzzleSmokeLifeTime = 0.5f;

    protected LightFeedbackTemp _lightFeedback;

    protected virtual void Start()
    {
        if (m_GunReference == null) m_GunReference = GetComponentInChildren<gunRef>().gameObject;
        m_Controller = GetComponent<IPlatformer2DUserControl>();
        m_DelayManager = GetComponent<DelayManager>();
        if(m_HpScript == null) m_HpScript = GetComponent<HPScript>();
        _lightFeedback = GetComponent<LightFeedbackTemp>();
        _lightFeedback.LightDone += ResetDelayManager;
    }

    protected void ResetDelayManager(object sender, System.EventArgs e)
    {
        m_DelayManager.SetDelay(0);
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (GameManager.instance.IsPlaying)
        {

            //CheckLight();

            if (m_Controller.m_Shoot && m_DelayManager.CanShoot)
            {
                Fire();
            }
        }
    }

    public virtual void Fire()
    {
        m_HasDisplayed = false;
        FireOneBullet();

        m_DelayManager.AddDelay(m_ShotDelay);
    }

    public void FireOneBullet()
    {
        Bullet newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
        newBullet.ShootBullet();
        newBullet.AddImmuneTarget(m_HpScript);
        if (BulletFired != null) BulletFired(this, new BulletFiredEventArgs { BulletFired = newBullet });
        SoundManager.PlaySFX(GunShot);
    }

    private void CheckLight()
    {
        if (!m_HasDisplayed && m_DelayManager.CanShoot) _lightFeedback.StartLightFeedback(m_ShotDelay);
    }

    protected void DisplayParticles()
    {
        InstatiateParticle(m_MuzzleFlash, m_GunRef, true, m_MuzzleFlashLifeTime);
        InstatiateParticle(m_MuzzleSmoke, m_GunRef, true, m_MuzzleSmokeLifeTime);
    }

    public event EventHandler<BulletFiredEventArgs> BulletFired;

}
