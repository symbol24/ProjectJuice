using UnityEngine;
using System.Collections;

public class MeleeAnim : MonoBehaviour {
    [SerializeField] private GameObject m_parent;
    private MeleeAttack m_MeleeAbility;

    void Start()
    {
        if (m_parent == null) Debug.LogWarning("Missing Parent GameObject on MeleeAnim!");
        else
        {
            MeleeAttack[] list = m_parent.GetComponents<MeleeAttack>();
            foreach(MeleeAttack ma in list)
            {
                if (ma.isAbility) m_MeleeAbility = ma;
            }
            if (m_MeleeAbility == null) Debug.LogWarning("Missing Ability MeleeAttack on Parent GameObject!");
        }
        
    }

    public void EndAerial()
    {
        m_MeleeAbility.ResetSwing("Air", false);
    }

    public void EndGrounded()
    {
        m_MeleeAbility.ResetSwing("Grounded", false);
    }

    public void StartTrail()
    {
        m_MeleeAbility.StartTrail();
    }

	
}
