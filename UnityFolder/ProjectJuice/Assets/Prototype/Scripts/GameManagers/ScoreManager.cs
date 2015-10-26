using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ScoreManager : ExtendedMonobehaviour 
{
    #region instance Stuffs
    private static ScoreManager _instance;

    public static ScoreManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ScoreManager>();
            
            return _instance;
        }
    }
    #endregion

    private List<ScoreData> _scoreDatas;
    [SerializeField] private float _delayToCheckIfDraw;
    private List<ScoreData> ScoreDatas { get { return _scoreDatas ?? (_scoreDatas = new List<ScoreData>()); } }
    private List<PlayerScoreTracker> _currentScores;
    private List<PlayerScoreTracker> CurrentScores
    {
        get { return _currentScores ?? (_currentScores = new List<PlayerScoreTracker>()); }
    }
    public IEnumerable<PlayerScoreTracker> PlayerScores
    {
        get { return CurrentScores; }
    }

    void Start()
    {
        GameManager.instance.SubscribeToEndRound();
    }
    

    public void FollowScoreOf(IPlatformer2DUserControl control)
    {
        var checkHpScript = control.gameObject.GetComponent<HPScript>();
        if(checkHpScript == null) Debug.LogError("CannotFindHpScript");
        FollowScoreOf(control, checkHpScript);
    }
    public void FollowScoreOf(IPlatformer2DUserControl control, HPScript hpScript)
    {
        CurrentScores.Add(new PlayerScoreTracker { CurrentScore = 0, Player = control.m_PlayerData});
        var data = new ScoreData() {HpScript = hpScript, Platformer2DUserControl = control};
        data.HpScript.Dead += HpScript_Dead;
        ScoreDatas.Add(data);
    }

    public event EventHandler<PlayerScoreEventArgs> PlayerScored;


    private void HpScript_Dead(object sender, EventArgs e)
    {
        var hpScriptReceived = (HPScript) sender;
        var checkInList = ScoreDatas.SingleOrDefault(c => c.HpScript.GetInstanceID() == hpScriptReceived.GetInstanceID());
        if (checkInList != null)
        {
            ScoreDatas.Remove(checkInList);
        }
        if (_isAnounceWinnerRunning)
        {
            StopCoroutine(_anounceWinner);
            _isAnounceWinnerRunning = false;
        }
        _anounceWinner = AnounceWinner();
        StartCoroutine(_anounceWinner);
    }

    private bool _isAnounceWinnerRunning = false;
    private IEnumerator _anounceWinner;

    private IEnumerator AnounceWinner()
    {
        if (ScoreDatas.Count <= 1)
        {
            yield return new WaitForSeconds(_delayToCheckIfDraw);
            //Check if we have a single player left
            if (ScoreDatas.Count == 1)
            {
                //Clear and fire event
                var possibleWinner = ScoreDatas.Single();
                ScoreDatas.Clear();
                OnPlayerScored(possibleWinner.GetEventArgs());
            }
            else
            {
                //Nobody in the round - it is a draw
                OnPlayerScored(new PlayerScoreEventArgs() {IsThereAWinner = false,});
            }

        }

    }

    protected virtual void OnPlayerScored(PlayerScoreEventArgs e)
    {
        try
        {
            if (e.IsThereAWinner)
            {
                var potentialPlayerWinning =
                    CurrentScores.First(c => c.Player == e.Platformer2DUserControl.m_PlayerData);
                    potentialPlayerWinning.CurrentScore++;
                e.PlayerScore = potentialPlayerWinning.CurrentScore;
            }
        }
        catch (Exception ex)
        {
            ex.Log("InScoreManager when checking for winner");
            throw;
        }
        try
        {
            EventHandler<PlayerScoreEventArgs> handler = PlayerScored;
            if (handler != null) handler(this, e);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }

    }

    public PlayerScoreTracker GetScoreData(PlayerData player)
    {
        var ret = CurrentScores.FirstOrDefault(c => c.Player == player);
        return ret;
    }

    public void Reset()
    {
        foreach(var score in CurrentScores)
        {
            score.CurrentScore = 0;
        }
    }


    //InternalDataClass
    private class ScoreData
    {
        public HPScript HpScript { get; set; }
        public IPlatformer2DUserControl Platformer2DUserControl { get; set; }

        public PlayerScoreEventArgs GetEventArgs()
        {
            return new PlayerScoreEventArgs()
            {
                HpScript = HpScript,
                Platformer2DUserControl = Platformer2DUserControl,
                IsThereAWinner = true,
            };
        }
    }
}




