using UnityEngine;
using System.Collections;

public class RoundMenuSub : MonoBehaviour {
    [SerializeField] private RoundMenu m_parent;

    public void ResetGoUp()
    {
        m_parent.UpdateBoolAnimator("GoUp", false);
        m_parent.SetDisplayBool(false);
        m_parent.TriggerAfterGoUp();
    }

    public void ResetGoDown()
    {
        m_parent.UpdateBoolAnimator("GoDown", false);
        m_parent.SetDisplayBool(true);
    }

    public void StartOfGoUp()
    {
        m_parent.TriggerOnStartOfGoUp();
    }

}
