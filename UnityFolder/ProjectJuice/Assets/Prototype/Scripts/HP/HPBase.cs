using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class HPBase : ExtendedMonobehaviour {

    [SerializeField]
    private float _maxHp;
    private float _currentHp;

    private object _currentHpLock = new object();

    public float MaxHp
    {
        get { return _maxHp; }
        protected set { _maxHp = value; }
    }
    public float CurrentHp
    {
        get { return _currentHp; }
        set
        {
           // lock (_currentHpLock)
           // {
                if (_currentHp != value)
                {
                    var newHpValue = value >= MaxHp ? MaxHp : value;
                    var e = new HpChangedEventArgs() { NewHp = newHpValue, PreviousHp = _currentHp };
                    _currentHp = newHpValue;
                    if ((e.PreviousHp > 0 && e.NewHp >= 0) || (e.PreviousHp > 0 && e.NewHp <= 0)) OnHpChanged(e);
                }
            //}
        }
    }


    public event EventHandler<HpChangedEventArgs> HpChanged;
    protected virtual void OnHpChanged(HpChangedEventArgs e)
    {
        try
        {
            EventHandler<HpChangedEventArgs> handler = HpChanged;
            if (handler != null) handler(this, e);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }
    }


	// Use this for initialization
	protected virtual void Start () 
    {
        CurrentHp = MaxHp;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}

    protected bool CheckIfIDamagableIsActive(IDamaging checkDamaging)
    {
        var ret = checkDamaging.IsAvailableForConsumption(this) && !IsAChild(checkDamaging);
        if (ret && checkDamaging.HasImmuneTargets)
        {
            //If it is not in the immunetargets collection, it should do damage
            ret = !checkDamaging.ImmuneTargets.Any(c => c == this);
        }
        return ret;

    }

    protected ImpactForceSettings GetDirectionFromImpact(Collider2D collider, ImpactForceSettings toUpdate)
    {
        //if its a bullet we get the side on which we are getting hit and applying the addimpactforce in that direction
        Bullet bullet = collider.GetComponent<Bullet>();
        if (bullet != null)
        {
            Vector3 dir = (collider.gameObject.transform.position - gameObject.transform.position).normalized;
            if (Mathf.Abs(dir.z) < 0.05f)
            {
                if (dir.x > 0)
                    toUpdate.DirectionComingForm = Direction2D.Right;
                else if (dir.x < 0)
                    toUpdate.DirectionComingForm = Direction2D.Left;
            }
        }

        return toUpdate;
    }
}
