using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup m_towerButtonParent;
    [SerializeField] private List<SOTower> m_soTowers = new List<SOTower>();
    [SerializeField] private TowerButton m_towerButtonPrefab;

    private void Awake()
    {
        foreach (SOTower soTower in m_soTowers)
        {
            Instantiate(m_towerButtonPrefab, m_towerButtonParent.transform).Init(soTower);
        }
    }

    public void CancelBuild()
    {
        TDGridManager.Instance.CancelBuildTower();
    }
}
