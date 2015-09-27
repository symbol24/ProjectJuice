using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : ExtendedMonobehaviour
{

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

    protected static GameState m_CurrentState;
    public GameState CurrentState { get { return m_CurrentState; } }

    protected static GameState m_PreviousState;
    public GameState PreviousState { get { return m_PreviousState; } }

    [SerializeField] private int m_AmountOfRoundsToWin = 5;
    public int AmountOfRoundsToWin { get { return m_AmountOfRoundsToWin; } }


    // Use this for initialization
    void Start()
    {
        
    }

    public void SubscribeToEndRound()
    {
        if (ScoreManager.instance != null)
            ScoreManager.instance.PlayerScored += CheckEndOfRound;
    }

    // Update is called once per frame
    void Update()
    {
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

    private void CheckEndOfRound(object sender, PlayerScoreEventArgs e)
    {
        
        if (e.IsThereAWinner) {
            if (CheckIfMatchWinner())
            {
                RoundMenu.instance.DisplayRoundMenu(true, true, e.Platformer2DUserControl.m_PlayerData.PlayerSponsor.SponsorName);
                m_CurrentState = GameState.MatchEnd;
            }
            else
            {
                RoundMenu.instance.DisplayRoundMenu(true, false, e.Platformer2DUserControl.m_PlayerData.PlayerSponsor.SponsorName);
                m_CurrentState = GameState.RoundEnd;
            }
        }
        else
        {
            RoundMenu.instance.DisplayRoundMenu(true, false, Database.instance.GameTexts[15]);
            m_CurrentState = GameState.RoundEnd;
        }

        
    }

    private bool CheckIfMatchWinner()
    {
        bool ret = false;

        foreach(PlayerScoreTracker score in ScoreManager.instance.PlayerScores)
        {
            if (score.CurrentScore == m_AmountOfRoundsToWin)
                ret = true;
        }

        return ret;
    }

    public void StartNextRound()
    {
        PlayerSpawner.instance.RespawnPlayers();
        m_CurrentState = GameState.Playing;
    }

    public void SetGameState(GameState newState)
    {
        m_CurrentState = newState;
    }
}
