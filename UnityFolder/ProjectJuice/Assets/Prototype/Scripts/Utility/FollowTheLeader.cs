using UnityEngine;
using System.Collections;

public class FollowTheLeader : MonoBehaviour {
    public GameObject _toFollow;

	
	// Update is called once per frame
	void Update () {
        if (_toFollow != null)
            transform.position = _toFollow.transform.position;
	}
}
