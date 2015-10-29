using UnityEngine;
using System.Collections;

public class RoundMenuSub : MonoBehaviour {
    [SerializeField] private RoundMenu m_parent;

    public void ResetGoUp()
    {
        m_parent.UpdateBoolAnimator("GoUp", false);
        m_parent.TriggerAfterGoUp();
    }

    public void ResetGoDown()
    {
        m_parent.UpdateBoolAnimator("GoDown", false);
    }

    public void StartOfGoUp()
    {
        m_parent.TriggerOnStartOfGoUp();
    }

}
