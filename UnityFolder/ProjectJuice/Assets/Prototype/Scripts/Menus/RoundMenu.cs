using UnityEngine;
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

    [SerializeField] private float m_DelayBeforeDisplay = 0.5f;

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

    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_maxSelection = m_ListOfButtones.Length;
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        if (GameManager.instance.CurrentState == GameState.RoundEnd)
        {
            for (int i = 0; i < PlayerSpawner.instance.ListOfPlayerDatas.Count; i++)
            {
                if (m_Controls.Confirm[i])
                {
                    GoToNextRound();
                }
            }
        }
        else if (GameManager.instance.CurrentState == GameState.MatchEnd)
        {
            for (int i = 0; i < PlayerSpawner.instance.ListOfPlayerDatas.Count; i++)
            {
                if(m_Controls.X[i] != 0 && m_DelayManager.m_CanShield && !m_inConfirm)
                {
                    SetNextActive(m_Controls.X[i]);
                }

                if(m_Controls.Confirm[i] && m_DelayManager.m_CanShoot)
                {
                    Activate();
                }

                if (m_Controls.Cancel[i] && m_DelayManager.m_CanShoot && m_inConfirm)
                {
                    CancelConfirm();
                }
            }
        }
	}

    public void DisplayRoundMenu(bool active, bool isMatchOver, string winnerName)
    {
        if (active)
            StartCoroutine(DelayBeforeActive(active, m_DelayBeforeDisplay, isMatchOver, winnerName));
        else
            m_RoundPanel.SetActive(active);
    }

    private IEnumerator DelayBeforeActive(bool active, float timer, bool isMatchOver, string winnerName)
    {
        yield return new WaitForSeconds(timer);
        m_RoundPanel.SetActive(active);
        DisplayPlayers(isMatchOver, winnerName);
    }

    private void DisplayPlayers(bool isMatchOver, string winnerName)
    {
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
                    if (x < GameManager.instance.AmountOfRoundsToWin)
                    {
                        child.gameObject.SetActive(true);
                        if (currentPlayerScore != null)
                        {
                            if (x < currentPlayerScore.CurrentScore)
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
        HidePanelStuff();
        GameManager.instance.StartNextRound();
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

    public void Rematch()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    private void Activate()
    {
        m_ListOfButtones[m_currentSelection].onClick.Invoke();
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    private void SetNextActive(float x)
    {
        if (x < 0)
        {
            m_currentSelection--;
            if (m_currentSelection < 0) m_currentSelection = m_maxSelection - 1;
        }
        else if (x > 0)
        {
            m_currentSelection++;
            if (m_currentSelection == m_maxSelection) m_currentSelection = 0;
        }

        m_ListOfButtones[m_currentSelection].Select();
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    public void Confirm(bool closeApp)
    {
        m_inConfirm = true;
        m_ListOfButtones[m_currentSelection].onClick.RemoveAllListeners();
        Text buttonText = m_ListOfButtones[m_currentSelection].GetComponentInChildren<Text>();
        buttonText.text = Database.instance.GameTexts[8];
        if (closeApp)
        {
            m_ListOfButtones[m_currentSelection].onClick.AddListener(() => CloseApp());
        }
        else
        {
            m_ListOfButtones[m_currentSelection].onClick.AddListener(() => ReturnToMainMenu());
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
        m_ListOfButtones[m_currentSelection].onClick.AddListener(() => Confirm(isCloseApp));
        m_inConfirm = false;
        m_DelayManager.CoroutineDelay(Database.instance.MenuInputDelay, false);
    }

    public void ReturnToMainMenu()
    {
        Application.LoadLevel("multipadTest");
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
