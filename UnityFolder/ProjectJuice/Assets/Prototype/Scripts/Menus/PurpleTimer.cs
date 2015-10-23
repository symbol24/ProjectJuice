using UnityEngine;
using System.Collections;

public class PurpleTimer : MonoBehaviour {
    [SerializeField] private Animator m_purpleAnimator;
    private RoundStartTimer m_roundMenu;
    private int m_animCounter = 0;

	// Use this for initialization
	void Start () {
        m_roundMenu = FindObjectOfType<RoundStartTimer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AnimationEnded()
    {
        if (m_animCounter == m_roundMenu.AmountOfSeconds)
            SoundManager.PlaySFX(m_roundMenu.GoClipName);
        else
            SoundManager.PlaySFX(m_roundMenu.NumberClipName);
        m_purpleAnimator.SetBool("Play", false);
        m_animCounter++;
        if (m_animCounter > m_roundMenu.AmountOfSeconds) ;
    }
}
