using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using GamepadInput;
using UnityStandardAssets._2D;

public class Gun : ExtendedMonobehaviour {
    public Platformer2DUserControl m_Controller { get; private set; }

    [SerializeField] private GameObject m_GunReference; //used for flipping maybe
    [SerializeField] private GameObject m_Gun;
    [SerializeField] private Bullet m_BulletPrefab;
    [SerializeField] private float m_ShotDelay = 0.1f;
    [SerializeField] private float m_BurstDelay = 0.1f;
    
    private float m_ShotTimer;


    void Awake()
    {
        m_Controller = GetComponent<Platformer2DUserControl>();
    }

    // Use this for initialization
    protected void Start () {
        m_ShotTimer = Time.time;
	}
	
	// Update is called once per frame
	protected void Update ()
    {
        if (m_Controller.m_Shoot && Time.time > m_ShotTimer)
        {
            Fire();
            m_ShotTimer = Time.time + m_ShotDelay;
        }
    }

    public virtual void Fire()
    {
        Bullet newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
    }
}
