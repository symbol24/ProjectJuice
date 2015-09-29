using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class HPBase : ExtendedMonobehaviour {

    [SerializeField]
    private float _maxHp;
    private float _currentHp;


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
            if (_currentHp != value)
            {
                var newHpValue = value >= MaxHp ? MaxHp : _currentHp = value;
                var e = new HpChangedEventArgs() { NewHp = newHpValue, PreviousHp = _currentHp };
                _currentHp = newHpValue;
                OnHpChanged(e);
            }
        }
    }


    public event EventHandler<HpChangedEventArgs> HpChanged;
    protected virtual void OnHpChanged(HpChangedEventArgs e)
    {
        EventHandler<HpChangedEventArgs> handler = HpChanged;
        if (handler != null) handler(this, e);
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
        var ret = checkDamaging.IsAvailableForConsumption && !IsAChild(checkDamaging);
        if (ret && checkDamaging.HasImmuneTargets)
        {
            //If it is not in the immunetargets collection, it should do damage
            ret = !checkDamaging.ImmuneTargets.Any(c => c == this);
        }
        return ret;

    }
}
