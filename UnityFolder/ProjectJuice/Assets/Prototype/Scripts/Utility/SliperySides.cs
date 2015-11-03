using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliperySides : ExtendedMonobehaviour {

    public List<Collider2D> SideColliders;
    public List<Collider2D> TopAndBottomColliders;

    [Range(0,10)]public float toleranceDegrees = 5;

    private enum ColliderMode { TopAndBottom, Sides, All};

    private void SetCollidersTo(List<Collider2D> collectionToUse, bool toSet)
    {
        foreach(var collider in collectionToUse)
        {
            collider.enabled = toSet;
        }
    }


    private ColliderMode _colliderMode;
    private ColliderMode Mode
    {
        get
        {
            return _colliderMode;
        }
        set
        {
            if(_colliderMode != value)
            {
                switch(value)
                {
                    case ColliderMode.All:
                        SetCollidersTo(SideColliders, true);
                        SetCollidersTo(TopAndBottomColliders, true);
                        break;
                    case ColliderMode.Sides:
                        SetCollidersTo(SideColliders, true);
                        SetCollidersTo(TopAndBottomColliders, false);
                        break;
                    case ColliderMode.TopAndBottom:
                        SetCollidersTo(SideColliders, false);
                        SetCollidersTo(TopAndBottomColliders, true);
                        break;

                }
                _colliderMode = value;
            }
        }
    }



    // Use this for initialization
    void Start()
    {
        _colliderMode = ColliderMode.All;
        Mode = ColliderMode.Sides;
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = To180Angle(transform.rotation.eulerAngles.z);
        if ((rotation > -toleranceDegrees && rotation < toleranceDegrees)||
            (rotation > 180 - toleranceDegrees || rotation < -180 + toleranceDegrees))
        {
            Mode = ColliderMode.Sides;
        }
        else if ((rotation > 90 - toleranceDegrees && rotation < 90 + toleranceDegrees) ||
            (rotation > -90 - toleranceDegrees && rotation < -90 + toleranceDegrees))
        {
            Mode = ColliderMode.TopAndBottom;
        }
        else
        {
            Mode = ColliderMode.All;
        }
        
    }



}
