using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : ExtendedMonobehaviour {

    #region singleton Stuffs
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    #endregion

    [SerializeField] private PlayerSpawner m_SpawnManager;

    protected static GameState m_CurrentState;
    public GameState CurrentState { get { return m_CurrentState; } }

    protected static GameState m_PreviousState;
    public GameState PreviousState { get { return m_PreviousState; } }


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPaused(bool isPaused)
    {
        if (isPaused)
        {
            m_PreviousState = m_CurrentState;
            m_CurrentState = GameState.Paused;
            Time.timeScale = 0f;
        }
        else
        {
            m_CurrentState = m_PreviousState;
            Time.timeScale = 1f;
        }
    }

    public bool CheckIfPlaying()
    {
        return m_CurrentState == GameState.Playing;
    }
}
