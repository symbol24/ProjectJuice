using UnityEngine;
using System.Collections;

public abstract class CameraShakeBase<T> : ExtendedMonobehaviour where T : Component
{

    [SerializeField] protected T _componentToListenTo;
    [SerializeField] protected CameraShaker _cameraShaker;
    // Use this for initialization
    protected virtual void Start()
    {
        var test = Camera.main.gameObject.GetComponent<CameraShaker>();
        if (test == null) Camera.main.gameObject.AddComponent<CameraShaker>();

        if (_cameraShaker == null) _cameraShaker = FindObjectOfType<CameraShaker>();
        if (_cameraShaker == null) Debug.LogWarning("NoCameraShakerFound (CameraShake on MainCamera)");
        if (_componentToListenTo == null) _componentToListenTo = GetComponent<T>();
        if (_componentToListenTo == null) Debug.LogWarning("CannotFindComponent");
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
