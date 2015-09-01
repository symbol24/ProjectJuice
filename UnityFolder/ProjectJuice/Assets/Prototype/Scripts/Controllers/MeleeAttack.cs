using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class MeleeAttack : ExtendedMonobehaviour
{
    [SerializeField] 
    private Platformer2DUserControl _inputManager;
    [SerializeField] 
    private GameObject _swingerCollider;
    [SerializeField] 
    private Transform _swingerColliderInitialRotation;
    [SerializeField]
    private Transform _swingerColliderFinalRotation;

    public float rotationSpeed = 100f;
    public float startingRotation = -45;
    public float endingRotation = 45;

    // Use this for initialization
    private void Start()
    {
        if (_inputManager == null) _inputManager = GetComponent<Platformer2DUserControl>();
        SetRotationEulerZ(startingRotation);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_swingerCollider.transform.rotation.eulerAngles.z <= endingRotation)
        {
            SetRotationEulerZ(startingRotation);
        }
        else
        {
            _swingerCollider.transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
        }
        
    }



}
