using UnityEngine;
using System.Collections;

public class RagdollHeadPop : MonoBehaviour {
    [SerializeField] private float _impulseValue = 500f;
    [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Impulse;
    private Rigidbody2D _rigidbody;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody != null) _rigidbody.AddForce(new Vector2(0, _impulseValue), _forceMode);
        else Debug.LogError(gameObject.name + " cannot find rigidbody2d");
	}
}
