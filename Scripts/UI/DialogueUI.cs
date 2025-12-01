using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace TimeLoopCity.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        
        [Header("Choices")]
        [SerializeField] private Transform choiceContainer;
        [SerializeField] private GameObject choiceButtonPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        public void ShowDialogue(string npcName)
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            if (nameText != null) nameText.text = npcName;
            
            // Clear previous choices
            ClearChoices();
            
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
                continueButton.onClick.RemoveAllListeners();
            }
        }

        public void HideDialogue()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        public void SetDialogueText(string text)
        {
            if (dialogueText != null) dialogueText.text = text;
        }

        public IEnumerator TypeSentence(string sentence)
        {
            if (dialogueText != null)
            {
                dialogueText.text = "";
                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText.text += letter;
                    yield return null;
                }
            }
        }

        public void ShowChoices(List<Dialogue.DialogueChoice> choices, System.Action<Dialogue.DialogueChoice> onChoiceSelected)
        {
            if (continueButton != null) continueButton.gameObject.SetActive(false);
            ClearChoices();

            if (choiceContainer == null || choiceButtonPrefab == null) return;

            foreach (var choice in choices)
            {
                GameObject btnObj = Instantiate(choiceButtonPrefab, choiceContainer);
                var btn = btnObj.GetComponent<Button>();
                var txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();

                if (txt != null) txt.text = choice.choiceText;
                
                if (btn != null)
                {
                    btn.onClick.AddListener(() => onChoiceSelected(choice));
                }
            }
        }

        private void ClearChoices()
        {
            if (choiceContainer == null) return;
            foreach (Transform child in choiceContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void SetContinueAction(UnityEngine.Events.UnityAction action)
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(action);
            }
        }
    }
}
