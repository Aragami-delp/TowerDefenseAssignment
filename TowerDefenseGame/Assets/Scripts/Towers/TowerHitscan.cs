using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that uses a hitscan attack
/// </summary>
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
                // Hitscan hits the enemy and shows a particle effect for a short time
                m_shootParticle.gameObject.SetActive(true);
                m_shootParticle.transform.LookAt(shootTarget.ShootTarget);
                shootTarget.GetDamage(m_towerData.Damage);
                Invoke(nameof(DisableParticle), .2f);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Disables the particle effect
    /// </summary>
    private void DisableParticle()
    {
        m_shootParticle.gameObject.SetActive(false);
    }
}
