using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Manages Waves and Enemies
/// </summary>
public class WaveManager : MonoBehaviour
{
    #region Singleton
    public static WaveManager Instance = null;
    #endregion

    #region Vars

    [SerializeField, Tooltip("Which and how many enemies are added each wave")] private List<WaveModifier> m_addEachWaveEnemies = new();
    /// <summary>
    /// The type and amount of enemies in total for this wave
    /// </summary>
    private Dictionary<ENEMYTYPE, int> m_totalWaveEnemies = new();
    /// <summary>
    /// The type and amount of enemies left to spawn for this wave
    /// </summary>
    private Dictionary<ENEMYTYPE, int> m_currentWaveEnemies = new();

    /// <summary>
    /// The pool of enemies to reduce Instantiate calls
    /// </summary>
    private static Dictionary<ENEMYTYPE, List<Enemy>> m_enemyPool = new();
    /// <summary>
    /// ScriptableObjects of the available enemies
    /// </summary>
    private static SOEnemy[] m_soEnemies;

    /// <summary>
    /// Is the wave currently ongoing
    /// </summary>
    private bool m_waveActive = false;
    /// <summary>
    /// Time until the next enemy can be spawned
    /// </summary>
    [SerializeField, Range(0.1f, 10f)] private float m_spawnFrequenzy = 1f;
    /// <summary>
    /// Times since the last enemy spawn
    /// </summary>
    private float m_timeSinceLastSpawn = 0f;
    /// <summary>
    /// Whether all enemies for this wave have been spawned yet
    /// </summary>
    private bool m_allEnemiesSpawned = false;
    /// <summary>
    /// List of all currently living enemies
    /// </summary>
    private List<Enemy> m_aliveEnemies = new();

    /// <summary>
    /// Current wave or last wave if no wave ongoing
    /// </summary>
    private int m_currentWaveNumber = 0; // Starts with 0, first wave triggered is wave 1
    /// <summary>
    /// Total waves survived
    /// </summary>
    public int WavesSurvived => m_currentWaveNumber - 1;
    #endregion

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

        // Load ScriptableObjects
        m_soEnemies = Resources.LoadAll("Enemies", typeof(SOEnemy)).Cast<SOEnemy>().ToArray();
    }

    private void Start()
    {
        // Init waves
        foreach (WaveModifier addThis in m_addEachWaveEnemies)
        {
            m_totalWaveEnemies.Add(addThis.m_type, addThis.m_amount);
        }
    }

    private void Update()
    {
        // Spawn when wave active
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

    /// <summary>
    /// Starts the next wave, note: can be forced in the editor, but not in the game
    /// </summary>
    [ContextMenu("Start Wave")]
    public void StartWave()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            // Reset wave values
            m_currentWaveNumber++;
            UIManager.Instance.UpdateWaveHud(m_currentWaveNumber);
            m_currentWaveEnemies = new Dictionary<ENEMYTYPE, int>(m_totalWaveEnemies); // By value
            m_waveActive = true;
            m_timeSinceLastSpawn = 0f;
            m_allEnemiesSpawned = false;
        }
    }

    /// <summary>
    /// Ends the current wave
    /// </summary>
    private void EndWave()
    {
        m_waveActive = false;
        m_timeSinceLastSpawn = 0f;
        AddWaveEnemies();
        UIManager.Instance.OnEndWave();
    }

    /// <summary>
    /// Checks whether the ongoing wave can be ended
    /// </summary>
    private void CheckWaveStatus()
    {
        if (m_allEnemiesSpawned && m_waveActive && m_aliveEnemies.Count == 0)
        {
            EndWave();
        }
    }

    #region Enemies
    /// <summary>
    /// Processes what happens when an enemy reaches the finish tile
    /// </summary>
    /// <param name="_atFinish">Enemy that reaches the finish</param>
    /// <param name="_amoutToLose">Health to lose; Default: 1</param>
    public void EmemyAtFinish(Enemy _atFinish, int _amoutToLose = 1)
    {
        GameManager.Instance.ReduceHealth(_amoutToLose);
        // Back to pool
        _atFinish.gameObject.SetActive(false);
        m_aliveEnemies.Remove(_atFinish);
        CheckWaveStatus();
    }

    /// <summary>
    /// Processes what happens when an enemy gets killed by a tower
    /// </summary>
    /// <param name="_dyingEnemy">Enemy that dies</param>
    /// <param name="_moneyReward">Reward this enemy gives</param>
    public void EnemyDies(Enemy _dyingEnemy, int _moneyReward)
    {
        GameManager.Instance.AddMoney(_moneyReward);
        m_aliveEnemies.Remove(_dyingEnemy);
        // Back to pool
        _dyingEnemy.gameObject.SetActive(false);
        CheckWaveStatus();
    }

    /// <summary>
    /// Retrieves the enemy that is the furthest on its path to the finish that is within the range of the tower
    /// </summary>
    /// <param name="_tower">Source of the range</param>
    /// <param name="_range">Range of the tower</param>
    /// <returns>First enemy to attack by a tower</returns>
    public Enemy FurthestEnemyInRange(Vector3 _tower, float _range)
    {
        Enemy furthestEnemy = null;
        float furthestPercentage = 0f;
        for (int i = 0; i < m_aliveEnemies.Count; i++)
        {
            float x = Vector3.Distance(m_aliveEnemies[i].transform.position, _tower);
            // Should be faster than Physics.OverlapShpere (advantage: having no Physics on enemies)
            if (Vector3.Distance(m_aliveEnemies[i].transform.position, _tower) < _range * TDGridManager.TILE_SCALE)
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
    /// Increases the amount of enemies each wave so it gets harder over time
    /// </summary>
    private void AddWaveEnemies()
    {
        foreach (WaveModifier addThis in m_addEachWaveEnemies)
        {
            m_totalWaveEnemies[addThis.m_type] += addThis.m_amount;
        }
    }

    /// <summary>
    /// Retrives an Enemy from the pool and creates new one if non exists
    /// </summary>
    /// <param name="_type">Type of Enemy</param>
    /// <returns>A previously unused Enemy</returns>
    private static Enemy GetPooledEnemy(ENEMYTYPE _type)
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
    /// <returns>True: Spawned enemy; False: No enemies left in wave</returns>
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
        // Send out Enemy
        nextEnemy.gameObject.SetActive(true);
        nextEnemy.Init(nextSOEnemy.m_maxHealth, nextSOEnemy.m_speed, nextSOEnemy.m_damageToLife, nextSOEnemy.m_moneyReward, TDGridManager.Instance.EnemyWalkPathTiles);
        m_currentWaveEnemies[nextEnemyType]--;
        return true;
    }

    /// <summary>
    /// Retrieves a random type of enemy of all that could still be spawned
    /// </summary>
    /// <returns>Random enemy type</returns>
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
