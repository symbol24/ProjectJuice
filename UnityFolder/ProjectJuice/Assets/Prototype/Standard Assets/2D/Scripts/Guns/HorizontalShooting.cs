using UnityEngine;
using System.Collections;

public class HorizontalShooting : Gun {
    new void Start()
    {
        base.Start();
        m_CurrentDelay = m_ShotDelayFast;
    }

    public override void Fire()
    {
        base.Fire();
        Bullet newBullet = Instantiate(m_BulletPrefab, m_GunReference.transform.position, m_GunReference.transform.rotation) as Bullet;
    }
}
