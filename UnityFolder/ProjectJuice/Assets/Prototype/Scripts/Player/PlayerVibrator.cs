using UnityEngine;
using System.Collections;

public class PlayerVibrator : MonoBehaviour {

    [SerializeField]
    private IPlatformer2DUserControl _inputManager;

	// Use this for initialization
	void Start () {
        _inputManager = GetComponent<IPlatformer2DUserControl>();
        if (_inputManager == null) Debug.LogWarning("No Platformer2dUserControl Detected!");
	}
    private bool _needToContinueVibration = false;
    void Update()
    {
        if(GameManager.instance.CurrentState != GameState.Playing && _vibrationOnGoing)
        {
            StopCoroutine(_vibrationCoroutine);
            _vibrationOnGoing = false;
            StopVibration();
            _needToContinueVibration = GameManager.instance.CurrentState == GameState.Paused;
        }
        else if(_needToContinueVibration && GameManager.instance.CurrentState == GameState.Playing)
        {
            StartVibration(_settingsOnGoing, _currentTimePassed);
            _needToContinueVibration = false;
        }
    }


    public void Vibrate(VibrationSettings settings, bool addToExternalObject = false)
    {
        if (settings.LeftSideVibration >= _settingsOnGoing.LeftSideVibration || settings.RightSideVibration >= _settingsOnGoing.RightSideVibration)
        {
            StopVibration();
            _vibrationCoroutine = StartVibration(settings);
            if (addToExternalObject)
            {
                var newGameObject = new GameObject();
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
    }
    public void StopVibration()
    {
        if(_vibrationOnGoing)
        {
            StopCoroutine(_vibrationCoroutine);
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



    private void SetVibration(float leftSide, float rightSide)
    {
        XInputDotNetPure.GamePad.SetVibration(_inputManager.m_PlayerData.GamepadIndex.ToPlayerIndex(), leftSide, rightSide);
    }

}
