using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYTYPE
{
    NONE = 0,
    GROUND = 1,
    AIR = 2
}

[CreateAssetMenu(fileName = "Enemy", menuName = "TD/Enemy")]
public class SOEnemy : ScriptableObject
{
    public int m_maxHealth;
    public float m_speed;
    public int m_damageToLife;
    public int m_moneyReward;
    public ENEMYTYPE m_type;
    public Enemy m_enemyPrefab;
}
