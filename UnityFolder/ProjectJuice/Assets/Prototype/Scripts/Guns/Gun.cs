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
    [SerializeField] private GameObject m_GunObject;
    public GameObject m_Gun { get { return m_GunObject; } }
    [SerializeField] private Bullet m_BulletPrefab;
    public Bullet m_Bullet { get { return m_BulletPrefab; } }
    [Range(0,5)][SerializeField] private float m_ShotDelay = 0.1f;
    public float m_Delay { get { return m_ShotDelay; } }
    [SerializeField] private HPScript m_HpScript;
    public HPScript m_HpScp { get { return m_HpScript; } }
    [SerializeField] private Light m_GunLight;
    [Range(0,3)][SerializeField] float m_LightOn = 0.1f;
    [Range(0,3)][SerializeField] float m_LightOff = 0.1f;
    [Range(0, 10)][SerializeField] private int m_AmountOfFlashes = 3;
    protected bool m_HasDisplayed = true;

    void Start()
    {
        if (m_GunReference == null) m_GunReference = GetComponentInChildren<gunRef>().gameObject;
        m_Controller = GetComponent<IPlatformer2DUserControl>();
        m_DelayManager = GetComponent<DelayManager>();
        if(m_HpScript == null) m_HpScript = GetComponent<HPScript>();
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (GameManager.instance.CheckIfPlaying())
        {

            CheckLight();

            if (m_Controller.m_Shoot && m_DelayManager.m_CanShoot)
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
    }

    private void CheckLight()
    {
        if (!m_HasDisplayed && m_DelayManager.m_CanShoot) StartCoroutine(DisplayGunLight());
    }

    IEnumerator DisplayGunLight()
    {
        m_HasDisplayed = true;
        for (int i = 0; i < m_AmountOfFlashes; i++)
        {
            m_GunLight.enabled = true;
            yield return new WaitForSeconds(m_LightOn);
            m_GunLight.enabled = false;
            yield return new WaitForSeconds(m_LightOff);
        }
    }
}
