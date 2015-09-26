using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ScoreManager : ExtendedMonobehaviour 
{
    private List<ScoreData> _scoreDatas;
    [SerializeField] private float _delayToCheckIfDraw;
    private List<ScoreData> ScoreDatas { get { return _scoreDatas ?? (_scoreDatas = new List<ScoreData>()); } }
    private List<PlayerScoreTracker> _currentScores;
    private List<PlayerScoreTracker> CurrentScores
    {
        get { return _currentScores ?? (_currentScores = new List<PlayerScoreTracker>()); }
    }
    
    
    //Public Methods/Properties Available
    public IEnumerable<PlayerScoreTracker> PlayerScores
    {
        get { return CurrentScores; }
    }
    public void FollowScoreOf(IPlatformer2DUserControl control)
    {
        var checkHpScript = control.gameObject.GetComponent<HPScript>();
        if(checkHpScript == null) Debug.LogError("CannotFindHpScript");
        FollowScoreOf(control, checkHpScript);
    }
    public void FollowScoreOf(IPlatformer2DUserControl control, HPScript hpScript)
    {
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
        yield return new WaitForSeconds(_delayToCheckIfDraw);
        if (ScoreDatas.Any())
        {
            //Check if we have a single player left
            var possibleWinner = ScoreDatas.SingleOrDefault();
            if (possibleWinner != null)
            {
                //Clear and fire event
                ScoreDatas.Clear();
                OnPlayerScored(possibleWinner.GetEventArgs());
            }
        }
        else
        {
            //Nobody in the round - it is a draw
            OnPlayerScored(new PlayerScoreEventArgs(){IsThereAWinner =  false,});
        }
    }

    protected virtual void OnPlayerScored(PlayerScoreEventArgs e)
    {
        var potentialPlayerWinning =
            CurrentScores.SingleOrDefault(c => c.Player == e.Platformer2DUserControl.m_PlayerData);
        if (potentialPlayerWinning != null)
        {
            potentialPlayerWinning.CurrentScore++;
        }
        else
        {
            CurrentScores.Add(new PlayerScoreTracker {CurrentScore = 1, Player = e.Platformer2DUserControl.m_PlayerData});
        }

        EventHandler<PlayerScoreEventArgs> handler = PlayerScored;
        if (handler != null) handler(this, e);
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




