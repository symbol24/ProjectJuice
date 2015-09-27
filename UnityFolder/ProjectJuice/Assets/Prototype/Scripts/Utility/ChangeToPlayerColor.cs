using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class ChangeToPlayerColor : MonoBehaviour
{
    //[SerializeField] private PlayerData _playerData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Platformer2DUserControl _platformer2DUserControl;
    [SerializeField] private IPlatformer2DUserControl _character;

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
                if (_character.m_PlayerData == null || _character.m_PlayerData.GetInstanceID() != value.m_PlayerData.GetInstanceID())
                {
                    _character = value;
                    ChangeColour();
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
        _spriteRenderer.color = _character.m_PlayerData.PlayerSponsor.SponsorColor;
    }

    // Use this for initialization
	void Start ()
	{
	    if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
	    if (_spriteRenderer == null) Debug.LogError("SpriteRenderer not found");

	    if (_platformer2DUserControl != null) _character = _platformer2DUserControl;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
