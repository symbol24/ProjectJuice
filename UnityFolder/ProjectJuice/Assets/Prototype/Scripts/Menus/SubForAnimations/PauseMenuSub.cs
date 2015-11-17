using UnityEngine;
using System.Collections;

public class PauseMenuSub : MonoBehaviour {
    [SerializeField] private PauseMenu m_parentPauseMenu;

    public void ResetGoUp()
    {
        m_parentPauseMenu.ResetAnimatorBool("GoUp", false);
        m_parentPauseMenu.ChangePanelState();
    }

    public void ResetGoDown()
    {
        m_parentPauseMenu.ResetAnimatorBool("GoDown", false);
        m_parentPauseMenu.SetAnimState(MenuAnimState.Usable);
    }
}
