using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class CameraShaker : MonoBehaviour {

    public bool shakePosition;
    //private bool shakeRotation = false; //NotImplemented

        /*
    public float _shakeIntensity = 1.5f;
    public float shakeTime = 0.5f;
    public float _mitigationOnShakeAddition = 0.2f;
    public float _shakeFlatConstant = 2f;*/

    private bool isShakeRunning = false;


    public void DoShake(CameraShakeSettings settings)
    {
        var startingIntensity = Math.Abs(_currentIntensity) < 0.01f ? 0f : _currentIntensity;
        if (isShakeRunning)
        {
            StopCoroutine(_currentShake);
            startingIntensity += settings.ShakeIntensity* settings.MitigationOnShakeAddition;
            isShakeRunning = false;
        }
        else
        {
            _originalPosition = transform.position;
        }

        _currentShake = ProcessShake(settings.ShakeIntensity, startingIntensity + settings.ShakeIntensity, settings.ShakeTime);
        StartCoroutine(_currentShake);
    }

    private IEnumerator _currentShake;
    private float _currentIntensity = 0f;
    private Vector3 _originalPosition;
    IEnumerator ProcessShake(float shakeIntensity, float startingIntensity, float shakeTime)
    {
        if (startingIntensity == 0f) startingIntensity = 0.0001f;
        if (!isShakeRunning)
        {
            _currentIntensity = startingIntensity;
            isShakeRunning = true;
            var originalRotation = transform.rotation;
            float currentShakeIntensity = _currentIntensity;
            var targetTime = Time.time + shakeTime;
            bool isIncreasing = true;
            while (Time.time < targetTime)
            {
                Debug.Log(shakeIntensity + " " + currentShakeIntensity);
                if (shakePosition)
                {
                    transform.position = _originalPosition + Random.insideUnitSphere * (AddPerlinNoiseTo(shakeIntensity) * currentShakeIntensity);
                }
                /*if (shakeRotation)
                {
                    transform.rotation = new Quaternion(originalRotation.x + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f,
                                                        originalRotation.y + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f,
                                                        originalRotation.z + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f,
                                                        originalRotation.w + AddPerlinNoiseTo(Random.Range(-currentShakeIntensity, currentShakeIntensity)) * .2f);
                }*/
                if (isIncreasing)
                {
                    currentShakeIntensity += (startingIntensity/(shakeTime*1000))*2f;
                    isIncreasing = currentShakeIntensity < shakeIntensity;
                }
                else
                {
                    currentShakeIntensity -= (shakeIntensity/(shakeTime*1000))*2f;
                }
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
