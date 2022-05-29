using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    #region Singleton
    public static WaveManager Instance = null;
    #endregion

    [SerializeField] private List<WaveModifier> m_addEachWaveEnemies = new();
    private Dictionary<ENEMYTYPE, int> m_totalWaveEnemies = new();
    private Dictionary<ENEMYTYPE, int> m_currentWaveEnemies = new();

    private Dictionary<ENEMYTYPE, List<Enemy>> m_enemyPool = new();
    private SOEnemy[] m_soEnemies;

    private bool m_waveActive = false;
    [SerializeField, Range(0.1f, 10f)] private float m_spawnFrequenzy = 1f;
    private float m_timeSinceLastSpawn = 0f;
    private bool m_allEnemiesSpawned = false;
    private List<Enemy> m_aliveEnemies = new();

    private int m_currentWaveNumber = 0; // Starts with 0, first wave triggered is wave 1
    public int WavesSurvived => m_currentWaveNumber - 1;

    private void Awake()
    {
        #region Singleton
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        #endregion

        m_soEnemies = Resources.LoadAll("Enemies", typeof(SOEnemy)).Cast<SOEnemy>().ToArray();
    }

    private void Start()
    {
        foreach (WaveModifier addThis in m_addEachWaveEnemies)
        {
            m_totalWaveEnemies.Add(addThis.m_type, addThis.m_amount);
        }
    }

    private void Update()
    {
        if (m_waveActive && !m_allEnemiesSpawned)
        {
            m_timeSinceLastSpawn += Time.deltaTime;
            if (m_timeSinceLastSpawn >= m_spawnFrequenzy)
            {
                m_timeSinceLastSpawn = 0f;
                m_allEnemiesSpawned = !SpawnEnemy();
            }
        }
    }

    [ContextMenu("Start Wave")]
    public void StartWave()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            m_currentWaveNumber++;
            UIManager.Instance.UpdateWaveHud(m_currentWaveNumber);
            m_currentWaveEnemies = new Dictionary<ENEMYTYPE, int>(m_totalWaveEnemies); // By value
            m_waveActive = true;
            m_timeSinceLastSpawn = 0f;
            m_allEnemiesSpawned = false;
        }
    }

    private void EndWave()
    {
        m_waveActive = false;
        m_timeSinceLastSpawn = 0f;
        AddWaveEnemies();
        UIManager.Instance.OnEndWave();
    }

    private void CheckWaveStatus()
    {
        // TODO: CheckWaveStatus() when enemy dies
        if (m_allEnemiesSpawned && m_waveActive && m_aliveEnemies.Count == 0)
        {
            EndWave();
        }
    }

    public void EmemyAtFinish(Enemy _atFinish, int _amoutToLose = 1)
    {
        GameManager.Instance.ReduceHealth(_amoutToLose);
        _atFinish.gameObject.SetActive(false);
        m_aliveEnemies.Remove(_atFinish);
        CheckWaveStatus();
    }

    public void EnemyDies(Enemy _dyingEnemy, int _moneyReward)
    {
        GameManager.Instance.AddMoney(_moneyReward);
        m_aliveEnemies.Remove(_dyingEnemy);
        _dyingEnemy.gameObject.SetActive(false);
        CheckWaveStatus();
    }

    #region Enemies

    public Enemy FurthestEnemyInRange(Tower _tower, float _range)
    {
        Enemy furthestEnemy = null;
        float furthestPercentage = 0f;
        for (int i = 0; i < m_aliveEnemies.Count; i++)
        {
            float x = Vector3.Distance(m_aliveEnemies[i].transform.position, _tower.transform.position);
            // Should be faster than Physics.OverlapShpere (advantage: having no Physics on enemies)
            if (Vector3.Distance(m_aliveEnemies[i].transform.position, _tower.transform.position) < _range * 2)
            {
                float nextEnemyProgress = m_aliveEnemies[i].GetProgress();
                if (nextEnemyProgress > furthestPercentage)
                {
                    furthestEnemy = m_aliveEnemies[i];
                    furthestPercentage = nextEnemyProgress;
                }
            }
        }
        return furthestEnemy;
    }

    /// <summary>
    /// Should be done at the end of a wave/beginning of the build phase
    /// </summary>
    private void AddWaveEnemies()
    {
        foreach (WaveModifier addThis in m_addEachWaveEnemies)
        {
            m_totalWaveEnemies[addThis.m_type] += addThis.m_amount;
        }
    }

    private Enemy GetPooledEnemy(ENEMYTYPE _type)
    {
        if (m_enemyPool.ContainsKey(_type))
        {
            for (int i = 0; i < m_enemyPool[_type].Count; i++)
            {
                if (!m_enemyPool[_type][i].gameObject.activeInHierarchy)
                {
                    return m_enemyPool[_type][i];
                }
            }

            // No disabled object in pool
            Enemy nextEnemyPrefab = null;
            foreach (SOEnemy soEnemy in m_soEnemies)
            {
                if (soEnemy.m_type == _type)
                {
                    nextEnemyPrefab = soEnemy.m_enemyPrefab;
                    break;
                }
            }
            Enemy newEnemy;
            if (nextEnemyPrefab != null)
            {
                newEnemy = Instantiate(nextEnemyPrefab);
            }
            else
            {
                throw new UnityException("No prefab on SOEnemy for type: " + _type);
            }
            m_enemyPool[_type].Add(newEnemy);
            return newEnemy;
        }
        else
        {
            m_enemyPool.Add(_type, new List<Enemy>());
            return GetPooledEnemy(_type);
        }
    }

    /// <summary>
    /// Spawns a random enemy
    /// </summary>
    /// <returns>True: Spawned enmey; False: No enemies left in wave</returns>
    private bool SpawnEnemy()
    {
        ENEMYTYPE nextEnemyType = GetRandomEnemyOfCurrentWave();
        if (nextEnemyType == ENEMYTYPE.NONE)
            return false;
        Enemy nextEnemy = GetPooledEnemy(nextEnemyType);
        m_aliveEnemies.Add(nextEnemy);
        SOEnemy nextSOEnemy = null;
        foreach (SOEnemy soEnemy in m_soEnemies)
        {
            if (soEnemy.m_type == nextEnemyType)
            {
                nextSOEnemy = soEnemy;
                break;
            }
        }
        nextEnemy.gameObject.SetActive(true);
        nextEnemy.Init(nextSOEnemy.m_maxHealth, nextSOEnemy.m_speed, nextSOEnemy.m_damageToLife, nextSOEnemy.m_moneyReward, TDGridManager.Instance.EnemyWalkPathTiles);
        m_currentWaveEnemies[nextEnemyType]--;
        return true;
    }

    private ENEMYTYPE GetRandomEnemyOfCurrentWave()
    {
        Dictionary<ENEMYTYPE, int> dicWithValuesGreaterZero = m_currentWaveEnemies.Where(i => i.Value > 0).ToDictionary(i => i.Key, i => i.Value);
        if (dicWithValuesGreaterZero.Count == 0)
            return ENEMYTYPE.NONE;
        KeyValuePair<ENEMYTYPE, int> randomType = dicWithValuesGreaterZero.ElementAt(UnityEngine.Random.Range(0, dicWithValuesGreaterZero.Count));
        return randomType.Key;
    }
    #endregion
}

/// <summary>
/// Which enemy should get added each round and how many
/// </summary>
[System.Serializable]
public struct WaveModifier
{
    [SerializeField, Tooltip("Enemy to add each round")] public ENEMYTYPE m_type;
    [SerializeField, Tooltip("Amount to add to")] public int m_amount;
}
