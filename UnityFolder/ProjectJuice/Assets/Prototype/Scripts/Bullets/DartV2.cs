using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DontGoThroughThings))]
public class DartV2 : DartBase
{
    public DartChainV3 _kinematicChain;

    private bool _collidedWithSmth = false;

    private void Start()
    {
        DartCollision += DartV2_DartCollision;
    }

    void Update()
    {
        bool dartToBeDestroyed = !_inputManager.m_SpecialStay && !_settings.DartInvencible;
        if (dartToBeDestroyed) OnDartDestroyed();
    }

    private IEnumerator DestroyOnTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        OnDartDestroyed();
    }
    private IEnumerator _toDestroy;
    private DartGunV2 _dartGun;

    public override HPScript Target { get { return _target; } }
    private HPScript _target;

    private void DartV2_DartCollision(object sender, EventArgs e)
    {
        if(_toDestroy == null)
        {
            _toDestroy = DestroyOnTimer(_settings._dartDestroyTimer);
            MainRigidbody2D.isKinematic = true;
            StartCoroutine(_toDestroy);
        }
    }

    private IEnumerator DestroyOnTimer(object _dartDestroyTimer)
    {
        yield return new WaitForSeconds(_settings._dartDestroyTimer);
        OnDartDestroyed();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_collidedWithSmth)
        {
            var hpScript = collider.gameObject.GetComponent<HPScript>();
            if (hpScript == null) hpScript = collider.gameObject.GetComponentInParent<HPScript>();
            if (hpScript != null && _dartGun.IsAChild(hpScript)) return;
            else if (hpScript != null)
            {
                if (hpScript != null)
                {
                    HandleHpScript(hpScript);
                }
            }
            else
            {
                var ground = collider.gameObject.GetComponent<Ground>();
                //var explosion = collider.gameObject.GetComponent<ExplosiveObjectDamagableCollider>();
                if ((ground == null || !ground.IsPassThrough) && collider.gameObject.name != "ExplosionColliders" )
                {
                    CollideWithIt();
                }
            }
        }
    }

    private void HandleHpScript(HPScript hpScript)
    {
        _collidedWithSmth = true;
        var shield = hpScript.gameObject.GetComponent<shield>();
        if (shield != null && shield.IsShieldActive)
        {
            StickTo(shield.gameObject);
            SoundManager.PlaySFX(Severed);
        }
        else
        {
            _collidedWithSmth = true;
            _target = hpScript;
            StickTo(hpScript.gameObject);
            _suckHpCoroutine = StartCoroutine(SuckHpFrom(hpScript));
            _hpScriptToUnsubscribe = hpScript;
            _dartToUnsubscribe = this;
            hpScript.Dead += HpScript_Dead;
            DartDestroyed += HpScript_Dead;
            SoundManager.PlaySFX(PlayerImpact);
        }
        OnDartCollision();
    }


    Coroutine _suckHpCoroutine;
    HPScript _hpScriptToUnsubscribe;
    IDart _dartToUnsubscribe;
    private void HpScript_Dead(object sender, EventArgs e)
    {
        StopCoroutine(_suckHpCoroutine);
        _hpScriptToUnsubscribe.Dead -= HpScript_Dead;
        _dartToUnsubscribe.DartDestroyed -= HpScript_Dead;
        OnDartDestroyed();
    }

    private void StickTo(GameObject toStickTo)
    {
        MainRigidbody2D.isKinematic = true;
        gameObject.transform.parent = toStickTo.transform;
    }

    private IEnumerator SuckHpFrom(HPScript hpScript)
    {
        while (true)
        {
            if (Time.deltaTime > 0 && GameManager.instance.CurrentState == GameState.Playing)
            {
                float hpToSuck = _settings.HpSuckedPerSecond * Time.deltaTime;
                hpScript.CurrentHp -= hpToSuck;
                OnJuiceSucked(new JuiceSuckedEventArgs { HpSucked = hpToSuck });
            }
            yield return null;
        }
    }

    private void CollideWithIt()
    {
        OnDartCollision();
        _collidedWithSmth = true;
        MainRigidbody2D.isKinematic = true;
        SoundManager.PlaySFX(GroundImpact);
    }

    public void SubscribeToChain(DartChainV3 chain)
    {
        chain.BrokenOnTolerance += (sender, e) => OnDartDestroyed();
        chain.BrokenOnGround += (sender, e) => OnDartDestroyed();
    }

    public void AssignOwner(DartGunV2 dartGun)
    {
        _dartGun = dartGun;
    }

}
