using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using TimeLoopCity.Core;
using TimeLoopCity.Managers;

namespace TimeLoopCity.UI
{
    public class EvidenceBoardUI : MonoBehaviour
    {
        public static EvidenceBoardUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject boardPanel;
        [SerializeField] private Transform clueContainer;
        [SerializeField] private GameObject clueItemPrefab;
        [SerializeField] private TextMeshProUGUI emptyMessageText;

        private bool isOpen = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (boardPanel != null) boardPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleBoard();
            }
        }

        public void ToggleBoard()
        {
            isOpen = !isOpen;
            
            if (boardPanel != null) boardPanel.SetActive(isOpen);

            if (isOpen)
            {
                RefreshBoard();
                GameManager.Instance?.SetPaused(true);
            }
            else
            {
                GameManager.Instance?.SetPaused(false);
            }
        }

        private void RefreshBoard()
        {
            if (clueContainer == null || clueItemPrefab == null) return;

            // Clear existing
            foreach (Transform child in clueContainer)
            {
                Destroy(child.gameObject);
            }

            // Get clues
            var clues = PersistentClueSystem.Instance?.GetAllClues();
            
            if (clues == null || clues.Count == 0)
            {
                if (emptyMessageText != null) emptyMessageText.gameObject.SetActive(true);
                return;
            }

            if (emptyMessageText != null) emptyMessageText.gameObject.SetActive(false);

            foreach (string clueId in clues)
            {
                var data = PersistentClueSystem.Instance.GetClueData(clueId);
                if (data != null)
                {
                    GameObject obj = Instantiate(clueItemPrefab, clueContainer);
                    
                    // Setup UI elements (assuming prefab structure)
                    var texts = obj.GetComponentsInChildren<TextMeshProUGUI>();
                    if (texts.Length > 0) texts[0].text = data.clueName; // Title
                    if (texts.Length > 1) texts[1].text = data.description; // Desc
                    
                    var img = obj.GetComponentInChildren<Image>();
                    if (img != null && data.icon != null) img.sprite = data.icon;
                }
            }
        }
    }
}
