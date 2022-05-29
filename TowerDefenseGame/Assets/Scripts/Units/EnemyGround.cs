using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGround : Enemy
{
    private int m_currentWayIndex = 1; // Start at 0 walk to 1

    public override void Init(int _health, float _speed, List<TDGridObjectWay> _way)
    {
        base.Init(_health, _speed, _way);
        m_currentTarget = _way[1];
        this.transform.GetChild(0).LookAt(m_currentTarget.WorldCenterPos);
    }

    protected override void Update()
    {
        if (Vector3.Distance(m_way[m_way.Count - 1].WorldCenterPos, this.transform.position) < 0.1f) // TODO: Vectir3.Distance is not very efficient
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
}
