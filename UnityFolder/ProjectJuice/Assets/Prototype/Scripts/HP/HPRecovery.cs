using UnityEngine;
using System.Collections;

public class HPRecovery : ExtendedMonobehaviour, IPowerUp 
{

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsAvailableForConsumption { get { return true; } }
    public void Consumed()
    {
        var destroyer = gameObject.AddComponent<DestroyOnTimer>();
        destroyer.Timeout = 0;
    }

    public float HPRecov { get; set; }
   
}
