using UnityEngine;
using System.Collections;

public class DartChain : MonoBehaviour {

    public Dart DartSource { get; set; }
    public SappingDartGun GunSource { get; set; }
    public DartChain BeforeDartChain { get; set; }
    public DartChain AfterDartChain { get; set; }
    public DartChain _dartChainPrefab;

    [SerializeField] private float _crossSectionLength = 0.1f;
    [SerializeField] private AddTo AddCrosssectionsAt = AddTo.GunSide;
    [SerializeField] private float _timeToDie = 1f;

    private enum AddTo
    {
        DartSide,
        GunSide
    };

    public float CrossSectionLength
    {
        get { return _crossSectionLength; }
    }

    private Rigidbody2D _mainRigidbody;

    public Rigidbody2D MainRigidbody
    {
        get { return _mainRigidbody ?? (_mainRigidbody = GetComponent<Rigidbody2D>()); }
    }

    private HingeJoint2D _hingeJoint;
    public HingeJoint2D HingeJoint
    {
        get { return _hingeJoint ?? (_hingeJoint = GetComponent<HingeJoint2D>()); }
    }


    // Use this for initialization
    private void Start()
    {
        if (!MainRigidbody.isKinematic)
        {
            DartSource.DartDestroyed += DartSource_DartDestroyed;
        }
        else
        {
            DartSource = GetComponent<Dart>();
            GunSource = GetComponent<SappingDartGun>();
        }
    }

    void DartSource_DartDestroyed(object sender, System.EventArgs e)
    {
        if (_timeToDie <= 0f) Destroy(gameObject);
        else StartCoroutine(DestroyOnTimeout(_timeToDie));
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (!MainRigidbody.isKinematic && (BeforeDartChain.MainRigidbody.isKinematic || AfterDartChain.MainRigidbody.isKinematic))
	    {
	        switch (AddCrosssectionsAt)
	        {
	            case AddTo.GunSide:
	                var distance = Vector3.Distance(GunSource.transform.position, transform.position);
	                if (distance >= CrossSectionLength) InstantiateNewHose(BeforeDartChain, this);
	                break;
	            case AddTo.DartSide:
	                var distanceToDart = Vector3.Distance(DartSource.transform.position, transform.position);
                    if (distanceToDart >= CrossSectionLength) InstantiateNewHose(this, AfterDartChain);
	                break;
	            default:
	                Debug.LogError("Not implemented");
	                break;
	        }

	    }
	}

    private void InstantiateNewHose(DartChain beforeDartChain, DartChain afterDartChain)
    {
        var newHose = (DartChain) Instantiate(_dartChainPrefab,
            beforeDartChain.transform.position.Midpoint(afterDartChain.transform.position), transform.rotation);
        newHose.DartSource = DartSource;
        newHose.GunSource = GunSource;
        beforeDartChain.AddAfter(newHose);
        afterDartChain.AddBefore(newHose);
        Debug.Break();
    }

    public void AddAfter(DartChain dartChain, bool cascade = true)
    {
        //Check if we receive valid stuff
        if (AfterDartChain == null || (dartChain.GetInstanceID() != this.GetInstanceID()  && dartChain.GetInstanceID() != AfterDartChain.GetInstanceID()))
        {
            //If cascading and there is a node to cascade
            if (cascade)
            {
                dartChain.AddBefore(this, false);
            }
            if (!MainRigidbody.isKinematic)
            {
                HingeJoint.connectedBody = dartChain.MainRigidbody;
                AfterDartChain = dartChain;
            }

        }
        else
        {
            Debug.LogError("Trying to addSameNode or CurrentAfterNode");
        }
    }
    public void AddBefore(DartChain dartChain, bool cascade = true)
    {
        //Check if we receive valid stuff
        if (BeforeDartChain == null  ||( dartChain.GetInstanceID() != this.GetInstanceID() && dartChain.GetInstanceID() != BeforeDartChain.GetInstanceID()))
        {
            //If cascading and there is no node to cascade
            if (cascade)
            {
                dartChain.AddAfter(this, false);
            }
            if(!MainRigidbody.isKinematic) BeforeDartChain = dartChain;
            
            
        }
        else
        {
            Debug.LogError("Trying to addSameNode or CurrentBeforeNode");
        }
    }

    public void Reset()
    {
        BeforeDartChain = null;
        AfterDartChain = null;
    }

    IEnumerator DestroyOnTimeout(float timeout)
    {
        yield return new WaitForSeconds(timeout);
        Destroy(gameObject);
    }

}
