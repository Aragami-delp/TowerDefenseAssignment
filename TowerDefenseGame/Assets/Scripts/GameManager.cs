using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance = null;
    #endregion

    [SerializeField] private int m_startMoney = 100;
    [SerializeField] private int m_maxHealth = 100;

    private int m_currentMoney;
    private int m_currentHealth;

    public int WavesSurvived { get; private set; } = 0;

    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        #region Singleton
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion
    }

    private void Start()
    {
        m_currentMoney = m_startMoney;
        m_currentHealth = m_maxHealth;
        UIManager.Instance.UpdateMoneyHud(m_currentMoney);
        UIManager.Instance.UpdateHealthHud(m_currentHealth);
    }

    public void AddMoney(int _amoutToAdd)
    {
        m_currentMoney += _amoutToAdd;
        UIManager.Instance.UpdateMoneyHud(m_currentMoney);
    }

    public bool RemoveMoney(int _amoutToRemove)
    {
        if (HasEnoughMoney(_amoutToRemove))
        {
            m_currentMoney -= _amoutToRemove;
            UIManager.Instance.UpdateMoneyHud(m_currentMoney);
            return true;
        }
        return false;
    }

    public bool HasEnoughMoney(int _amoutToHave)
    {
        return m_currentMoney >= _amoutToHave;
    }

    public void ReduceHealth(int _reduceAmount = 1)
    {
        m_currentHealth = Mathf.Max(0, m_currentHealth - _reduceAmount);
        UIManager.Instance.UpdateHealthHud(m_currentHealth);
        if (m_currentHealth == 0)
        {
            IsGameOver = true;
            WavesSurvived = WaveManager.Instance.WavesSurvived;
            SceneManager.LoadScene(0);
        }
    }
}
