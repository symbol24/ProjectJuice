using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class ArcShooting : Gun {
    float m_yAxis;
    float m_xAxis;
    public bool m_isTwinStick;
    string m_VerticalName = "Vertical";
    string m_HorizontalName = "Horizontal";
    string m_TSVertical = "rightVertical";
    string m_TSHorizontal = "rightHorizontal";
    string m_ArcVertical = "Vertical";
    string m_ArcHorizontal = "Horizontal";

    public float m_TwinStickDelay = 0.3f;
    private float m_TwinStickTimer;

    new void Start()
    {
        base.Start();
        print(m_VerticalName);
        m_CurrentDelay = m_ShotDelayMedium;
    }

    // Update is called once per frame
    new void Update() {
        base.Update();
        if((m_isTwinStick && m_VerticalName != m_TSVertical) || (!m_isTwinStick && m_VerticalName != m_ArcVertical))
            SetTwinStick(m_isTwinStick);

        m_yAxis = CrossPlatformInputManager.GetAxis(m_VerticalName);
        m_xAxis = CrossPlatformInputManager.GetAxis(m_HorizontalName);
        
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        if (m_xAxis != 0f || m_yAxis != 0f)
        {
            RotateGun(m_xAxis, m_yAxis);
        }
    }

    private void RotateGun(float xAxis, float yAxis)
    {
        float zAngle = -(Mathf.Atan2(xAxis, yAxis) * Mathf.Rad2Deg);
        float xAngle = 0f;
        float yAngle = 0f;


        transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);
        if (m_isTwinStick) Fire();
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

    private void SetTwinStick(bool isTwinStick)
    {

        if (isTwinStick)
        {
            m_VerticalName = m_TSVertical;
            m_HorizontalName = m_TSHorizontal;
            m_TwinStickTimer = Time.time;
        }
        else
        {
            m_VerticalName = m_ArcVertical;
            m_HorizontalName = m_ArcHorizontal;
        }
    }
}
