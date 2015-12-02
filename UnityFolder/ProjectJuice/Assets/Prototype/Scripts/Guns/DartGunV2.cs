using System;
using UnityEngine;
using System.Collections;

public class DartGunV2 : DartGunBase
{
    public DartV2 _dartPrefab;
    public DartChainV3 _dartChainPrefab;
    public DartChainV3 _kinematicChain;

    private Coroutine _spawningHoseCoroutine;
    private IDart _currentDart;
    protected override void FireDart()
    {
        var leDart =
            (DartV2) Instantiate(_dartPrefab, _dartSpawnPoint.transform.position, _dartSpawnPoint.transform.rotation);
        leDart.PushConfig(Settings, _inputManager, _delayManager);
        leDart.ShootBullet(Settings.DartForce, _dartSpawnPoint.transform);
        OnDartFired(new DartFiredEventArgs { Dart = leDart });

        _spawningHoseCoroutine = StartCoroutine(SpawnHosePortions(this, leDart));
        _currentDart = leDart;
        leDart.DartDestroyed += LeDart_DartDestroyed;
        SoundManager.PlaySFX(Fire);
        InstatiateParticle(m_firingParticle, ParticlesSpawningPoint, true);
    }

    private void LeDart_DartDestroyed(object sender, EventArgs e)
    {
        StopCoroutine(_spawningHoseCoroutine);
    }

    [HideInInspector] public DartGunSettings _settings;

    public override DartGunSettings Settings { get { return _settings; } set { _settings = value; } }

    private IEnumerator SpawnHosePortions(DartGunV2 dartGunV2, DartV2 leDart)
    {
        DartChainV3 closestHose = leDart._kinematicChain;
        bool _dartCollided = false;
        leDart.DartCollision += (sender, e) => _dartCollided = true;
        int numberOfCrossSectionsSpawned = 0;
        while(!_dartCollided)
        {
            //Debug.Log(Vector3.Distance(closestHose.transform.position, dartGunV2._kinematicChain.transform.position) + " " + Settings.HoseLength);
            while (Vector3.Distance(leDart._kinematicChain.transform.position, dartGunV2._kinematicChain.transform.position) > Settings.HoseLength * numberOfCrossSectionsSpawned)
            {
                //Debug.Log(Vector3.Distance(closestHose.transform.position, dartGunV2._kinematicChain.transform.position) + " " + Settings.HoseLength);
                var positionRelative = -closestHose.transform.position + dartGunV2._kinematicChain.transform.position;
                var newClosestHose = (DartChainV3)Instantiate(_dartChainPrefab, closestHose.transform.position + positionRelative, _dartSpawnPoint.transform.rotation);

                leDart.AssignOwner(this);
                leDart.SubscribeToChain(newClosestHose);
                newClosestHose._distanceJoint.distance = Settings.JointLength;
                newClosestHose.SubscribeToDart(leDart,this);
                newClosestHose.AddNext(closestHose, Settings);

                closestHose = newClosestHose;
                numberOfCrossSectionsSpawned++;
            }
            //Debug.Break();
            yield return new WaitForFixedUpdate();
        }

        //Amazingly bad practice :(
        if(numberOfCrossSectionsSpawned <= Settings.MinimumCrossSectionsToSpawn)
        {
            while(numberOfCrossSectionsSpawned <= Settings.MinimumCrossSectionsToSpawn)
            {
                var positionRelative = -closestHose.transform.position + dartGunV2._kinematicChain.transform.position;
                var newClosestHose = (DartChainV3)Instantiate(_dartChainPrefab, closestHose.transform.position + positionRelative, _dartSpawnPoint.transform.rotation);

                leDart.AssignOwner(this);
                leDart.SubscribeToChain(newClosestHose);
                newClosestHose._distanceJoint.distance = Settings.JointLength;
                newClosestHose.SubscribeToDart(leDart, this);
                newClosestHose.AddNext(closestHose, Settings);

                closestHose = newClosestHose;
                numberOfCrossSectionsSpawned++;
            }
            //Debug.Break();
        }
        closestHose.Track(_kinematicChain);
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_currentDart != null)
        {
            _currentDart.DartDestroyed -= LeDart_DartDestroyed;
        }
    }

}
