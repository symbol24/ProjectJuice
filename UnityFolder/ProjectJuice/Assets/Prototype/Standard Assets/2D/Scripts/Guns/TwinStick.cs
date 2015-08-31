using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TwinStick : Gun {
    float m_yAxis;
    float m_xAxis;

    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        m_yAxis = CrossPlatformInputManager.GetAxis("rightVertical");
        m_xAxis = CrossPlatformInputManager.GetAxis("rightHorizontal");

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
    }

    public override void Fire()
    {
        base.Fire();
        Bullet newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
    }
}
