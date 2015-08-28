using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Gun : MonoBehaviour {

    public GameObject m_GunReference;
    public Bullet m_BulletPrefab;
    private bool m_Fire1;


    // Use this for initialization
    protected void Start () {
	
	}
	
	// Update is called once per frame
	protected void Update () {


        if (!m_Fire1)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Fire1 = CrossPlatformInputManager.GetButtonDown("Fire1");
        }
    }

    private void FixedUpdate()
    {


        if (m_Fire1)
            Fire();

        m_Fire1 = false;
    }



    public virtual void Fire()
    {
        print("fire");
        Bullet newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
    }
}
