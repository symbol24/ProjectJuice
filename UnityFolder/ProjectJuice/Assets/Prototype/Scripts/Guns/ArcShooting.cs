﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;
using GamepadInput;

public class ArcShooting : Gun
{
    [Range(0, 3)][SerializeField] private float m_BurstDelay = 0.1f;
    [Range(0, 10)][SerializeField] int m_burstAmmount = 3;

    protected override void Update()
    {
        if (GameManager.instance.IsPlaying)
        {
            base.Update();

            m_Gun.transform.eulerAngles = GetRotation(m_Controller);
        }
    }

    public void SpawnRotation(IPlatformer2DUserControl controller)
    {
        m_Gun.transform.eulerAngles = GetRotation(controller);
    }

    public override void Fire()
    {
        m_hasPlayedReloaded = false;
        StartCoroutine(BurstFire());
    }

    IEnumerator BurstFire()
    {
        m_DelayManager.AddDelay(100f);
        _feedBack.SetBool();
        for (int i = m_burstAmmount; i >= 0; i--)
        {
            if (i > 0)
            {
                FireOneBullet();
            }
            else
            {
                m_DelayManager.SetDelay(m_Delay);
            }

            yield return new WaitForSeconds(m_BurstDelay);
        }
    }
}
