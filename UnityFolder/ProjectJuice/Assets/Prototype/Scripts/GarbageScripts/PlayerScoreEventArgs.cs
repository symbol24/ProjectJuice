using System;

public class PlayerScoreEventArgs : EventArgs
{
    public HPScript HpScript { get; set; }
    public IPlatformer2DUserControl Platformer2DUserControl { get; set; }
    public bool IsThereAWinner { get; set; }
    public int PlayerScore { get; set; }
}
