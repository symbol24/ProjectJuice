using UnityEngine;
using System.Collections;

public class Sponsor : MonoBehaviour {

    //public enum Sponsors { One, Two, Three, Four, Five, Six }

    [SerializeField] private string m_SponsorName;
    public string SponsorName { get { return m_SponsorName; } }

    [SerializeField] private Color m_SponsorColor;
    public Color SponsorColor { get { return m_SponsorColor; } }

    [SerializeField] private Sprite m_SponsorImage;
    public Sprite SponsorImage { get { return m_SponsorImage; } }

    [SerializeField] private Material m_SponsorMaterial;
    public Material SponsorMaterial { get { return m_SponsorMaterial; } }

    //[SerializeField] private Sponsors m_SponsorID;
    //public Sponsors ID { get { return m_SponsorID; } }

    private bool m_IsTaken = false;
    public bool isTaken { get { return m_IsTaken; } }

    public void TakeSponsor()
    {
        if (!m_IsTaken)
            m_IsTaken = true;
    }

    public void ReleaseSponsor()
    {
        m_IsTaken = false;
    }
}
