using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class ChangeToPlayerColor : MonoBehaviour
{
    //[SerializeField] private PlayerData _playerData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Platformer2DUserControl _platformer2DUserControl;
    [SerializeField] private IPlatformer2DUserControl _character;
    private float _originalAlpha;

    public IPlatformer2DUserControl Character
    {
        get
        {
            return _character;
        }
        set
        {
            if (value != null)
            {
                if (value.m_PlayerData == null) StartCoroutine(DelaySetterAFrame(value));
                else if (value.m_PlayerData != null ||
                         _character.m_PlayerData.GetInstanceID() != value.m_PlayerData.GetInstanceID())
                {
                    if (_character != value)
                    {
                        _character = value;
                        ChangeColour();
                    }
                }
            }
            else
            {
                Debug.LogError("Trying to assign Null");
            }
        }
    }

    private void ChangeColour()
    {
        Color touse = _character.m_PlayerData.PlayerSponsor.SponsorColor;
        if (_originalAlpha != _character.m_PlayerData.PlayerSponsor.SponsorColor.a)
            touse.a = _originalAlpha;

        _spriteRenderer.color = touse;
    }

    // Use this for initialization
	void Start ()
	{
	    if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
	    if (_spriteRenderer == null) Debug.LogError("SpriteRenderer not found");

	    if (_platformer2DUserControl != null) Character = _platformer2DUserControl;
        if (_spriteRenderer != null) _originalAlpha = _spriteRenderer.color.a;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    IEnumerator DelaySetterAFrame(IPlatformer2DUserControl controller)
    {
        //yield return new WaitForEndOfFrame();
        yield return null;
        Character = controller;
    }
}
