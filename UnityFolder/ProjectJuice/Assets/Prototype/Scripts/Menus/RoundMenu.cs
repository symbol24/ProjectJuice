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

    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public void DisplayRoundMenu(bool active)
    {
        if (active)
            StartCoroutine(DelayBeforeActive(active, m_DelayBeforeDisplay));
        else
            m_RoundPanel.SetActive(active);
    }

    private IEnumerator DelayBeforeActive(bool active, float timer)
    {
        yield return new WaitForSeconds(timer);
        m_RoundPanel.SetActive(active);
        DisplayPlayers();
    }

    private void DisplayPlayers()
    {

        for (int i = 0; i < PlayerSpawner.instance.Players.Count; i++)
        {
            m_ListOfPlayerLines[i].SetActive(true);
            Transform ParentToUse = transform.FindChild("Circles");

            int x = 0;
            foreach(Transform child in ParentToUse)
            {
                
                if (x < GameManager.instance.AmountOfRoundsToWin)
                {
                    child.gameObject.SetActive(true);
                    Image toChange = child.GetComponent<Image>();
                    if (x < ScoreManager.instance.PlayerScores[i].CurrentScore)
                        toChange.sprite = m_Circles[1];
                }
                else
                    child.gameObject.SetActive(false);
            }
            
        }
    }
}
