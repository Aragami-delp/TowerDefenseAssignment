using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enemy the walks from start to finish
/// </summary>
public class EnemyGround : Enemy
{
    /// <summary>
    /// Index of the tile to walk to
    /// </summary>
    private int m_currentWayIndex = 1; // Start at 0 walk to 1

    public override void Init(int _health, float _speed, int _damageToLife, int _moneyReward, List<TDGridObjectWay> _way)
    {
        base.Init(_health, _speed, _damageToLife, _moneyReward, _way);
        m_currentWayIndex = 1;
        m_currentTarget = _way[m_currentWayIndex];
        this.transform.GetChild(0).LookAt(m_currentTarget.WorldCenterPos);
    }

    protected override void Update()
    {
        if (Vector3.Distance(m_way[m_way.Count - 1].WorldCenterPos, this.transform.position) < 0.1f)
        {
            EnemyAtFinish();
            return;
        }
        if (Vector3.Distance(m_currentTarget.WorldCenterPos, this.transform.position) < 0.1f)
        {
            m_currentTarget = m_way[++m_currentWayIndex];
            this.transform.GetChild(0).LookAt(m_currentTarget.WorldCenterPos);
        }
        Move(Time.deltaTime);
    }

    public override float GetProgress()
    {
        return Mathf.InverseLerp(0, m_way.Count - 1, m_currentWayIndex);
    }
}
