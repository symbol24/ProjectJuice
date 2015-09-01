using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;
using GamepadInput;

public class ArcShooting : Gun {
    float m_LeftAxisY;
    float m_LeftAxisX;
    float m_RightAxisY;
    float m_RightAxisX;
    public bool m_isTwinStick;
    private bool m_isIdle = true;

    public float m_TwinStickDelay = 0.3f;
    private float m_TwinStickTimer;

    new void Start()
    {
        base.Start();
        m_CurrentDelay = m_ShotDelayMedium;
    }

    // Update is called once per frame
    new void Update() {
        base.Update();

        if (m_isTwinStick)
        {
            m_RightAxisX = GamePad.GetAxis(GamePad.Axis.RightStick, controller).x;
            m_RightAxisY = GamePad.GetAxis(GamePad.Axis.RightStick, controller).y;
        }
        else
        {
            m_LeftAxisY = GamePad.GetAxis(GamePad.Axis.LeftStick, controller).y;
            m_LeftAxisX = GamePad.GetAxis(GamePad.Axis.LeftStick, controller).x;
        }

        if (m_isTwinStick && (m_RightAxisX != 0 || m_RightAxisY != 0))
            m_isIdle = false;
        else if (!m_isTwinStick && (m_LeftAxisX != 0 || m_LeftAxisY != 0))
            m_isIdle = false;
        else
            m_isIdle = true;
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        if (m_isTwinStick && !m_isIdle)
        {
            RotateGun(m_RightAxisX, m_RightAxisY);
        }
        else if(!m_isTwinStick && !m_isIdle)
        {
            RotateGun(m_LeftAxisX, m_LeftAxisY);
        }
    }

    private void RotateGun(float xAxis, float yAxis)
    {
        float zAngle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
        float xAngle = 0f;
        float yAngle = 0f;


        transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);
        if (m_isTwinStick && !m_isIdle) Fire();
    }

    public override void Fire()
    {
        base.Fire();
        bool canFire = true;
        if (m_isTwinStick)
        {
            canFire = false;
            if (Time.time > m_TwinStickTimer)
            {
                canFire = true;
                m_TwinStickTimer = Time.time + m_TwinStickDelay;
            }
        }

        Bullet newBullet;
        if (canFire)
            newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
    }
}
