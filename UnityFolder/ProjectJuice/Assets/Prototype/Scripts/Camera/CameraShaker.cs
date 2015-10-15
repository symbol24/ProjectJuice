using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class CameraShaker : MonoBehaviour {

    public bool shakePosition;
    private bool shakeRotation = false;

    public float _shakeIntensity = 1f;
    public float shakeDecay = 0.02f;
    public float shakeTime = 1f;
    public float _mitigationOnAddition = 0.3f;
    public float _shakeFlatConstant = 2f;

    private bool isShakeRunning = false;


    public void DoShake()
    {
        var startingIntensity = Math.Abs(_currentIntensity) < 0.01f ? 0f : _currentIntensity;
        if (isShakeRunning)
        {
            StopCoroutine(_currentShake);
            startingIntensity += _shakeIntensity * _mitigationOnAddition;
            isShakeRunning = false;
        }
        else
        {
            _originalPosition = transform.position;
        }

        _currentShake = ProcessShake(_shakeIntensity, startingIntensity);
        StartCoroutine(_currentShake);
    }

    private IEnumerator _currentShake;
    private float _currentIntensity = 0f;
    private Vector3 _originalPosition;
    IEnumerator ProcessShake(float shakeIntensity, float startingIntensity)
    {
        Debug.Log(shakeIntensity + " " + startingIntensity);
        if (!isShakeRunning)
        {
            _currentIntensity = startingIntensity;
            isShakeRunning = true;
            //var originalPosition = transform.position;
            var originalRotation = transform.rotation;
            float currentShakeIntensity = _currentIntensity;
            var targetTime = Time.time + shakeTime;
            bool isIncreasing = true;
            while (Time.time < targetTime)
            {
                if (shakePosition)
                {
                    transform.position = _originalPosition + Random.insideUnitSphere * (AddPerlinNoiseTo(shakeIntensity) * currentShakeIntensity);
                }
                if (shakeRotation)
                {
                    transform.rotation = new Quaternion(originalRotation.x + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f,
                                                        originalRotation.y + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f,
                                                        originalRotation.z + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f,
                                                        originalRotation.w + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f);
                }
                if (isIncreasing)
                {
                    currentShakeIntensity += (startingIntensity/(targetTime - Time.time))*2;
                    isIncreasing = currentShakeIntensity < shakeIntensity;
                }
                else currentShakeIntensity -= (shakeIntensity/(targetTime - Time.time))*2;
                //currentShakeIntensity -= shakeDecay;
                yield return null;
            }
            transform.position = _originalPosition;
            transform.rotation = originalRotation;
            _currentIntensity = 0f;
            isShakeRunning = false;
        }
    }

    private float AddPerlinNoiseTo(float toAddNoise)
    {
        var ret = Mathf.PerlinNoise(toAddNoise, 0);
        return ret;
    }
}
