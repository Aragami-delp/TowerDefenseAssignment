using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance = null;
    #endregion

    [SerializeField] private int m_startMoney = 100;
    [SerializeField] private int m_maxHealth = 100;

    private int m_currentMoney;
    private int m_currentHealth;

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
    }

    private void Start()
    {
        m_currentMoney = m_startMoney;
        m_currentHealth = m_maxHealth;
        UIManager.Instance.UpdateMoneyHud(m_currentMoney);
        UIManager.Instance.UpdateHealthHud(m_currentHealth);
    }

    public bool HasEnoughMoney(int _amoutToHave)
    {
        return m_currentMoney >= _amoutToHave;
    }

    public void SetHealth(int _setHealth)
    {
        m_currentHealth = _setHealth;
    }

    public void ReduceHealth(int _reduceAmount = 1)
    {
        m_currentHealth = Mathf.Max(0, m_currentHealth - _reduceAmount);
    }
}
