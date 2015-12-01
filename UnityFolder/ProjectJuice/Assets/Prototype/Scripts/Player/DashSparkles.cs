using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;
using System.Collections.Generic;
using System.Linq;

public class DashSparkles : ExtendedMonobehaviour {

    [SerializeField]
    private PlatformerCharacter2D _movement;
    public List<GameObject> _refSpawnPoints;
    public ParticleSystem _particlesToSpawn;
    public float _particleCooldown = 0.2f;

    // Use this for initialization
    void Start()
    {
        if (_movement == null) _movement = GetComponent<PlatformerCharacter2D>();
        if (_movement == null) Debug.LogWarning("No PlatformerCharacter2d Found");
        if (!_refSpawnPoints.Any()) Debug.LogWarning("No SpawnPointsFound");
        if (_particlesToSpawn == null) Debug.LogWarning("NoParticlesToSpawn");     
        _movement.PlayerDashed += _movement_PlayerDashed;
        _movement.PlayerCanDashAgain += _movement_PlayerFinishedDashing;
    }

    private void _movement_PlayerFinishedDashing(object sender, System.EventArgs e)
    {
        if (_spawnParticlesRunning)
        {
            StopCoroutine(_spawnParticles);
            _spawnParticlesRunning = false;
        }
    }

    private void _movement_PlayerDashed(object sender, System.EventArgs e)
    {
        _spawnParticles = StartCoroutine(SpawnParticles());
    }

    private bool _spawnParticlesRunning = false;
    private Coroutine _spawnParticles;
    private IEnumerator SpawnParticles()
    {
        _spawnParticlesRunning = true;
        while (true)
        {
            if (_movement.IsGrounded)
            {
                while (!_movement.IsGrounded) ;
                foreach (var refPoint in _refSpawnPoints)
                {
                    InstatiateParticle(_particlesToSpawn, refPoint);
                }
            }
            yield return new WaitForSeconds(_particleCooldown);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
