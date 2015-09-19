using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    protected MenuControls m_Controls;

    private bool m_isActive = false;
    public bool isActive { get { return m_isActive; } set { m_isActive = value; } }


    // Use this for initialization
    protected virtual void Start () {

        m_Controls = FindObjectOfType<MenuControls>();
    }

    // Update is called once per frame
    protected virtual void Update () {
	
	}
}
