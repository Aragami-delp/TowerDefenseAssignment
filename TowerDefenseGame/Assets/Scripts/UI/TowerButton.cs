using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private SOTower m_soTower;
    private TMP_Text m_buttonText;

    private void Awake()
    {
        m_buttonText = GetComponentInChildren<TMP_Text>();
        Init(m_soTower);
    }

    public void Init(SOTower _soTower)
    {
        m_soTower = _soTower;
        m_buttonText.text = _soTower.Name;
    }

    public void OnClick()
    {
        UIManager.Instance.BuildTower(m_soTower);
    }
}
