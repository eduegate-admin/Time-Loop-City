using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TimeLoopCity.Managers;
using TimeLoopCity.Core;
using TimeLoopCity.UI;

namespace TimeLoopCity.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private Queue<string> sentences;
        private DialogueData currentDialogue;
        private bool isDialogueActive = false;
        public bool IsDialogueActive => isDialogueActive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            sentences = new Queue<string>();
        }

        public void StartDialogue(DialogueData dialogue)
        {
            if (dialogue == null) return;

            currentDialogue = dialogue;
            isDialogueActive = true;
            
            // Pause game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetPaused(true);
            }

            // Init UI
            DialogueUI.Instance?.ShowDialogue(dialogue.npcName);
            DialogueUI.Instance?.SetContinueAction(DisplayNextSentence);

            sentences.Clear();
            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (sentences.Count == 0)
            {
                // Check for choices before ending
                if (currentDialogue.choices != null && currentDialogue.choices.Count > 0)
                {
                    DialogueUI.Instance?.ShowChoices(currentDialogue.choices, OnChoiceSelected);
                }
                else
                {
                    EndDialogue();
                }
                return;
            }

            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            if (DialogueUI.Instance != null)
            {
                StartCoroutine(DialogueUI.Instance.TypeSentence(sentence));
            }
        }

        private void OnChoiceSelected(DialogueChoice choice)
        {
            // Handle choice event
            if (!string.IsNullOrEmpty(choice.triggerEventId))
            {
                Debug.Log($"[Dialogue] Trigger Event: {choice.triggerEventId}");
                // EventManager.Trigger(choice.triggerEventId); // If EventManager existed
            }

            // Go to next dialogue or end
            if (choice.nextDialogue != null)
            {
                StartDialogue(choice.nextDialogue);
            }
            else
            {
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            isDialogueActive = false;
            DialogueUI.Instance?.HideDialogue();

            // Handle post-dialogue effects
            if (currentDialogue != null)
            {
                // Give clue
                if (!string.IsNullOrEmpty(currentDialogue.giveClueId))
                {
                    PersistentClueSystem.Instance?.DiscoverClue(currentDialogue.giveClueId);
                }

                // Missions
                if (currentDialogue.startMission != null)
                {
                    MissionManager.Instance?.StartMission(currentDialogue.startMission);
                }
                if (currentDialogue.completeMission != null)
                {
                    MissionManager.Instance?.CompleteMission(currentDialogue.completeMission);
                }
            }

            // Unpause
            GameManager.Instance?.SetPaused(false);
            
            Debug.Log("[DialogueManager] End of conversation.");
        }
    }
}
