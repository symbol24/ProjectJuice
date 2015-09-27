using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class PlayerData : MonoBehaviour {

    private static PlayerData _instance;

    public static PlayerData instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerData>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    [SerializeField] private PlayerIDs m_PlayerID;
    public PlayerIDs playerID { get { return m_PlayerID; } set { m_PlayerID = value; } }

    [SerializeField] private GamePad.Index m_PlayerGamepad;
    public GamePad.Index GamepadIndex { get { return m_PlayerGamepad; } set { m_PlayerGamepad = value; } }

    private bool m_PlayerActivated = false;
    public bool isActivated { get { return m_PlayerActivated; } set { m_PlayerActivated = value; } }

    [SerializeField] private Abilities m_PlayerAbility = Abilities.None;
    public Abilities PlayerAbility { get { return m_PlayerAbility; } set { m_PlayerAbility = value; } }

    [SerializeField] private Sponsor m_PlayerSponsor;
    public Sponsor PlayerSponsor { get { return m_PlayerSponsor; } set { m_PlayerSponsor = value; } }

    void Awake()
    {
        List<PlayerData> all = Utility.Utilities.GetAllPlayerData();
        int x = 0;
        foreach (PlayerData pd in all)
        {
            if (pd.GamepadIndex == m_PlayerGamepad)
                x++;
        }
        if (x > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void CheckActivated()
    {
        if(!m_PlayerActivated)
            Destroy(this.gameObject);
    }
}


