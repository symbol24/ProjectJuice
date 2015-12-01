using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using GamepadInput;

public class PlayerVibrator : MonoBehaviour
{

    [SerializeField]
    private IPlatformer2DUserControl _inputManager;

    private string _guidIdentifierForExternalObject;

    void Start()
    {
        _inputManager = GetComponent<IPlatformer2DUserControl>();
        if (_inputManager == null) Debug.LogWarning("No Platformer2dUserControl Detected!");
        _guidIdentifierForExternalObject = Guid.NewGuid().ToString();
    }
    private bool _needToContinueVibration = false;
    void Update()
    {
        if (GameManager.instance.CurrentState != GameState.Playing && _vibrationOnGoing)
        {
            StopCoroutine(_vibrationCoroutine);
            _vibrationOnGoing = false;
            StopVibration();
            _needToContinueVibration = GameManager.instance.CurrentState == GameState.Paused;
        }
        else if (_needToContinueVibration && GameManager.instance.CurrentState == GameState.Playing)
        {
            StartVibration(_settingsOnGoing, _currentTimePassed);
            _needToContinueVibration = false;
            _settingsOnGoing = null;
        }
    }


    public void Vibrate(VibrationSettings settings, bool addToExternalObject = false)
    {
        if (GameManager.instance.CurrentState != GameState.Playing) return;
        //this check was gining a nice null pointer, so I added a null check on _settingsOnGoing, but now there noe more vibration, sorry Victor...
        //if (_settingsOnGoing != null && (settings.LeftSideVibration >= _settingsOnGoing.LeftSideVibration || settings.RightSideVibration >= _settingsOnGoing.RightSideVibration))
        if (_vibrationOnGoing && _settingsOnGoing != null)
        {
            if (settings.LeftSideVibration <= _settingsOnGoing.LeftSideVibration || settings.RightSideVibration <= _settingsOnGoing.RightSideVibration)
            {
                return;
            }
        }
        StopVibration();
        _vibrationCoroutine = StartVibration(settings);
        if (addToExternalObject)
        {
            var newGameObject = new GameObject() { name = _guidIdentifierForExternalObject };
            var holder = newGameObject.AddComponent<CoroutineHolder>();
            var destroyOnTimer = newGameObject.AddComponent<DestroyOnTimer>();
            holder.StartAndKeepCoroutine(_vibrationCoroutine);
            destroyOnTimer.Timeout = settings.TimeToVibrate + 0.3f;
        }
        else
        {
            StartCoroutine(_vibrationCoroutine);
        }

    }
    public void StopVibration()
    {
        if (_vibrationOnGoing)
        {
            StopCoroutine(_vibrationCoroutine);
            _vibrationOnGoing = false;
        }
        SetVibration(0, 0);
    }
    public bool IsVibrating
    {
        get
        {
            return _vibrationOnGoing;
        }
    }

    private IEnumerator _vibrationCoroutine;
    private bool _vibrationOnGoing = false;
    private VibrationSettings _settingsOnGoing;
    private float _currentTimePassed;
    private IEnumerator StartVibration(VibrationSettings settings, float startTime = 0)
    {
        _vibrationOnGoing = true;
        _settingsOnGoing = settings;
        _currentTimePassed = startTime;
        SetVibration(settings.LeftSideVibration, settings.RightSideVibration);
        while (_currentTimePassed <= settings.TimeToVibrate)
        {
            _currentTimePassed += Time.deltaTime;
            yield return null;
        }
        SetVibration(0, 0);
        _vibrationOnGoing = false;
    }



    private GamePad.Index _gamepadIndex = GamePad.Index.Any;
    private void SetVibration(float leftSide, float rightSide)
    {
        _gamepadIndex = _inputManager.m_PlayerData.GamepadIndex;
        XInputDotNetPure.GamePad.SetVibration(_gamepadIndex.ToPlayerIndex(), leftSide, rightSide);
    }

    void OnDestroy()
    {
        if (_gamepadIndex != GamePad.Index.Any)
        {
            if (!FindObjectsOfType<CoroutineHolder>().Any(c => c.gameObject.name == _guidIdentifierForExternalObject))
            {
                XInputDotNetPure.GamePad.SetVibration(_gamepadIndex.ToPlayerIndex(), 0, 0);
            }
        }
    }


}
