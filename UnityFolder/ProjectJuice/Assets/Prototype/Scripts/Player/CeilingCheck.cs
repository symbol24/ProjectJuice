using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class CeilingCheck : MonoBehaviour {
    [SerializeField] private PlatformerCharacter2D m_parent;

	void OnTriggerEnter2D(Collider2D collider)
    {
        //m_parent.CheckPassThrough(collider);
    }

    void OntriggerExit2D(Collider2D collider)
    {
        //m_parent.ResetPassthrough(collider);
    }
}
