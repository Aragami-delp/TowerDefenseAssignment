using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IGridBuilding
{
    private SOTower m_towerData;

    public void Init(SOTower _ownTower)
    {
        m_towerData = _ownTower;
    }
}
