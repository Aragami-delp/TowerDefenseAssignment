using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField] private VerticalLayoutGroup m_towerButtonParent;
    [SerializeField] private TowerButton m_towerButtonPrefab;
    [SerializeField] private Button m_cancelBuildButton;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        SOTower[] loadedSOTowersObject = Resources.LoadAll("Towers", typeof(SOTower)).Cast<SOTower>().ToArray();
        foreach (SOTower soTower in loadedSOTowersObject)
        {
            Instantiate(m_towerButtonPrefab, m_towerButtonParent.transform).Init(soTower);
        }

        m_cancelBuildButton.interactable = false;
    }

    public void BuildTower(SOTower _soTower)
    {
        m_cancelBuildButton.interactable = true;
        TDGridManager.Instance.BuildTower(_soTower);
    }

    public void CancelBuild()
    {
        DisableCancel();
        TDGridManager.Instance.CancelBuildTower();
    }

    public void DisableCancel()
    {
        m_cancelBuildButton.interactable = false;
    }
}
