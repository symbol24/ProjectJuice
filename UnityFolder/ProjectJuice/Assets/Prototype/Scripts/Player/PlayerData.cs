using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerData : MonoBehaviour {

    private static PlayerData _instance;

    public static PlayerData instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PlayerData>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public enum PlayerIDs { A, B, C, D}

    [SerializeField] private PlayerIDs m_PlayerID;
    public PlayerIDs playerID { get { return m_PlayerID; } set { m_PlayerID = value; } }

    private GamePad.Index m_PlayerGamepad;
    public GamePad.Index playerGameplad { get { return m_PlayerGamepad; } set { m_PlayerGamepad = value; } }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

}


