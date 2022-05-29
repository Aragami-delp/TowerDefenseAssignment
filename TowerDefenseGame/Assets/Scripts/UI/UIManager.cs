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
    /// <summary>
    /// All currently used TowerButtons in the UI
    /// </summary>
    private List<TowerButton> m_towerButtons = new();
    [SerializeField, Tooltip("Button to cancel building")] private Button m_cancelBuildButton;
    [SerializeField, Tooltip("Button to start the next wave")] private Button m_startWaveButton;

    // Text holders
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

        // Load ScriptableObjects
        SOTower[] loadedSOTowersObject = Resources.LoadAll("Towers", typeof(SOTower)).Cast<SOTower>().ToArray();
        // Create TowerButtons for each available tower
        foreach (SOTower soTower in loadedSOTowersObject)
        {
            m_towerButtons.Add(Instantiate(m_towerButtonPrefab, m_towerButtonParent.transform).Init(soTower));
        }

        m_cancelBuildButton.interactable = false;
    }
    #endregion

    #region HUD
    /// <summary>
    /// Updates the hud for money and makes Towerbuttons update its interactable status
    /// </summary>
    /// <param name="_setValue">Money amount to display</param>
    public void UpdateMoneyHud(int _setValue)
    {
        m_moneyText.text = _setValue.ToString();
        foreach (TowerButton button in m_towerButtons)
        {
            button.UpdateEnoughMoney();
        }
    }

    /// <summary>
    /// Updates the hud for health
    /// </summary>
    /// <param name="_setValue">Health amount to display</param>
    public void UpdateHealthHud(int _setValue)
    {
        m_healthText.text = _setValue.ToString();
    }

    /// <summary>
    /// Updates the current wave hud
    /// </summary>
    /// <param name="_setValue">Current wave to display</param>
    public void UpdateWaveHud(int _setValue)
    {
        m_waveText.text = _setValue.ToString();
    }

    /// <summary>
    /// Call from the startWaveButton to start a new wave
    /// </summary>
    public void StartWaveButton()
    {
        m_startWaveButton.interactable = false;
        WaveManager.Instance.StartWave();
    }
    #endregion

    /// <summary>
    /// Enables the startWaveButton when a wave ends
    /// </summary>
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
