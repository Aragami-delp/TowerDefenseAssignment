using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that attacks enemies
/// </summary>
[SelectionBase]
public abstract class Tower : MonoBehaviour
{
    protected SOTower m_towerData;
    [SerializeField, Tooltip("Where the shoots will start from")] protected Transform m_shootingOrigin;
    [SerializeField, Tooltip("Mesh that shows the range of the tower")] protected MeshRenderer m_towerRangeSphere;
    [HideInInspector] public bool TowerPlacedDown = false;
    protected float m_timeSinceLastShot = 0f;

    /// <summary>
    /// Gets the center of this tower in world space
    /// </summary>
    protected Vector3 GetWorldCenter => this.transform.position + (new Vector3(m_towerData.Footprint.x, 0, m_towerData.Footprint.y) / 2);

    /// <summary>
    /// Initialzes the tower
    /// </summary>
    /// <param name="_ownTower">SO for this tower</param>
    /// <returns>Returns itself</returns>
    public virtual Tower Init(SOTower _ownTower)
    {
        m_towerData = _ownTower;
        SetRangeScale(2 * TDGridManager.TILE_SCALE * new Vector3(m_towerData.Range, m_towerData.Range, m_towerData.Range)); // radius*2
        ShowRange(true);
        return this;
    }

    /// <summary>
    /// Enables/Disables the range sphere to show the towers range
    /// </summary>
    /// <param name="_show">True: Show range; False: Hide range</param>
    public void ShowRange(bool _show)
    {
        m_towerRangeSphere.enabled = _show;
    }

    /// <summary>
    /// Shoots at an enemy
    /// </summary>
    /// <returns>True: If enemy exists; False: If there is not enemy</returns>
    public abstract bool Shoot();

    /// <summary>
    /// Sets the scale of the range sphere in world scale
    /// </summary>
    /// <param name="_globalScale">Scale of the range sphere in world units</param>
    public void SetRangeScale(Vector3 _globalScale)
    {
        m_towerRangeSphere.transform.localScale = Vector3.one;
        m_towerRangeSphere.transform.localScale = new Vector3(_globalScale.x/m_towerRangeSphere.transform.lossyScale.x, _globalScale.y/m_towerRangeSphere.transform.lossyScale.y, _globalScale.z/m_towerRangeSphere.transform.lossyScale.z);
    }

    /// <summary>
    /// Gets the enemy within range that is the furthest on his path
    /// </summary>
    /// <returns>Furthest enemy, null if no one is in range nor exists</returns>
    protected Enemy EnemyInRange()
    {
        return WaveManager.Instance.FurthestEnemyInRange(GetWorldCenter, m_towerData.Range);
    }

    protected virtual void Update()
    {
        // Only shoots when placed down
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
