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

    [SerializeField] private bool ForVictorTesting = false;

    private RoundStartTimer m_startTimer;

    private SoundConnection mySoundConnection;

    public void InjectRoundStartTimer(RoundStartTimer startTimer)
    {
        m_startTimer = startTimer;
    }


    // Use this for initialization
    void Start()
    {
        if (ForVictorTesting)
            m_CurrentState = GameState.Playing;
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

    public void SetPaused(bool isPaused, float volume = 1, bool isFullSoundPause = false)
    {
        if (isPaused)
        {
            TogglePauseSound(volume, isFullSoundPause);
            m_PreviousState = m_CurrentState;
            m_CurrentState = GameState.Paused;
            Time.timeScale = 0f;
        }
        else
        {
            TogglePauseSound(volume, isFullSoundPause);
            m_CurrentState = m_PreviousState;
            PlayerSpawner.instance.FlushAlivePlayerInputs();
            Time.timeScale = 1f;
        }
    }

    private void TogglePauseSound(float volume = 1, bool isFull = false)
    {
        if (isFull)
            SoundManager.PauseToggle();
        else
            SoundManager.SetVolumeMusic(volume);
    }

    public bool IsPlaying
    {
        get { return m_CurrentState == GameState.Playing; }
    }

    public bool IsPaused { get { return m_CurrentState == GameState.Paused; } }

    public bool IsCharacterSelect { get { return m_CurrentState == GameState.CharacterSelect; } }

    private void CheckEndOfRound(object sender, PlayerScoreEventArgs e)
    {
        
        if (e.IsThereAWinner) {
            if (CheckIfMatchWinner())
            {
                RoundMenu.instance.DisplayRoundMenu(true, true, e.Platformer2DUserControl.m_PlayerData.PlayerSponsor.SponsorName, GameState.MatchEnd);
                m_CurrentState = GameState.PreMatchEnd;
            }
            else
            {
                RoundMenu.instance.DisplayRoundMenu(true, false, e.Platformer2DUserControl.m_PlayerData.PlayerSponsor.SponsorName, GameState.RoundEnd);
                m_CurrentState = GameState.PreRoundEnd;
            }
        }
        else
        {
            RoundMenu.instance.DisplayRoundMenu(true, false, Database.instance.GameTexts[15], GameState.RoundEnd);
            m_CurrentState = GameState.PreRoundEnd;
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

    public void DisplayStartTimer()
    {
        if (m_startTimer != null)
        {
            m_startTimer.gameObject.SetActive(true);
            m_startTimer.Reset();
        }
    }

    public void StartNextRound()
    {
        if (m_startTimer != null)
        {
            m_startTimer.gameObject.SetActive(true);
            m_startTimer.Reset();
        }
        PlayerSpawner.instance.RespawnPlayers();
    }

    public void SetGameState(GameState newState)
    {
        m_CurrentState = newState;
    }
}
