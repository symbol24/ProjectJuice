using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class GroundCheck : MonoBehaviour {
    [SerializeField] private PlatformerCharacter2D m_parent;

	void OnTriggerEnter2D(Collider2D collider)
    {
        //m_parent.CheckifFeetPassed(collider);
        m_parent.ResetPassthrough();
    }
}
