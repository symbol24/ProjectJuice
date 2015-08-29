using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Gun : MonoBehaviour {

    public GameObject m_GunReference;
    public Bullet m_BulletPrefab;
    private bool m_Fire1;
    public float m_ShotDelayFast = 0.1f;
    public float m_ShotDelayMedium = 0.3f;
    private float m_ShotTimer;
    protected float m_CurrentDelay;


    // Use this for initialization
    protected void Start () {
        m_ShotTimer = Time.time;
	}
	
	// Update is called once per frame
	protected void Update ()
    {
        m_Fire1 = CrossPlatformInputManager.GetButton("Fire1");
    }

    protected void FixedUpdate()
    {
        if (m_Fire1 && Time.time > m_ShotTimer)
        {
            Fire();
            m_ShotTimer = Time.time + m_CurrentDelay;
        }
    }



    public virtual void Fire()
    {
        print("fire");
    }
}
