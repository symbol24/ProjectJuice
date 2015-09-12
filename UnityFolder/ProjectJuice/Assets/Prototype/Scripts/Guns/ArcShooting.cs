using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;
using GamepadInput;

public class ArcShooting : Gun
{
    [SerializeField] private float m_BurstDelay = 0.1f;
    [Range(0, 10)][SerializeField] int m_burstAmmount = 3;

    private float m_BurstTimer;
    private bool m_CanFire = true;

    protected override void Update() {
        base.Update();

<<<<<<< HEAD
        m_GunProperties.transform.eulerAngles = GetRotation(m_Controller);
=======
        RotateGun(m_Controller.m_XAxis, m_Controller.m_YAxis);
    }

    private void RotateGun(float xAxis, float yAxis)
    {
        if (!m_Controller.m_FacingRight && xAxis > 0) xAxis = -xAxis;
        if (!m_Controller.m_FacingRight && xAxis == 0 && yAxis == 0) xAxis = -1f;

        float zAngle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
        float xAngle = 0f;
        float yAngle = 0f;
        
        m_Gun.transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);
>>>>>>> 0cd709af66132d1b0da1da45d276d965c3029652
    }

    public override void Fire()
    {
        StartCoroutine(BurstFire());
    }

    IEnumerator BurstFire()
    {
        m_CanFire = false;
        for (int i = m_burstAmmount; i >= 0; i--)
        {
            if (i > 0)
            {
                FireOneBullet();
                m_DelayManager.AddDelay(m_BurstDelay);
            }
            else
                m_DelayManager.AddDelay(m_Delay);

            yield return new WaitForSeconds(m_BurstDelay);
        }

        m_CanFire = true;
    }

}
