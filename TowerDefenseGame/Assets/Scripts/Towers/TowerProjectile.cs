using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : Tower
{
    [SerializeField] private Projectile m_projectilePrefab;
    private List<Projectile> m_projectilePool = new();
    public override bool Shoot()
    {
        if (TowerPlacedDown)
        {
            Enemy shootTarget = EnemyInRange();
            if (shootTarget != null)
            {
                Projectile nextProjectile = GetProjectileFromPool();
                nextProjectile.transform.position = this.m_shootingOrigin.position;
                nextProjectile.gameObject.SetActive(true);
                nextProjectile.FlyAtTarget(shootTarget, m_towerData.Damage);
                return true;
            }
        }
        return false;
    }

    private Projectile GetProjectileFromPool()
    {
        for (int i = 0; i < m_projectilePool.Count; i++)
        {
            if (!m_projectilePool[i].gameObject.activeInHierarchy)
            {
                return m_projectilePool[i];
            }
        }
        Projectile nextProjectile = Instantiate(m_projectilePrefab);
        m_projectilePool.Add(nextProjectile);
        return nextProjectile;
    }
}
