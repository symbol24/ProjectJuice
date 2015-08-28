using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TwinStick : Gun {
    float m_yAxis;
    float m_xAxis;
    Quaternion m_startRotation;

    void Start()
    {
        base.Start();
        m_startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        m_yAxis = CrossPlatformInputManager.GetAxis("Vertical");
        m_xAxis = CrossPlatformInputManager.GetAxis("Horizontal");
        if (m_xAxis != 0f || m_yAxis != 0f)
            RotateGun(m_xAxis, m_yAxis);
        else
            transform.rotation = m_startRotation;
    }

    private void RotateGun(float xAxis, float yAxis)
    {
        float angle = -(Mathf.Atan2(xAxis, yAxis) * Mathf.Rad2Deg);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
    }
}
