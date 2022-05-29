using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHitscan : Tower
{
    [SerializeField] private ParticleSystem m_shootParticle;
    public override bool Shoot()
    {
        if (TowerPlacedDown)
        {
            Enemy shootTarget = EnemyInRange();
            if (shootTarget != null)
            {
                // TODO Particle
                m_shootParticle.gameObject.SetActive(true);
                m_shootParticle.transform.LookAt(shootTarget.ShootTarget);
                shootTarget.GetDamage(m_towerData.Damage);
                Invoke(nameof(DisableParticle), .2f);
                return true;
            }
        }
        return false;
    }

    private void DisableParticle()
    {
        m_shootParticle.gameObject.SetActive(false);
    }
}
