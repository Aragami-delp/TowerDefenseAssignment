using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance = null;
    #endregion

    #region Inspector Vars
    [SerializeField, Tooltip("To organize buttons")] private VerticalLayoutGroup m_towerButtonParent;
    [SerializeField, Tooltip("Prefab of TowerButton")] private TowerButton m_towerButtonPrefab;
    [SerializeField, Tooltip("Button to cancel building")] private Button m_cancelBuildButton;

    [SerializeField] private TMP_Text m_moneyText;
    [SerializeField] private TMP_Text m_healthText;
    [SerializeField] private TMP_Text m_waveText;

    [SerializeField] private int m_startMoney = 100;
    [SerializeField] private int m_maxHealth = 100;

    private int m_currentMoney;
    private int m_currentHealth;
    private int m_currentWave;
    #endregion

    #region Mono Callbacks
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

        SOTower[] loadedSOTowersObject = Resources.LoadAll("Towers", typeof(SOTower)).Cast<SOTower>().ToArray();
        foreach (SOTower soTower in loadedSOTowersObject)
        {
            Instantiate(m_towerButtonPrefab, m_towerButtonParent.transform).Init(soTower);
        }

        m_cancelBuildButton.interactable = false;
    }

    private void Start()
    {
        m_currentMoney = m_startMoney;
        m_currentHealth = m_maxHealth;
        m_currentWave = 0;
    }
    #endregion

    #region HUD
    public void SetMoney(int _setAmount)
    {
        m_currentMoney = _setAmount;
        m_moneyText.text = m_currentMoney.ToString();
    }

    public void ReduceMoney(int _reduceAmount)
    {
        m_currentMoney -= _reduceAmount;
        m_moneyText.text = m_currentMoney.ToString();
    }

    public void IncreaseMoney(int _increaseAmount)
    {
        m_currentMoney += _increaseAmount;
        m_moneyText.text = m_currentMoney.ToString();
    }

    public bool HasEnoughMoney(int _amoutToHave)
    {
        return m_currentMoney >= _amoutToHave;
    }

    public void SetHealth(int _setHealth)
    {
        m_currentHealth = _setHealth;
        m_healthText.text = m_currentHealth.ToString();
    }

    public void ReduceHealth(int _reduceAmount)
    {
        m_currentHealth = Mathf.Max(0, m_currentHealth - _reduceAmount);
        m_healthText.text = m_currentHealth.ToString();
    }

    public void SetWave(int _setAmount)
    {
        m_currentWave = _setAmount;
        m_waveText.text = m_currentWave.ToString();
    }

    public void IncreaseWave(int _increaseAmount = 1)
    {
        m_currentWave++;
        m_waveText.text = m_currentWave.ToString();
    }
    #endregion

    #region Building Mode
    /// <summary>
    /// Start building selected tower
    /// </summary>
    /// <param name="_soTower">Tower to build</param>
    public void BuildTower(SOTower _soTower)
    {
        m_cancelBuildButton.interactable = true;
        TDGridManager.Instance.BuildTower(_soTower);
    }

    /// <summary>
    /// Stop building
    /// </summary>
    public void CancelBuild()
    {
        DisableCancel();
        TDGridManager.Instance.CancelBuildTower();
    }

    /// <summary>
    /// Disable cancel button
    /// </summary>
    public void DisableCancel()
    {
        m_cancelBuildButton.interactable = false;
    }
    #endregion
}
