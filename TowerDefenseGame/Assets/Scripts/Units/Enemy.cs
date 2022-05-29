using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected int m_health = 10;
    protected float m_speed = 1f;
    protected List<TDGridObjectWay> m_way;
    protected bool m_alive = false;
    protected TDGridObjectWay m_currentTarget;

    public virtual void Init(int _health, float _speed, List<TDGridObjectWay> _way)
    {
        m_health = _health;
        m_speed = _speed;
        m_way = _way;
        transform.position = _way[0].transform.position;
        m_alive = true;
    }

    public virtual void Move(float _deltaTime)
    {
        transform.Translate((m_currentTarget.WorldCenterPos - this.transform.position).normalized * _deltaTime * m_speed);
    }

    protected abstract void Update();

    protected void EnemyAtFinish()
    {
        // TODO: Lose one life
    }
}