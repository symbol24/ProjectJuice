using UnityEngine;
using System.Collections;

public class PurpleTimer : MonoBehaviour {
    [SerializeField] private Animator m_purpleAnimator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AnimationEnded()
    {
        m_purpleAnimator.SetBool("Play", false);
    }
}
