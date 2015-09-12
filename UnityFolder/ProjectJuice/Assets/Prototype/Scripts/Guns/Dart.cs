using UnityEngine;
using System.Collections;
using System;

public class Dart : MonoBehaviour {

    public float Speed { get; set; }
    public Vector2 Angle { get; set; }
    public bool IsContiniousSucking { get; set; }
    public float SuckingInterval { get; set; }
    public float HpSuckedPerSecond { get; set; }
    public float DartCollTimerDisappear { get; set; }

    private HPScript _targetHpScript;
    private float currentIntervalTimer = 0f;
    private bool _hitAWall = false;
    private float destroyTimer;
    private Rigidbody2D _mainRigidbody;

    // Use this for initialization
    void Start()
    {
        _mainRigidbody = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(_hitAWall)
        {
            destroyTimer += Time.deltaTime;
            if(destroyTimer >= DartCollTimerDisappear)
            {
                OnDartDestroyed();
                Destroy(this.gameObject);
            }
        }
        else if (_targetHpScript != null)
        {
            if (IsContiniousSucking)
            {
                SuckHP();
            }
            else
            {
                currentIntervalTimer += Time.deltaTime;
                if (currentIntervalTimer >= SuckingInterval)
                {
                    SuckHP();
                    currentIntervalTimer = 0f;
                }
            }
        }
        else
        {
            _mainRigidbody.MovePosition(transform.position.ToVector2() + (Angle * Speed));
        }
	}

    


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!CheckForHPCollision(collider.gameObject))
        {
            CheckForWall(collider.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!CheckForHPCollision(collision.gameObject))
        {
            CheckForWall(collision.gameObject);
        }
    }

    private void CheckForWall(GameObject gameObject)
    {
        //For now
        _hitAWall = true;
        OnDartCollision();
    }

    private bool CheckForHPCollision(GameObject toCheck)
    {
        var ret = false;
        if (_targetHpScript == null)
        {
            _targetHpScript = toCheck.GetComponent<HPScript>();
            if (_targetHpScript != null)
            {
                OnDartCollision();
                ret = true;
            }
        }
        return ret;
    }
    private void SuckHP()
    {
        float hpToSuck = HpSuckedPerSecond / SuckingInterval;
        OnJuiceSucked(new JuiceSuckedEventArgs { HpSucked = hpToSuck });
        _targetHpScript.CurrentHp -= hpToSuck;
    }
    #region events
    public event EventHandler<JuiceSuckedEventArgs> JuiceSucked;
    protected void OnJuiceSucked(JuiceSuckedEventArgs e)
    {
        if(JuiceSucked != null)
        {
            JuiceSucked(this, e);
        }
    }
    public event EventHandler DartCollision;
    protected void OnDartCollision()
    {
        if (DartCollision != null) DartCollision(this, EventArgs.Empty);
    }
    public event EventHandler DartDestroyed;
    protected void OnDartDestroyed()
    {
        if (DartDestroyed != null) DartDestroyed(this, EventArgs.Empty);
    }
    #endregion events
}
