using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    protected SOTower m_towerData;
    [SerializeField] protected Transform m_shootingOrigin;
    [SerializeField] protected MeshRenderer m_towerRangeSphere;
    [HideInInspector] public bool TowerPlacedDown = false;

    /// <summary>
    /// Initialzes the tower
    /// </summary>
    /// <param name="_ownTower">SO for this tower</param>
    /// <returns>Returns itself</returns>
    public virtual Tower Init(SOTower _ownTower)
    {
        m_towerData = _ownTower;
        float range = m_towerData.Range / ((m_towerData.Footprint.x + m_towerData.Footprint.y) / 2); // Account for local/world scale
        m_towerRangeSphere.transform.localScale = new Vector3(range, range, range);
        ShowRange(true);
        return this;
    }

    public void ShowRange(bool _show)
    {
        m_towerRangeSphere.enabled = _show;
    }

    public abstract void Shoot(Transform _target);
}
