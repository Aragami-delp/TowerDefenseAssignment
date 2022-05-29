using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Enemy that gets from start to finish
/// </summary>
[SelectionBase]
public abstract class Enemy : MonoBehaviour
{
    protected int m_health = 10;
    protected float m_speed = 1f;
    protected int m_damageToLife = 1;
    protected int m_moneyReward;
    protected List<TDGridObjectWay> m_way; // Path to take from start to finish
    protected bool m_alive = false;
    /// <summary>
    /// The current Target to get to
    /// </summary>
    protected TDGridObjectWay m_currentTarget;
    /// <summary>
    /// Where towers should shoot at when attacking this Enemy
    /// </summary>
    [SerializeField] public Transform ShootTarget;

    /// <summary>
    /// Initializes this enemy
    /// </summary>
    /// <param name="_health">The health this Enemy starts with</param>
    /// <param name="_speed">The speed this Enemy uses to get to the finish</param>
    /// <param name="_damageToLife">The damge that is dealt to the health when reaching the finish</param>
    /// <param name="_moneyReward">The reward that is given when this Enemy dies</param>
    /// <param name="_way">The path to take from start to finish</param>
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

    /// <summary>
    /// Moves this Enemy toward the current target with timescale
    /// </summary>
    /// <param name="_deltaTime">Timescale for moving</param>
    public void Move(float _deltaTime)
    {
        transform.Translate(_deltaTime * m_speed * (m_currentTarget.WorldCenterPos - this.transform.position).normalized);
    }

    protected abstract void Update();

    /// <summary>
    /// What happens when the enemy reaches the finish
    /// </summary>
    protected void EnemyAtFinish()
    {
        WaveManager.Instance.EmemyAtFinish(this, m_damageToLife);
    }

    /// <summary>
    /// This enemy takes to given amount of damage
    /// </summary>
    /// <param name="_amoutOfDamage">Amount of damage to take</param>
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
