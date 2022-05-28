using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private SOTower m_towerData;

    /// <summary>
    /// Initialzes the tower
    /// </summary>
    /// <param name="_ownTower">SO for this tower</param>
    public void Init(SOTower _ownTower)
    {
        m_towerData = _ownTower;
    }
}
