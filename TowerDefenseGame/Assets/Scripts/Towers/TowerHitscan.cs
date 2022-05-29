using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHitscan : Tower
{
    public override bool Shoot()
    {
        if (TowerPlacedDown)
        {
            Enemy shootTarget = EnemyInRange();
            if (shootTarget != null)
            {
                Debug.Log("TowerHitscan shoot");
                return true;
            }
        }
        return false;
    }
}
