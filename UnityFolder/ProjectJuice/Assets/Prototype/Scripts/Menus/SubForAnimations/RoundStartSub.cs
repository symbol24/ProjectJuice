using UnityEngine;
using System.Collections;

public class RoundStartSub : MonoBehaviour {
    [SerializeField] private RoundStartTimer _parent;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("RoundStartSub can't find the animator on itself.");
    }
	
    public void ResetEnter()
    {
        _parent.SetBool(_animator, "Enter", false);
        _parent.StartTimer();
    }

    public void ResetExit()
    {
        _parent.SetBool(_animator, "Exit", false);
        _parent.Deactivate();
    }
}
