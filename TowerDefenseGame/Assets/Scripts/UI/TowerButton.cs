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

    /// <summary>
    /// Initializes this TowerButton
    /// </summary>
    /// <param name="_soTower">ScriptableObject with data to the tower</param>
    /// <returns>Resturns itself</returns>
    public TowerButton Init(SOTower _soTower)
    {
        m_soTower = _soTower;
        m_buttonText.text = _soTower.Name;
        return this;
    }

    /// <summary>
    /// When pressed on the button
    /// </summary>
    public void OnClick()
    {
        UIManager.Instance.BuildTower(m_soTower);
    }

    /// <summary>
    /// Enables/Disables itself depending if there is enough money to buy this kind of tower
    /// </summary>
    public void UpdateEnoughMoney()
    {
        if (m_ownButton != null)
            m_ownButton.interactable = GameManager.Instance.HasEnoughMoney(m_soTower.Cost);
    }
}
