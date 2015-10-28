using UnityEngine;
using System.Collections;

public class ScaleBasedOnHpWhiteBar : ScaleBasedOnHP
{
    [SerializeField]
    private float _firstWaitSeconds;
    [SerializeField]
    private float _loweringWaitSeconds;
    protected override void HpScriptOnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        float newScalePercent = _originalX * (_hpScript.CurrentHp / _hpScript.MaxHp);
        if(_slowlyDecreaseBarRunning)
        {
            StopCoroutine(_slowlyDecreaseBarRoutine);
            _slowlyDecreaseBarRunning = false;
        }
        _slowlyDecreaseBarRoutine = SlowlyDecreaseBar(newScalePercent);
        StartCoroutine(_slowlyDecreaseBarRoutine);
    }

    private bool _slowlyDecreaseBarRunning = false;
    private IEnumerator _slowlyDecreaseBarRoutine = null;
    private IEnumerator SlowlyDecreaseBar(float newScalePercent)
    {
        _slowlyDecreaseBarRunning = true;
        yield return new WaitForSeconds(_firstWaitSeconds);
        var chunk = (_gameObjectToScale.transform.localScale.x - newScalePercent) * Time.fixedDeltaTime / _loweringWaitSeconds;
        while(newScalePercent < _gameObjectToScale.transform.localScale.x)
        {
            _gameObjectToScale.transform.localScale = _gameObjectToScale.transform.localScale.SetX(_gameObjectToScale.transform.localScale.x - chunk);
            yield return new WaitForFixedUpdate();
        }


        /*_gameObjectToScale.transform.localScale =
            _gameObjectToScale.transform.localScale.SetX(newScalePercent);*/

        _slowlyDecreaseBarRunning = false;
    }
}
