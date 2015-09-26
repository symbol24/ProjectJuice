using UnityEngine;
using System.Collections;

public class Sponsor : MonoBehaviour {

    //public enum Sponsors { One, Two, Three, Four, Five, Six }

    [SerializeField] private string m_SponsorName;
    public string SponsorName { get { return m_SponsorName; } }

    [SerializeField] private Color m_SponsorColor;
    public Color SponsorColor { get { return m_SponsorColor; } }

    //[SerializeField] private Sponsors m_SponsorID;
    //public Sponsors ID { get { return m_SponsorID; } }

    private bool m_IsTaken = false;
    public bool isTaken { get { return m_IsTaken; } }

    public void TakeSponsor()
    {
        if (!m_IsTaken)
            m_IsTaken = true;
    }
}
