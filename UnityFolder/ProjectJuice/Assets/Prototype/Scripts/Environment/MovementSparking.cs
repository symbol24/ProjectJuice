using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GroundChecker), typeof(MovementChecker))]
public class MovementSparking : ExtendedMonobehaviour
{
    [HideInInspector]public ParticleSystem _effectsToSpawn;
    private GroundChecker _groundChecker;
    private MovementChecker _movementChecker;
    private DelayManager _delayManager;
    [SerializeField] 
    private float _sparklesDestroyTimeout = 1f;
    [SerializeField]
    private float _sparklesSpawnCooldown = 0.1f;
    [Range(0, 1)]
    [SerializeField]
    private float _delayFourPushingSound = 0.4f;
    private string Pushing { get { return _explosiveObject.Pushing; } }
    private ExplosiveObject _explosiveObject;

    public List<Transform> _pointsOfSparks;
    private float _groundTolerance = 0.01f;

    // Use this for initialization
	void Start ()
	{
	    _groundChecker = GetComponent<GroundChecker>();
	    _movementChecker = GetComponent<MovementChecker>();
        _movementChecker.X_PositionChanged += XPositionChanged;
        _movementChecker.MovementStopped += DetectedStopping;
        _delayManager = GetComponent<DelayManager>();
        _explosiveObject = GetComponent<ExplosiveObject>();
	}
    private void XPositionChanged(object sender, PositionChangedEventArgs positionChangedEventArgs)
    {
        if (!_spawnSparklesRunning)
        {
            StartCoroutine(SpawnSparkles());
        }
    }
    private void DetectedStopping(object sender, EventArgs e)
    {
        print("stoppingSound");
        if(_currentAudio != null && _currentAudio.isPlaying)
        {
            _currentAudio.Stop();
        }
    }

    private bool _spawnSparklesRunning = false;
    private AudioSource _currentAudio;

    private IEnumerator SpawnSparkles()
    {
        if (!_spawnSparklesRunning)
        {
            _spawnSparklesRunning = true;
            //GetPointOfImpact(_groundChecker.PointOfCollision.transform, transform);

            if (_groundChecker.IsGrounded)
            {
                var lowestPoint = _pointsOfSparks.Min(c => c.transform.position.y);
                var pointsCloseToGround =
                    _pointsOfSparks.Where(c => c.transform.position.y < (lowestPoint + _groundTolerance));


                foreach (var pointsOfSpark in pointsCloseToGround)
                {
                    //InstatiateParticle()
                    
                    var positionToSpawn = pointsOfSpark.position;
                    InstatiateParticle(_effectsToSpawn, pointsOfSpark, true, _sparklesDestroyTimeout);
                    /*var particles =
                        (GameObject)
                            Instantiate(_effectsToSpawn,
                                positionToSpawn,
                                _effectsToSpawn.transform.rotation);
                    particles.transform.parent = transform;
                    particles.AddComponent<DestroyOnTimer>().Timeout = _sparklesDestroyTimeout;*/
                }
                if (_delayManager.SoundReady)
                {
                    //if (!isMuted)
                    //print("StartingSound");
                    _currentAudio = SoundManager.PlaySFX(Pushing);
                    _delayManager.AddSoundDelay(_delayFourPushingSound);
                }

                yield return new WaitForSeconds(_sparklesSpawnCooldown);
            }
            _spawnSparklesRunning = false;
        }
    }
}
