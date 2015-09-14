using UnityEngine;
using System.Collections;

public class HPCollider : MonoBehaviour
{
    public HPScript _hpScript;

	// Use this for initialization
	void Start ()
	{
	    if (_hpScript == null) GetComponentInParent<HPScript>();
	    if (_hpScript == null) Debug.LogError("NoHPFor HPCollider");
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    private void OnTriggerEnter2D(Collider2D collider)
    {
        _hpScript.RouteOnTriggerEnter2D(collider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _hpScript.RouteOnCollisionEnter2D(collision);
    }



}
