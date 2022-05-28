using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : Tower
{
    [SerializeField] private ParticleSystem m_projecttilePrefab;
    public override void Shoot(Transform _target)
    {
        throw new System.NotImplementedException();
    }
}
