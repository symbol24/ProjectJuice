using UnityEngine;
using System.Collections;
using System;

public class TriggerOnDistance : MonoBehaviour
{

    public float _maxDistance = 30f;
    private float _currentDistanceTravelled = 0f;

    private Vector3 previousPosition;

    public event EventHandler DistanceTravelled;

    // Use this for initialization
    void Start()
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _currentDistanceTravelled += Vector3.Distance(previousPosition, transform.position);
        previousPosition = transform.position;
        if(_currentDistanceTravelled >= _maxDistance)
        {
            if (DistanceTravelled != null) DistanceTravelled(this, EventArgs.Empty);
        }
        print(_currentDistanceTravelled);
    }
}
