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

    protected Vector3 GetRotation(IPlatformer2DUserControl controller)
    {
        float xAxis = controller.m_XAxis;
        float yAxis = controller.m_YAxis;
        if (!controller.m_FacingRight && xAxis > 0) xAxis = -xAxis;
        if (!controller.m_FacingRight && xAxis == 0 && yAxis == 0) xAxis = -1f;

        float zAngle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
        float xAngle = 0f;
        float yAngle = 0f;
        return new Vector3(xAngle, yAngle, zAngle);
    }


    public void ChangeVisibilty(GameObject toChange)
    {
        if (toChange.activeInHierarchy) toChange.SetActive(true);
        else toChange.SetActive(false);
    }

    protected Vector2 GetPointOfImpact(IDamaging chkDamaging, Collider2D targetCollider,Transform sourceReference, int raycastIterationsToFindTarget = 5, float raycastVariationPerTry = 0.1f)
    {
        Vector2 ret = new Vector2();
        var othersPosition = targetCollider.gameObject.transform.position - sourceReference.position;
        RaycastHit2D hit = default(RaycastHit2D);
        if (chkDamaging.HasPreferredImpactPoint)
        {
            ret = chkDamaging.PreferredImpactPoint;
        }
        else
        {
            for (int i = 0; i < raycastIterationsToFindTarget; i++)
            {
                var firstTarget = new Vector3(othersPosition.x + raycastVariationPerTry * i,
                    othersPosition.y + raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(sourceReference.position, firstTarget, float.MaxValue);
                if (hit.collider == targetCollider) break;
                //Testing where raycast went
                //Debug.DrawRay(_centerOfReferenceForJuice.position, firstTarget, Color.green);
                //EditorApplication.isPaused = true;
                var secondTarget = new Vector3(othersPosition.x - raycastVariationPerTry * i,
                    othersPosition.y - raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(sourceReference.position, secondTarget, float.MaxValue);
                if (hit.collider == targetCollider) break;
                //Debug.DrawRay(_centerOfReferenceForJuice.position, secondTarget, Color.red);
            }
            ret = hit.point;
        }


        return ret;
    }

    
}
