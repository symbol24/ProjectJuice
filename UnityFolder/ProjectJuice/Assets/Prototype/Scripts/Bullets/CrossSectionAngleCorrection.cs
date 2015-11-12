using UnityEngine;
using System.Collections;

public class CrossSectionAngleCorrection : ExtendedMonobehaviour
{
    public bool IsEdge
    {
        get
        {
            return _previousGameObject == null || _nextGameObject == null;
        }
    }
    public GameObject _previousGameObject;
    public GameObject _nextGameObject;
    public float _rotationCorrection = 90f;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (_previousGameObject != null && _nextGameObject != null)
        {
            var newAngle = GetAlphaFrom(-_nextGameObject.transform.position + _previousGameObject.transform.position);
            newAngle += _rotationCorrection;
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    private float GetAlphaFrom(Vector3 toCalculateAngle)
    {
        float ret;
        if (toCalculateAngle.x == 0f) ret = 90;
        else if (toCalculateAngle.y == 0f) ret = 0;
        else
        {
            var ratio = toCalculateAngle.y / toCalculateAngle.x;
            ret = Mathf.Atan(ratio) * Mathf.Rad2Deg;
        }
        return ret;
    }
	
}
