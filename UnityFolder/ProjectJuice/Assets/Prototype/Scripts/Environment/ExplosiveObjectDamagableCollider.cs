using UnityEngine;
using System.Collections;

public class ExplosiveObjectDamagableCollider : MonoBehaviour
{

    [SerializeField] private ExplosiveObject _explosiveObject;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _explosiveObject.RouteOnCollisionEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        _explosiveObject.RouteOnTriggerEnter2D(collider);
    }

    private void OnCollisionStay2D(Collision2D collider)
    {
        _explosiveObject.RouteOnCollisionStay2D(collider.collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        _explosiveObject.RouteOnCollisionStay2D(collider);
    }
}
