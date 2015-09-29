using UnityEngine;
using System.Collections;

public class ShieldReRouter : MonoBehaviour {
    [SerializeField] private shield m_Shield;

    void OnTriggerEnter2D (Collider2D collider)
    {
        m_Shield.RoutedTriggerEnter(collider);
    }
	
}
