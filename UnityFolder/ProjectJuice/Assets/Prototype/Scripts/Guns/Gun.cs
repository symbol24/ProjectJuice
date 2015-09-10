using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;
using UnityStandardAssets._2D;

public class Gun : ExtendedMonobehaviour {
    public IPlatformer2DUserControl m_Controller { get; private set; }
    public DelayManager m_DelayManager { get; private set; }

    [SerializeField] private GameObject m_GunReference; //used for flipping maybe
    public GameObject m_GunRef { get { return m_GunReference; } }
    [SerializeField] private GameObject m_Gun;
    public GameObject m_GunProperties { get { return m_Gun; } }
    [SerializeField] private Bullet m_BulletPrefab;
    public Bullet m_Bullet { get { return m_BulletPrefab; } }
    [Range(0,5)][SerializeField] private float m_ShotDelay = 0.1f;
    public float m_Delay { get { return m_ShotDelay; } }
    [SerializeField] private HPScript m_HpScript;
    public HPScript m_HpScp { get { return m_HpScript; } }


    protected virtual void Start()
    {
        m_Controller = GetComponent<IPlatformer2DUserControl>();
        m_DelayManager = GetComponent<DelayManager>();
        if(m_HpScript == null) m_HpScript = GetComponent<HPScript>();
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (m_Controller.m_Shoot && m_DelayManager.m_CanShoot)
        {
            Fire();
        }
    }

    public virtual void Fire()
    {
        FireOneBullet();

        m_DelayManager.AddDelay(m_ShotDelay);
    }

    public void FireOneBullet()
    {
        Bullet newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
        newBullet.ShootBullet();
        newBullet.AddImmuneTarget(m_HpScript);
    }
}
