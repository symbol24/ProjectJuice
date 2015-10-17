using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineHolder : MonoBehaviour {
    private List<IEnumerator> _coroutinesToHold;
    private List<IEnumerator> CoroutinesToHold
    {
        get
        {
            if (_coroutinesToHold == null) _coroutinesToHold = new List<IEnumerator>();
            return _coroutinesToHold;
        }
    }
    public void StartAndKeepCoroutine(IEnumerator toKeep)
    {
        StartCoroutine(toKeep);
        CoroutinesToHold.Add(toKeep);
    }
}
