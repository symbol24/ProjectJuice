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


    public void Vibrate(VibrationSettings settings, bool addToExternalObject = false)
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
    public void StopVibration()
    {
        if(_vibrationOnGoing)
        {
            StopCoroutine(_vibrationCoroutine);
        }
    }

    private IEnumerator _vibrationCoroutine;
    private bool _vibrationOnGoing = false;
    private IEnumerator StartVibration(VibrationSettings settings)
    {
        _vibrationOnGoing = true;
        SetVibration(settings.LeftSideVibration, settings.RightSideVibration);
        yield return new WaitForSeconds(settings.TimeToVibrate);
        SetVibration(0, 0);
        _vibrationOnGoing = false;
    }



    private void SetVibration(float leftSide, float rightSide)
    {
        XInputDotNetPure.GamePad.SetVibration(_inputManager.m_PlayerData.GamepadIndex.ToPlayerIndex(), leftSide, rightSide);
    }

}
