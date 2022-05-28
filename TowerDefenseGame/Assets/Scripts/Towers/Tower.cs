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
        SetGlobalScale(m_towerRangeSphere.transform, new Vector3(m_towerData.Range, m_towerData.Range, m_towerData.Range) * TDGridManager.TILE_SCALE * 2); // radius*2
        ShowRange(true);
        return this;
    }

    public void ShowRange(bool _show)
    {
        m_towerRangeSphere.enabled = _show;
    }

    public abstract void Shoot(Transform _target);

    public static void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
    }
}
