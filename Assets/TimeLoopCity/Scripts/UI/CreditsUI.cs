using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace TimeLoopCity.UI
{
    public class CreditsUI : MonoBehaviour
    {
        public static CreditsUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private RectTransform scrollingContent;
        [SerializeField] private Button quitButton;
        
        [Header("Settings")]
        [SerializeField] private float scrollSpeed = 50f;
        
        private bool isRolling = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (quitButton != null)
            {
                quitButton.gameObject.SetActive(false);
                quitButton.onClick.AddListener(QuitGame);
            }
        }

        public void RollCredits()
        {
            if (creditsPanel != null) creditsPanel.SetActive(true);
            isRolling = true;
            StartCoroutine(ScrollRoutine());
        }

        private IEnumerator ScrollRoutine()
        {
            while (isRolling)
            {
                if (scrollingContent != null)
                {
                    scrollingContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
                    
                    // Check if done (arbitrary height check or wait for seconds)
                    if (scrollingContent.anchoredPosition.y > 2000) // Adjust based on content height
                    {
                        isRolling = false;
                        ShowQuitButton();
                    }
                }
                yield return null;
            }
        }

        private void ShowQuitButton()
        {
            if (quitButton != null) quitButton.gameObject.SetActive(true);
        }

        private void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }
}
