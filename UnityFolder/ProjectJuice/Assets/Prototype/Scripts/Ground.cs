using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {

	[SerializeField] private bool m_IsPassThrough = false;
    public bool IsPassThrough { get { return m_IsPassThrough; } }
}
