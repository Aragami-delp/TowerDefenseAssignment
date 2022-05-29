using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAir : Enemy
{
    public override void Init(int _health, float _speed, int _damageToLife, List<TDGridObjectWay> _way)
    {
        base.Init(_health, _speed, _damageToLife, _way);
        m_currentTarget = _way[_way.Count - 1];
        this.transform.GetChild(0).LookAt(m_currentTarget.WorldCenterPos);
    }

    protected override void Update()
    {
        if (Vector3.Distance(m_currentTarget.WorldCenterPos, this.transform.position) < 0.1f)
        {
            EnemyAtFinish();
            return;
        }
        Move(Time.deltaTime);
    }
}
