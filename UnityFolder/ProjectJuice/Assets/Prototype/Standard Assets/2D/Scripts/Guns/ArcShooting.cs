using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;
using GamepadInput;

public class ArcShooting : Gun {

    
    new void Update() {
        base.Update();

        RotateGun(m_Controller.m_XAxis, m_Controller.m_YAxis);
    }

    private void RotateGun(float xAxis, float yAxis)
    {
        float zAngle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
        float xAngle = 0f;
        float yAngle = 0f;

        transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);
    }

}
