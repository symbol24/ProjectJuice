using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class gunRef : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
            Flip(true);
        else if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
            Flip(false);
	
	}

    void Flip(bool isRight)
    {
        float newZ = 270f;

        if (!isRight) newZ = 90f;


        Quaternion newQ = transform.rotation;
 
        if (newQ.eulerAngles.z != newZ)
        {
            newQ = Quaternion.Euler(newQ.eulerAngles.x, newQ.eulerAngles.y, newZ);
            transform.rotation = newQ;
        }
    }
}
