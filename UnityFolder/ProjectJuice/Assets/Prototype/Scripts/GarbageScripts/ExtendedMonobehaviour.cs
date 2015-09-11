using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class ExtendedMonobehaviour : MonoBehaviour, IGameObject {

    protected void SetRotationEulerX(float xValue)
    {
        transform.rotation = Quaternion.Euler(xValue, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    protected void SetRotationEulerY(float yValue)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yValue, transform.rotation.eulerAngles.z);
    }

    protected void SetRotationEulerXY(float xValue, float yValue)
    {
        transform.rotation = Quaternion.Euler(xValue, yValue, transform.rotation.eulerAngles.z);
    }

    protected void SetRotationEulerZ(float zValue)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, zValue);
    }

    protected bool IsAChild(IDamaging toCheck)
    {
        var checkComponents = GetComponentsInChildren<IDamaging>();
        return checkComponents.Any(c => c == toCheck);
    }

    protected IEnumerator DashDragReset(float timer, Rigidbody2D toCheckDrag)
    {
        yield return new WaitForSeconds(timer);
        CheckDrag(toCheckDrag);
    }
    private void CheckDrag(Rigidbody2D toCheck)
    {
        if (toCheck.drag != 0) toCheck.drag = 0;
    }

    protected IEnumerator PrintObject(Rigidbody2D toPrint, float timer)
    {
        float currentTimer = 0;
        while(currentTimer < timer)
        {
            print(toPrint.velocity);
            currentTimer += Time.deltaTime;
            yield return null;
        }
    }
}
