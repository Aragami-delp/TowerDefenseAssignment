using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Tower : MonoBehaviour
{
    protected SOTower m_towerData;
    [SerializeField] protected Transform m_shootingOrigin;
    [SerializeField] protected MeshRenderer m_towerRangeSphere;
    [HideInInspector] public bool TowerPlacedDown = false;
    protected float m_timeSinceLastShot = 0f;

    protected Vector3 GetWorldCenter => this.transform.position + (new Vector3(m_towerData.Footprint.x, 0, m_towerData.Footprint.y) / 2);

    /// <summary>
    /// Initialzes the tower
    /// </summary>
    /// <param name="_ownTower">SO for this tower</param>
    /// <returns>Returns itself</returns>
    public virtual Tower Init(SOTower _ownTower)
    {
        m_towerData = _ownTower;
        SetGlobalScale(m_towerRangeSphere.transform, 2 * TDGridManager.TILE_SCALE * new Vector3(m_towerData.Range, m_towerData.Range, m_towerData.Range)); // radius*2
        ShowRange(true);
        return this;
    }

    public void ShowRange(bool _show)
    {
        m_towerRangeSphere.enabled = _show;
    }

    public abstract bool Shoot();

    public static void SetGlobalScale(Transform _transform, Vector3 _globalScale)
    {
        _transform.localScale = Vector3.one;
        _transform.localScale = new Vector3(_globalScale.x/_transform.lossyScale.x, _globalScale.y/_transform.lossyScale.y, _globalScale.z/_transform.lossyScale.z);
    }

    protected Enemy EnemyInRange()
    {
        return WaveManager.Instance.FurthestEnemyInRange(GetWorldCenter, m_towerData.Range);
    }

    protected virtual void Update()
    {
        if (TowerPlacedDown)
        {
            m_timeSinceLastShot += Time.deltaTime;
            if (m_timeSinceLastShot >= m_towerData.FireRate)
            {
                if (Shoot())
                    m_timeSinceLastShot = 0f;
                else
                    m_timeSinceLastShot -= .1f; // Don't try to shoot immediatly after last one no one was in range (wait 0.1 sec)
            }
        }
    }
}
