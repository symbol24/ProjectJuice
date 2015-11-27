using UnityEngine;
using System.Collections;
using System;

public enum SponsorStatus { Taken, Released,}

public class SponsorTakenEventArgs : EventArgs
{
    public SelectorMenu Sender { get; set; }
    public Sponsor SponsorTaken { get; set; }
    public SponsorStatus Status { get; set; }
}
