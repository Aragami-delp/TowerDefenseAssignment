using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_speed = 20f;
    private Enemy m_target;
    private int m_damage;

    /// <summary>
    /// Start flying at a target
    /// </summary>
    /// <param name="_target">Target to fly to</param>
    /// <param name="_damage">Amount of damge to dead to target</param>
    public void FlyAtTarget(Enemy _target, int _damage)
    {
        m_target = _target;
        m_damage = _damage;
    }

    private void Update()
    {
        if (m_target != null)
        {
            // If close deal damage and get back into pool
            if (Vector3.Distance(m_target.ShootTarget.position, this.transform.position) < 0.1f)
            {
                m_target.GetDamage(m_damage);
                this.gameObject.SetActive(false);
                return;
            }
            Move(Time.deltaTime);
        }
    }

    /// <summary>
    /// Moves towards a target
    /// </summary>
    /// <param name="_deltaTime">Timescale</param>
    private void Move(float _deltaTime)
    {
        transform.Translate(_deltaTime * m_speed * (m_target.ShootTarget.position - this.transform.position).normalized);
    }
}