﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoundMenu : Menu {

    #region instance Stuffs
    private static RoundMenu _instance;

    public static RoundMenu instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<RoundMenu>();

            return _instance;
        }
    }
    #endregion
    
    [SerializeField] GameObject m_RoundPanel;
    private DelayManager m_DelayManager;

    [Range(0,20)][SerializeField] private float m_DelayBeforeDisplay = 0.5f;
    [Range(0,3)][SerializeField] private float m_DelayForRtuenToCharSel = 0.5f;
    [Range(0,5)][SerializeField] private float m_DelayToDisplayStar = 0.5f;
    [Range(0,3)][SerializeField] private float _delayBeforeFlashing = 0.5f;
    [Range(0,10)][SerializeField] private int _circleFlashCount = 5;
    [Range(0,1)][SerializeField] private float _flashDelay = 0.3f;

    private bool _canDisplayWinner = false;
    private Image _winnerImage;

    [SerializeField] List<Sprite> m_Circles;
    [SerializeField] List<GameObject> m_ListOfPlayerLines;
    [SerializeField] GameObject m_CirclePrefab;

    [SerializeField] Text m_TopText;
    [SerializeField] GameObject m_RoundMenu;
    [SerializeField] GameObject m_MatchMenu;

    [SerializeField] private Button[] m_ListOfButtones;
    private int m_currentSelection = 0;
    private int m_maxSelection = 0;
    private bool m_inConfirm = false;
    private bool m_playingSpecialBGM = false;
    [SerializeField] private bool m_playSpecialBGM = false;
    [Range(0,1)][SerializeField] private float m_BGMVolume = 0.2f;
    [SerializeField] private AudioClip m_SpecialBGM;
    private SoundConnection m_myConnection;
    private int m_lastController = 0;

    [SerializeField] private Animator m_animator;
    private bool m_isMatchOver = false;
    private int m_targetLevel = -1;

    [HideInInspector] public int VictoryID;
    [HideInInspector] public string VictoryName;
    [HideInInspector] public int DrawID;
    [HideInInspector] public string DrawName;
    [HideInInspector] public string MenuOpen;
    [HideInInspector] public string MenuClose;
    [HideInInspector] public string ReturnToCS;
    [HideInInspector] public string MedalAppear;
    [HideInInspector] public string MedalCrowd;
    [HideInInspector] public string RoundPhaseOut;
    [HideInInspector] public string MatchPhaseIn;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_maxSelection = GetListOfActiveButtons();
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
        m_myConnection = SoundManager.GetCurrentSoundConnection();
        m_RoundMenu.SetActive(false);
        m_MatchMenu.SetActive(false);
        m_RoundPanel.SetActive(false);
    }

    private int GetListOfActiveButtons()
    {
        int ret = 0;

        foreach (Button b in m_ListOfButtones) if (b.IsActive()) ret++;

        return ret;
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        if (GameManager.instance.CurrentState == GameState.RoundEnd)
        {
            for (int i = 0; i < PlayerSpawner.instance.ListOfPlayerDatas.Count; i++)
            {
                if (m_RoundMenu.activeInHierarchy && m_DelayManager.CanShoot && (m_Controls.Confirm[i] || m_Controls._Start[i]))
                {
                    SoundManager.PlaySFX(Database.instance.MenuClickName);
                    GoToNextRound();
                }
            }
        }
        else if (GameManager.instance.CurrentState == GameState.MatchEnd)
        {
            for (int i = 0; i < PlayerSpawner.instance.ListOfPlayerDatas.Count; i++)
            {
                if (m_MatchMenu.activeInHierarchy)
                {
                    if (m_DelayManager.CanShield && !m_inConfirm && m_Controls.X[i] != 0)
                    {
                        SetNextActive(m_Controls.X[i]);
                    }

                    if (m_DelayManager.CanShoot && m_Controls.Confirm[i])
                    {
                        RoundActivate();
                    }

                    if (m_DelayManager.CanShoot && m_inConfirm && m_Controls.Cancel[i])
                    {
                        CancelConfirm();
                    }
                }
            }
        }
	}

    public void DisplayRoundMenu(bool active, bool isMatchOver, string winnerName, GameState nextGameState)
    {
        m_isMatchOver = isMatchOver;

        if (active)
            StartCoroutine(DelayBeforeActive(active, m_DelayBeforeDisplay, isMatchOver, winnerName, nextGameState));
        else
        {
            m_RoundPanel.SetActive(active);
        }
    }

    private IEnumerator DelayBeforeActive(bool active, float timer, bool isMatchOver, string winnerName, GameState nextState)
    {
        m_DelayManager.AddDelay(float.MaxValue);
        PlayerSpawner.instance.FlushAlivePlayerInputs();
        yield return new WaitForSeconds(timer);
        SwitchBGM();
        m_RoundPanel.SetActive(active);
        DisplayPlayers(isMatchOver, winnerName);
        GameManager.instance.SetGameState(nextState);
        m_DelayManager.Reset();
        m_DelayManager.AddDelay(Database.instance.LongMenuInputDelay);
        SoundManager.PlaySFX(MenuOpen);
        UpdateBoolAnimator("GoDown", true);
    }

    private void SwitchBGM()
    {
        if (m_playSpecialBGM)
        {
            if (!m_playingSpecialBGM)
            {
                SoundManager.StopMusicImmediately();
                SoundManager.Play(m_SpecialBGM, true);
            }
            else
            {
                SoundManager.StopMusicImmediately();
                if(m_myConnection != null)
                    SoundManager.PlayConnection(m_myConnection);
            }
            m_playingSpecialBGM = !m_playingSpecialBGM;
        }
        else
        {
            if (SoundManager.GetVolumeMusic() > m_BGMVolume)
                SoundManager.SetVolumeMusic(m_BGMVolume);
            else
                SoundManager.SetVolumeMusic(1);
        }
    }

    private void DisplayPlayers(bool isMatchOver, string winnerName)
    {
        _winnerImage = null;
        if (isMatchOver)
        {
            m_TopText.text = Database.instance.GameTexts[14] + winnerName;
            m_MatchMenu.SetActive(true);
            m_ListOfButtones[m_currentSelection].Select();
        }
        else
        {
            m_TopText.text = Database.instance.GameTexts[13] + winnerName;
            m_RoundMenu.SetActive(true);
        }


        for (int i = 0; i < m_ListOfPlayerLines.Count; i++)
        {
            if (i < PlayerSpawner.instance.ListOfPlayerDatas.Count)
            {
                m_ListOfPlayerLines[i].SetActive(true);

                Text textTrans = m_ListOfPlayerLines[i].transform.FindChild("PlayerName").gameObject.GetComponent<Text>();
                textTrans.text = PlayerSpawner.instance.ListOfPlayerDatas[i].PlayerSponsor.SponsorName;
                textTrans.color = PlayerSpawner.instance.ListOfPlayerDatas[i].PlayerSponsor.SponsorColor;

                Transform ParentToUse = m_ListOfPlayerLines[i].transform.FindChild("Circles");

                PlayerScoreTracker currentPlayerScore = ScoreManager.instance.GetScoreData(PlayerSpawner.instance.ListOfPlayerDatas[i]);

                int x = 0;
                foreach (Transform child in ParentToUse)
                {

                    Image toChange = child.GetComponent<Image>();
                    if (x < Database.instance.AmountOfRounds)
                    {
                        child.gameObject.SetActive(true);
                        if (currentPlayerScore != null)
                        {
                            if(winnerName == currentPlayerScore.Player.PlayerSponsor.SponsorName && x == currentPlayerScore.CurrentScore - 1)
                            {
                                _winnerImage = toChange;
                            }
                            else if (x < currentPlayerScore.CurrentScore)
                                toChange.sprite = m_Circles[1];
                        }
                    }
                    else
                        child.gameObject.SetActive(false);
                    
                    x++;
                }
            }
        }
    }

    private void GoToNextRound()
    {
        GameManager.instance.SetGameState(GameState.Loading);
        SoundManager.PlaySFX(MenuClose);
        UpdateBoolAnimator("GoUp", true);
    }

    public void TriggerAfterGoUp()
    {
        if (!m_isMatchOver)
        {
            SwitchBGM();
            HidePanelStuff();
            GameManager.instance.StartNextRound();
        }
        else
        {
            SwitchBGM();

            if (m_targetLevel != Application.loadedLevel)
            {
                SoundManager.PlaySFX(ReturnToCS);
                Invoke("LoadLevel", m_DelayForRtuenToCharSel);
            }
            else
            {
                GameManager.instance.SetRoundEnd();
                Application.LoadLevel(m_targetLevel);
            }
        }
    }

    private void LoadLevel()
    {
        Application.LoadLevel(m_targetLevel);
    }

    public void TriggerOnStartOfGoUp()
    {
        SoundManager.PlaySFX(MenuClose);
    }

    private void HidePanelStuff()
    {
        m_MatchMenu.SetActive(false);
        m_RoundMenu.SetActive(false);

        for (int i = 0; i < m_ListOfPlayerLines.Count; i++)
        {
            if (i < PlayerSpawner.instance.ListOfPlayerDatas.Count)
            {

                Transform ParentToUse = m_ListOfPlayerLines[i].transform.FindChild("Circles");

                foreach (Transform child in ParentToUse)
                {
                    child.gameObject.SetActive(false); 
                }

                m_ListOfPlayerLines[i].SetActive(false);
            }
        }
        m_RoundPanel.SetActive(false);
    }

    public void RoundRematch()
    {
        m_targetLevel = Application.loadedLevel;
        UpdateBoolAnimator("GoUp", true);
    }

    private void RoundActivate()
    {
        SoundManager.PlaySFX(Database.instance.MenuClickName);
        m_ListOfButtones[m_currentSelection].onClick.Invoke();
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void SetNextActive(float x, int currentController = 0)
    {
        bool moved = false;

        if (m_lastController != currentController)
        {
            m_lastController = currentController;

            if (x < -Database.instance.MenuNavigationDeadZone || x > Database.instance.MenuNavigationDeadZone)
                m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);

            if (x < -Database.instance.MenuNavigationDeadZone)
            {
                m_currentSelection--;
                if (m_currentSelection < 0) m_currentSelection = m_maxSelection - 1;
                moved = true;
            }
            else if (x > Database.instance.MenuNavigationDeadZone)
            {
                m_currentSelection++;
                if (m_currentSelection == m_maxSelection) m_currentSelection = 0;
                moved = true;
            }

            if (moved)
            {
                SoundManager.PlaySFX(Database.instance.MenuSlideName);
                m_ListOfButtones[m_currentSelection].Select();
            }
        }
        else
        {
            m_lastController = -1;
        }
    }

    public void RoundConfirm(bool closeApp)
    {
        m_inConfirm = true;
        m_ListOfButtones[m_currentSelection].onClick.RemoveAllListeners();
        Text buttonText = m_ListOfButtones[m_currentSelection].GetComponentInChildren<Text>();
        buttonText.text = Database.instance.GameTexts[8];
        if (closeApp)
        {
            m_ListOfButtones[m_currentSelection].onClick.AddListener(() => RoundCloseApp());
        }
        else
        {
            m_ListOfButtones[m_currentSelection].onClick.AddListener(() => RoundReturnToMainMenu());
        }
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void CancelConfirm()
    {
        int textID = 11;
        bool isCloseApp = false;
        if (m_currentSelection == 2)
        {
            textID = 12;
            isCloseApp = true;
        }

        Text buttonText = m_ListOfButtones[m_currentSelection].GetComponentInChildren<Text>();
        buttonText.text = Database.instance.GameTexts[textID];
        m_ListOfButtones[m_currentSelection].onClick.RemoveAllListeners();
        m_ListOfButtones[m_currentSelection].onClick.AddListener(() => RoundConfirm(isCloseApp));
        m_inConfirm = false;
        SoundManager.PlaySFX(Database.instance.MenuCancelName);
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    public void RoundReturnToMainMenu()
    {
        GameManager.instance.SetRoundEnd();
        m_targetLevel = Database.instance.MainMenuID;
        UpdateBoolAnimator("GoUp", true);
    }

    public void RoundCloseApp()
    {
        Application.Quit();
    }
    
    public void UpdateBoolAnimator(string toUpdate, bool with)
    {
        m_animator.SetBool(toUpdate, with);
    }

    public void SetDisplayBool(bool newValue)
    {
        _canDisplayWinner = newValue;
        if (_canDisplayWinner) StartCoroutine(DelayForDisplayWinner());
    }

    private IEnumerator DelayForDisplayWinner()
    {
        yield return new WaitForSeconds(m_DelayToDisplayStar);
        if (_winnerImage != null) DisplayStar();
    }

    private void DisplayStar()
    {
        StartCoroutine(FlashStar());
        SoundManager.PlaySFX(MedalAppear);
        SoundManager.PlaySFX(MedalCrowd);
    }

    private IEnumerator FlashStar()
    {
        _winnerImage.sprite = m_Circles[1];
        yield return new WaitForSeconds(_delayBeforeFlashing);
        for(int i = 0; i< _circleFlashCount; i++)
        {
            _winnerImage.sprite = m_Circles[0];
            yield return new WaitForSeconds(_flashDelay);
            _winnerImage.sprite = m_Circles[1];
            yield return new WaitForSeconds(_flashDelay);
        }
    }
}
