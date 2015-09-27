using System;
//using UnityEditor;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PhysicsParticleSystem : ExtendedMonobehaviour
{
    [Range(0,1)][SerializeField] private float _probabilityOfPickup = 0.1f;
    [SerializeField] private bool _overrideProbabilityToParticles = true;
    [SerializeField] private bool _overrideHealthAmountPerParticle = true;
    [SerializeField] private float _healthRecoveredPerParticle = 5f;
    public float _minXSpeed = 1f;
    public float _maxXSpeed = 5f;
    private float RandomXSpeed
    {
        get
        {
            var range = _maxXSpeed - _minXSpeed;
            var ret = Random.value*range + _minXSpeed;
            return ret;
        }
    }
    public float _minYSpeed = 1f;
    public float _maxYSpeed = 5f;
    private float RandomYSpeed
    {
        get
        {
            var range = _maxYSpeed - _minYSpeed;
            var ret = Random.value*range + _minYSpeed;
            if (_allowPositiveYForce) ret = Mathf.Abs(ret);
            return ret;
        }
    }
    public bool _allowPositiveYForce = true;
    public int PointsOfDamagePerParticle = 2;

    public HPScript _hpScript;

    [SerializeField]
    private GameObject _singleParticle;
    public GameObject SingleParticle
    {
        get { return _singleParticle; }
        set { _singleParticle = value; }
    }

    private PhysicsSingleParticle _particleScript;

    // Use this for initialization
    private void Start()
    {
        _particleScript = SingleParticle.GetComponent<PhysicsSingleParticle>();
        if(_particleScript == null)Debug.Log("gameObject attachs is a nonParticle");
        if (_hpScript == null)
        {
            Debug.Log("Attempting to getComponentFor hpScript");
            _hpScript = GetComponent<HPScript>();
            if (_hpScript == null)
            {
                Debug.LogError("hpScript not found");
            }
        }
        _hpScript.HpImpactReceived += HpScriptOnHpImpactReceived;
    }


    private void HpScriptOnHpImpactReceived(object sender, ImpactEventArgs impactEventArgs)
    {
        var iterations = Mathf.CeilToInt(impactEventArgs.Damage/(float) PointsOfDamagePerParticle);
        for (int i = 0; i < iterations; i++)
        {
            var singleParticle =
                Instantiate(SingleParticle, impactEventArgs.PointOfCollision, Quaternion.identity) as GameObject;
            var spriteRenderer = singleParticle.GetComponent<SpriteRenderer>();
            spriteRenderer.color = impactEventArgs.color;
            var particleScript = singleParticle.GetComponent<PhysicsSingleParticle>();
            if (_overrideProbabilityToParticles) particleScript.ProbabilityOfHealthPickup = _probabilityOfPickup;
            if (_overrideHealthAmountPerParticle) particleScript.HpRecovered = _healthRecoveredPerParticle;
            var particleRigidBody = particleScript.gameObject.GetComponent<Rigidbody2D>();
            var forceDirection =
                new Vector2(
                    (Mathf.Abs(RandomXSpeed)*Mathf.Sign(impactEventArgs.PointOfCollision.x - transform.position.x)),
                    RandomYSpeed);
            particleRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
        }
    }


    // Update is called once per frame
	void Update () {
	
	}
}
