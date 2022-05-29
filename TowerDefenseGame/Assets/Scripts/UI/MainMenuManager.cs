using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_scoreLabel;
    [SerializeField] private RectTransform m_startPanel;
    [SerializeField] private RectTransform m_gameOverPanel;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            m_gameOverPanel.gameObject.SetActive(true);
            m_startPanel.gameObject.SetActive(false);
            m_scoreLabel.text = "Waves survived: " + GameManager.Instance.WavesSurvived.ToString();
            return;
        }
        m_gameOverPanel.gameObject.SetActive(false);
        m_startPanel.gameObject.SetActive(true);
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
