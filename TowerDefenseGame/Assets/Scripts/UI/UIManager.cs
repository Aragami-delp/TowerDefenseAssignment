using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance = null;
    #endregion

    #region Inspector Vars
    [SerializeField, Tooltip("To organize buttons")] private VerticalLayoutGroup m_towerButtonParent;
    [SerializeField, Tooltip("Prefab of TowerButton")] private TowerButton m_towerButtonPrefab;
    [SerializeField, Tooltip("Button to cancel building")] private Button m_cancelBuildButton;
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
