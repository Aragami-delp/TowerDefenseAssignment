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

    /// <summary>
    /// Adds money and updates hud
    /// </summary>
    /// <param name="_amoutToAdd">Amount of money to add</param>
    public void AddMoney(int _amoutToAdd)
    {
        m_currentMoney += _amoutToAdd;
        UIManager.Instance.UpdateMoneyHud(m_currentMoney);
    }

    /// <summary>
    /// Removes money only if there is enough
    /// </summary>
    /// <param name="_amoutToRemove">Amount of money to remove</param>
    /// <returns>True: Money has been removed; False: Not enough money, it has not been removed</returns>
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

    /// <summary>
    /// Check whether there is at least the desired amout of money
    /// </summary>
    /// <param name="_amoutToHave">Desired amount of money</param>
    /// <returns>True: There is enough money; False: Not enough money</returns>
    public bool HasEnoughMoney(int _amoutToHave)
    {
        return m_currentMoney >= _amoutToHave;
    }

    /// <summary>
    /// Reduces health by the given amount. Default amount: 1
    /// </summary>
    /// <param name="_reduceAmount">Amount of health to reduce</param>
    public void ReduceHealth(int _reduceAmount = 1)
    {
        m_currentHealth = Mathf.Max(0, m_currentHealth - _reduceAmount);
        UIManager.Instance.UpdateHealthHud(m_currentHealth);
        if (m_currentHealth == 0)
        {
            WavesSurvived = WaveManager.Instance.WavesSurvived;
            SceneManager.LoadScene(0);
        }
    }
}
