using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class ExtendedMonobehaviour : MonoBehaviour, IGameObject { 

    protected SFXGroup GetGroupWithName(string name)
    {
        SFXGroup ret = null;

        foreach(SFXGroup sfxg in SoundManager.Instance.sfxGroups)
        {
            if (sfxg.groupName == name)
                ret = sfxg;
        }

        return ret;
    }

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
        //if (!controller.m_FacingRight && xAxis > 0) xAxis = -xAxis; //This causes rotatation to stop when dashing left
        if (!controller.m_FacingRight && xAxis == 0 && yAxis == 0) xAxis = -1f;

        float zAngle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
        float xAngle = 0f;
        float yAngle = 0f;
        return new Vector3(xAngle, yAngle, zAngle);
    }

    protected float GetRotation(Vector3 center, Vector3 target)
    {
        var xAxis = center.x;
        var yAxis = target.y;
        var rotation = Mathf.Atan2(yAxis, xAxis)*Mathf.Rad2Deg;
        return rotation;
    }


    public void ChangeVisibilty(GameObject toChange)
    {
        if (toChange.activeInHierarchy) toChange.SetActive(true);
        else toChange.SetActive(false);
    }

    private readonly bool _legacyGetPointOfImpact = false;
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
            if (_legacyGetPointOfImpact)
            {
                for (int i = 0; i < raycastIterationsToFindTarget; i++)
                {
                    var firstTarget = new Vector3(othersPosition.x + raycastVariationPerTry*i,
                        othersPosition.y + raycastVariationPerTry*i, othersPosition.z);
                    hit = Physics2D.Raycast(sourceReference.position, firstTarget, float.MaxValue);
                    if (hit.collider == targetCollider) break;
                    var secondTarget = new Vector3(othersPosition.x - raycastVariationPerTry*i,
                        othersPosition.y - raycastVariationPerTry*i, othersPosition.z);
                    hit = Physics2D.Raycast(sourceReference.position, secondTarget, float.MaxValue);
                    if (hit.collider == targetCollider) break;
                }
                ret = hit.point;
            }
            else
            {
                RaycastHit2D[] hits;
                hits = Physics2D.RaycastAll(sourceReference.position, othersPosition,
                    Vector3.Distance(sourceReference.position, othersPosition));
                //Debug.DrawRay(sourceReference.position, othersPosition, Color.green);
                hit = hits.FirstOrDefault(c => c.collider == targetCollider);
                ret = hit.point;
                if (hit == default(RaycastHit2D))
                {
                    ret = sourceReference.transform.position;
                }
            }
        }
        return ret;
    }
    
    protected void InstatiateParticle(ParticleSystem toInstantiate, GameObject point, bool isToParent = false, float destroyTimer = 1f)
    {
        if (toInstantiate != null)
        {
            ParticleSystem temp = Instantiate<ParticleSystem>(toInstantiate);
            temp.transform.position = point.transform.position;
            temp.transform.rotation = point.transform.rotation;
            temp.Play();
            StartCoroutine(DestroyParticleEmitter(temp, destroyTimer));
            if (isToParent) temp.transform.SetParent(point.transform);
        }
        else
            Debug.LogWarning("Missing Particle");
    }

    private IEnumerator DestroyParticleEmitter(ParticleSystem toDestroy, float timer = 1f)
    {
        yield return new WaitForSeconds(timer);
        Destroy(toDestroy.gameObject);
    }

    protected Vector2 GetPointOfImpact(Transform targetReference, Transform sourceReference,
        int raycastIterationsToFindTarget = 5, float raycastVariationPerTry = 0.1f)
    {
        var ret = default(Vector2);
        var othersPosition = targetReference.gameObject.transform.position - sourceReference.position;
        RaycastHit2D hit = default(RaycastHit2D);
        if (_legacyGetPointOfImpact)
        {
            for (int i = 0; i < raycastIterationsToFindTarget; i++)
            {
                var firstTarget = new Vector3(othersPosition.x + raycastVariationPerTry * i,
                    othersPosition.y + raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(sourceReference.position, firstTarget, float.MaxValue);
                if (hit.transform == targetReference) break;
                var secondTarget = new Vector3(othersPosition.x - raycastVariationPerTry * i,
                    othersPosition.y - raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(sourceReference.position, secondTarget, float.MaxValue);
                if (hit.transform == targetReference) break;
            }
            ret = hit.point;
        }
        else
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(sourceReference.position, othersPosition,
                Vector3.Distance(sourceReference.position, othersPosition));
            //Debug.DrawRay(sourceReference.position, othersPosition, Color.green);
            hit = hits.FirstOrDefault(c => c.transform == targetReference);
            ret = hit.point;
            if (hit == default(RaycastHit2D))
            {
                ret = sourceReference.transform.position;
            }
        }
        return ret;
    }

}
