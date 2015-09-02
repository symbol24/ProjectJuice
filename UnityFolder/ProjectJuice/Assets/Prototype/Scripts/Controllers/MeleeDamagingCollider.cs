using UnityEngine;
using System.Collections;

public class MeleeDamagingCollider : MonoBehaviour, IDamaging
{

    public MeleeAttack _meleeAttack;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	


	}

    public bool IsAvailableForConsumption { get { return _meleeAttack.IsAvailableForConsumption; } }
    public void Consumed()
    {
        _meleeAttack.Consumed();
    }

    public float Damage { get { return _meleeAttack.Damage; } }
}
