using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PlayerJuiceSpiller : MonoBehaviour
{

    [SerializeField] private HPScript _hpScript;
    [Range(0, 1)] [SerializeField] private float _percentageOfMaxHealth = 0.5f;
    [SerializeField] private int _batchParticleNumber = 25;
    [SerializeField] private PhysicsSingleParticle _singlePartPrefab;
    public float _minXSpeed = 1f;
    public float _maxXSpeed = 5f;
    private float RandomXSpeed
    {
        get
        {
            var range = _maxXSpeed - _minXSpeed;
            var ret = Random.value * range + _minXSpeed;
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
            var ret = Random.value * range + _minYSpeed;
            if (_allowPositiveYForce) ret = Mathf.Abs(ret);
            return ret;
        }
    }
    private HpChangedEventArgs lastHpChanged;
    [SerializeField] private bool _allowPositiveYForce;


    // Use this for initialization
	void Start ()
	{
	    if (_hpScript == null) _hpScript = GetComponent<HPScript>();
	    if (_hpScript == null) _hpScript = GetComponentInParent<HPScript>();
	    if (_hpScript == null) Debug.LogError("Could not find HpScript");
        _hpScript.Dead += HpScriptOnDead;
        _hpScript.HpChanged += _hpScript_HpChanged;
	}

    void _hpScript_HpChanged(object sender, HpChangedEventArgs e)
    {
        lastHpChanged = e;
    }

    private void HpScriptOnDead(object sender, EventArgs eventArgs)
    {
        var diff = lastHpChanged.NewHp - lastHpChanged.PreviousHp;
        transform.parent = null;
        StartCoroutine(StartSpill());
    }

    private IEnumerator StartSpill()
    {
        var healthToRecov = _percentageOfMaxHealth*_hpScript.MaxHp;
        var numberOfParticles = healthToRecov/_singlePartPrefab.HpRecovered;
        while (numberOfParticles >= 0)
        {
            var currentBatch = numberOfParticles > _batchParticleNumber ? _batchParticleNumber : numberOfParticles;
            numberOfParticles -= currentBatch;
            for (int i = 0; i < currentBatch; i++)
            {
                var singleParticle = (PhysicsSingleParticle)Instantiate(_singlePartPrefab, transform.position, transform.rotation);
                var forceDirection =
                new Vector2(
                    (Mathf.Abs(RandomXSpeed) * Mathf.Sign(Random.value - 0.5f)),
                    RandomYSpeed);
                var particleRigidBody = singleParticle.gameObject.GetComponent<Rigidbody2D>();
                particleRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            }
            yield return null;
        }


    }



}
