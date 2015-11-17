using UnityEngine;
using System.Collections;

public abstract class VibrationBase<T> : ExtendedMonobehaviour where T : IGameObject
{
    [SerializeField] protected T _componentToListen;
    [SerializeField] protected PlayerVibrator _playerVibrator;
	// Use this for initialization
    protected virtual void Start()
    {
        if (_playerVibrator == null) _playerVibrator = GetComponent<PlayerVibrator>();
        if (_playerVibrator == null) Debug.LogWarning("No vibrator found, where is my vibrator? :(");
        if (_componentToListen == null) _componentToListen = GetComponent<T>();
        if (_componentToListen == null) Debug.LogWarning("No component Found");
    }
}
