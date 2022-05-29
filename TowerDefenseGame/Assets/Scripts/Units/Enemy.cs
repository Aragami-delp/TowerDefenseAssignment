using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Enemy : MonoBehaviour
{
    protected int m_health = 10;
    protected float m_speed = 1f;
    protected int m_damageToLife = 1;
    protected int m_moneyReward;
    protected List<TDGridObjectWay> m_way;
    protected bool m_alive = false;
    protected TDGridObjectWay m_currentTarget;
    [SerializeField] public Transform ShootTarget;

    public virtual void Init(int _health, float _speed, int _damageToLife, int _moneyReward, List<TDGridObjectWay> _way)
    {
        m_health = _health;
        m_speed = _speed;
        m_damageToLife = _damageToLife;
        m_moneyReward = _moneyReward;
        m_way = _way;
        transform.position = _way[0].WorldCenterPos;
        m_alive = true;
    }

    public virtual void Move(float _deltaTime)
    {
        transform.Translate(_deltaTime * m_speed * (m_currentTarget.WorldCenterPos - this.transform.position).normalized);
    }

    protected abstract void Update();

    protected void EnemyAtFinish()
    {
        WaveManager.Instance.EmemyAtFinish(this, m_damageToLife);
    }

    public void GetDamage(int _amoutOfDamage)
    {
        if (m_alive)
        {
            m_health = Mathf.Max(0, m_health - _amoutOfDamage);
            if (m_health == 0)
            {
                m_alive = false;
                WaveManager.Instance.EnemyDies(this, m_moneyReward);
            }
        }
    }

    /// <summary>
    /// Progress from start to finish in percentage
    /// </summary>
    /// <returns>0f - 1f (%)</returns>
    public abstract float GetProgress();
}
