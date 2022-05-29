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

    [SerializeField, Tooltip("Button to start the next wave")] private Button m_startWaveButton;

    [SerializeField] private TMP_Text m_moneyText;
    [SerializeField] private TMP_Text m_healthText;
    [SerializeField] private TMP_Text m_waveText;
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

    }
    #endregion

    #region HUD
    public void UpdateMoneyHud(int _setValue)
    {
        m_moneyText.text = _setValue.ToString();
    }

    public void UpdateHealthHud(int _setValue)
    {
        m_healthText.text = _setValue.ToString();
    }

    public void UpdateWaveHud(int _setValue)
    {
        m_waveText.text = _setValue.ToString();
    }
    #endregion
    
    public void StartWaveButton()
    {
        m_startWaveButton.interactable = false;
        WaveManager.Instance.StartWave();
    }

    public void OnEndWave()
    {
        m_startWaveButton.interactable = true;
    }

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
    public void CancelBuildButton()
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
