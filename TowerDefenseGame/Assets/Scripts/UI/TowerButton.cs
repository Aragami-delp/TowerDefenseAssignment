using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private Button m_ownButton;
    private SOTower m_soTower;
    private TMP_Text m_buttonText;

    private void Awake()
    {
        m_buttonText = GetComponentInChildren<TMP_Text>();
    }

    public TowerButton Init(SOTower _soTower)
    {
        m_soTower = _soTower;
        m_buttonText.text = _soTower.Name;
        return this;
    }

    public void OnClick()
    {
        UIManager.Instance.BuildTower(m_soTower);
    }

    public void UpdateEnoughMoney()
    {
        if (m_ownButton != null)
            m_ownButton.interactable = GameManager.Instance.HasEnoughMoney(m_soTower.Cost);
    }
}
