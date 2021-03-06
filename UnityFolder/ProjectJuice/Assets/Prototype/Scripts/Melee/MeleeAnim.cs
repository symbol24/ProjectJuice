﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class MeleeAnim : MonoBehaviour {
    [SerializeField] private GameObject m_parent;
    [SerializeField] private MeleeAttack m_MeleeAbility;

    void Start()
    {
        if(!m_MeleeAbility.enabled)
             m_MeleeAbility = m_MeleeAbility.gameObject.GetComponents<MeleeAttack>().First(c => c.enabled);

    }

    public void EndAerial()
    {
        m_MeleeAbility.ResetSwing("Air", false);
    }

    public void EndGrounded()
    {
        m_MeleeAbility.ResetSwing("Grounded", false);
        OnAnimationGroundedFinished();
    }

    public void StartTrail()
    {
        m_MeleeAbility.StartTrail();
    }

    public event EventHandler AnimationGroundedFinished;
    protected virtual void OnAnimationGroundedFinished()
    {
        if (AnimationGroundedFinished != null) AnimationGroundedFinished(this, EventArgs.Empty);
    }
}
