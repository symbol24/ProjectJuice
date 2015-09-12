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

        m_GunProperties.transform.eulerAngles = GetRotation(m_Controller);
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
