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
        GameManager[] list = FindObjectsOfType<GameManager>();
        if (list.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }
    #endregion

    protected static GameState m_CurrentState;
    public GameState CurrentState { get { return m_CurrentState; } }

    protected static GameState m_PreviousState;
    public GameState PreviousState { get { return m_PreviousState; } }

    private RoundStartTimer m_startTimer;

    private SoundConnection mySoundConnection;

    public void InjectRoundStartTimer(RoundStartTimer startTimer)
    {
        m_startTimer = startTimer;
    }

    private bool _roundEnded = false;


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

    public void SetPaused(bool isPaused, float volume = 1, bool isFullSoundPause = false)
    {
        if (isPaused)
        {
            PauseUnpauseSFX(isPaused);
            TogglePauseSound(volume, isFullSoundPause);
            m_PreviousState = m_CurrentState;
            m_CurrentState = GameState.Paused;
            Time.timeScale = 0f;
        }
        else
        {
            PauseUnpauseSFX(isPaused);
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

    public bool IsLoading { get { return m_CurrentState == GameState.Loading; } }

    public bool IsPaused { get { return m_CurrentState == GameState.Paused; } }

    public bool IsCharacterSelect { get { return m_CurrentState == GameState.CharacterSelect; } }

    private void CheckEndOfRound(object sender, PlayerScoreEventArgs e)
    {
        if (!_roundEnded)
        {
            _roundEnded = true;
            ClearRoundSFX();
            if (e.IsThereAWinner)
            {
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
        
    }

    private void ClearRoundSFX()
    {
        AudioSource[] list = FindObjectsOfType<AudioSource>();
        foreach(AudioSource As in list)
        {
            if (As.isPlaying && As.loop) As.Stop();
        }
    }

    private void PauseUnpauseSFX(bool isPaused = true)
    {
        AudioSource[] list = FindObjectsOfType<AudioSource>();
        foreach (AudioSource As in list)
        {
            if (As.isPlaying && As.loop)
            {
                if (isPaused)
                    As.Pause();
                else
                    As.UnPause();
            }
        }

    }

    private bool CheckIfMatchWinner()
    {
        bool ret = false;

        foreach(PlayerScoreTracker score in ScoreManager.instance.PlayerScores)
        {
            if (score.CurrentScore == Database.instance.AmountOfRounds)
                ret = true;
        }

        return ret;
    }

    public void DisplayStartTimer()
    {
        if (_roundEnded) _roundEnded = false;
        if (m_startTimer != null)
        {
            m_startTimer.gameObject.SetActive(true);
            m_startTimer.Reset();
        }
    }

    public void StartNextRound()
    {
        _roundEnded = false;
        if (m_startTimer != null)
        {
            m_startTimer.gameObject.SetActive(true);
            m_startTimer.Reset();
        }
        PlayerSpawner.instance.RespawnPlayers();
        EnvironmentalObjectsManager.Instance.RespawnEnvironment();
    }

    public void SetGameState(GameState newState)
    {
        m_CurrentState = newState;
        //print("Loaded level " + Application.loadedLevel + " new GameState " + m_CurrentState);
    }

    public void SetRoundEnd(bool toValue = false)
    {
        _roundEnded = toValue;
    }
}
