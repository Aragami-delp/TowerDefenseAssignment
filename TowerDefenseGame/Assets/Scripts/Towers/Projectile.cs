using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_speed = 20f;
    private Enemy m_target;
    private int m_damage;

    public void FlyAtTarget(Enemy _target, int _damage)
    {
        m_target = _target;
        m_damage = _damage;
    }

    private void Update()
    {
        if (m_target != null)
        {
            if (Vector3.Distance(m_target.ShootTarget.position, this.transform.position) < 0.1f)
            {
                m_target.GetDamage(m_damage);
                this.gameObject.SetActive(false);
                return;
            }
            Move(Time.deltaTime);
        }
    }

    private void Move(float _deltaTime)
    {
        transform.Translate(_deltaTime * m_speed * (m_target.ShootTarget.position - this.transform.position).normalized);
    }
}