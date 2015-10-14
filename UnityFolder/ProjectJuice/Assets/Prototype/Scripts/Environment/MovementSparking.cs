using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GroundChecker), typeof(MovementChecker))]
public class MovementSparking : ExtendedMonobehaviour
{
    public GameObject _effectsToSpawn;
    private GroundChecker _groundChecker;
    private MovementChecker _movementChecker;
    [SerializeField] private float _sparklesTimeout = 1f;
    public List<Transform> _pointsOfSparks;
    private float _groundTolerance = 0.01f;

    // Use this for initialization
	void Start ()
	{
	    _groundChecker = GetComponent<GroundChecker>();
	    _movementChecker = GetComponent<MovementChecker>();
        _movementChecker.X_PositionChanged += XPositionChanged;
	}

    private void XPositionChanged(object sender, PositionChangedEventArgs positionChangedEventArgs)
    {
        base.GetPointOfImpact(_groundChecker.PointOfCollision.transform, transform);

        if (_groundChecker.IsGrounded)
        {
            var lowestPoint = _pointsOfSparks.Min(c => c.transform.position.y);
            var pointsCloseToGround =
                _pointsOfSparks.Where(c => c.transform.position.y < (lowestPoint + _groundTolerance));


            foreach (var pointsOfSpark in pointsCloseToGround)
            {
                var positionToSpawn = pointsOfSpark.position;
                var particles =
                    (GameObject)
                        Instantiate(_effectsToSpawn,
                            positionToSpawn,
                            _effectsToSpawn.transform.rotation);
                particles.transform.parent = transform;
                particles.AddComponent<DestroyOnTimer>().Timeout = _sparklesTimeout;
            }
            
        }
    }

    // Update is called once per frame
	void Update () {
	    
	}
}
