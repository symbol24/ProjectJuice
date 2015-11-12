using System;
using UnityEngine;
using System.Collections;

public interface IDartGun : IGameObject
{
    bool enabled { get; set; }
    DartGunSettings Settings { get; set; }

    /// <summary>
    /// Fire FX
    /// </summary>
    string Fire { get; set; }

    /// <summary>
    /// Transfering FX
    /// </summary>
    string Transfering { get; set; }

    /// <summary>
    /// CoolDown FX
    /// </summary>
    string CoolDown { get; set; }

    ParticleSystem m_firingParticle { get; set; }

    event EventHandler<DartFiredEventArgs> DartFired;

    GameObject DartSpawnPoint { get; }
}
