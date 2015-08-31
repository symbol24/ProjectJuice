using System;
using UnityEngine;
using System.Collections;

public class DestroyOnTimer : MonoBehaviour 
{
    [SerializeField]
    private float _timeout;
    public float Timeout
    {
        get { return _timeout; }
        set { _timeout = value; }
    }

    private float _timePassed;

    public enum KindOfDestruction
    {
        Script,
        WholeGamObject
    };

    public KindOfDestruction Kind = KindOfDestruction.WholeGamObject;


    // Use this for initialization
    void Start ()
    {
        _timePassed = 0f;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    _timePassed += Time.deltaTime;
	    if (_timePassed >= Timeout)
	    {
	        switch (Kind)
	        {
	            case KindOfDestruction.Script:
	                Destroy(this);
	                break;
	            case KindOfDestruction.WholeGamObject:
	                Destroy(this.gameObject);
	                break;
	            default:
	                throw new Exception("How did we get here");
	        }
	    }
	}
}
