using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;
using System;
using System.Collections.Generic;

public class Gun : ExtendedMonobehaviour {
    public IPlatformer2DUserControl m_Controller { get; private set; }
    public DelayManager m_DelayManager { get; private set; }
    
    [SerializeField] private GameObject m_GunReference; //used for flipping maybe
    public GameObject GunBulletReference { get { return m_GunReference; } }
    [SerializeField] private GameObject m_ParticleRefernce;
    public GameObject ParticleReference { get { return m_ParticleRefernce; } }


    [SerializeField] private GameObject m_GunObject;
    public GameObject m_Gun { get { return m_GunObject; } }
    [SerializeField] private Bullet m_BulletPrefab;
    public Bullet m_Bullet { get { return m_BulletPrefab; } }
    [Range(0,5)][SerializeField] protected float m_ShotDelay = 0.1f;
    public float m_Delay { get { return m_ShotDelay; } }
    [SerializeField] private HPScript m_HpScript;
    public HPScript m_HpScp { get { return m_HpScript; } }
    protected bool m_hasPlayedReloaded = true;

    [SerializeField] private SpriteRenderer m_muzzleFlash;
    [SerializeField] private List<Sprite> m_muzzleFlashes;
    [Range(0,10)][SerializeField] private int m_framesBetweenFlashes = 2;

    [HideInInspector] public string GunShot;
    [HideInInspector] public string GunReloaded;
    [HideInInspector] public ParticleSystem m_MuzzleSmoke;
    [Range(0, 5)][SerializeField] private float m_MuzzleSmokeLifeTime = 0.5f;

    protected virtual void Start()
    {
        if (m_GunReference == null) m_GunReference = GetComponentInChildren<gunRef>().gameObject;
        m_Controller = GetComponent<IPlatformer2DUserControl>();
        m_DelayManager = GetComponent<DelayManager>();
        if(m_HpScript == null) m_HpScript = GetComponent<HPScript>();
        if (m_muzzleFlash != null && m_muzzleFlash.enabled) m_muzzleFlash.enabled = false;
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
            if (m_DelayManager.CanShoot && !m_hasPlayedReloaded) m_hasPlayedReloaded = PlayReload();

            if (m_Controller.m_Shoot && m_DelayManager.CanShoot)
            {
                Fire();
            }
        }
    }

    public virtual void Fire()
    {
        m_hasPlayedReloaded = false;
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
        DisplayParticles();

    }

    protected void DisplayParticles()
    {
        StartCoroutine(DisplayMuzzleFlashes());
        InstatiateParticle(m_MuzzleSmoke, ParticleReference, true, m_MuzzleSmokeLifeTime, m_Controller.m_FacingRight);
    }

    public event EventHandler<BulletFiredEventArgs> BulletFired;

    private IEnumerator DisplayMuzzleFlashes()
    {
        if (m_muzzleFlash != null)
        {
            m_muzzleFlash.enabled = true;
            for (int i = 0; i < m_muzzleFlashes.Count; i++)
            {
                m_muzzleFlash.sprite = m_muzzleFlashes[i];
                for (int x = 0; x < m_framesBetweenFlashes; x++)
                    yield return new WaitForEndOfFrame();
            }
            m_muzzleFlash.enabled = false;
        }
    }

    protected bool PlayReload()
    {
        SoundManager.PlaySFX(GunReloaded);
        return true;
    }
}
