using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// UI for reading documents, newspapers, and notes.
    /// Pauses the game while open.
    /// </summary>
    public class ReadableUI : MonoBehaviour
    {
        public static ReadableUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (panel != null) panel.SetActive(false);
            if (closeButton != null) closeButton.onClick.AddListener(CloseDocument);
        }

        public void ShowDocument(string title, string content)
        {
            if (panel != null) panel.SetActive(true);
            if (titleText != null) titleText.text = title;
            if (contentText != null) contentText.text = content;

            // Pause Game
            Managers.GameManager.Instance?.SetPaused(true);
        }

        public void CloseDocument()
        {
            if (panel != null) panel.SetActive(false);

            // Unpause Game
            Managers.GameManager.Instance?.SetPaused(false);
        }
    }
}
