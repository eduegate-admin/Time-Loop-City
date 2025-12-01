using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.AI
{
    /// <summary>
    /// Component that gives NPCs the ability to remember previous loops.
    /// Memory triggers special dialogue and actions after certain loops.
    /// </summary>
    public class NPCMemoryComponent : MonoBehaviour
    {
        [Header("Memory Settings")]
        [SerializeField] private bool canRemember = false;
        [SerializeField] private int loopThresholdToRemember = 3; // Remembers after 3 loops

        [Header("Memory Dialogue")]
        [SerializeField] private List<string> normalDialogue = new List<string>();
        [SerializeField] private List<string> memoryDialogue = new List<string>();

        [Header("Special Actions")]
        [SerializeField] private bool revealsSecretAfterMemory = false;
        [SerializeField] private GameObject secretObject; // Object to reveal when remembering

        private bool hasRevealed = false;

        private void Start()
        {
            // Subscribe to loop changes
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopCountChanged.AddListener(OnLoopCountChanged);
            }
        }

        /// <summary>
        /// Check if NPC remembers previous loops
        /// </summary>
        public bool RemembersPreviousLoops()
        {
            if (!canRemember) return false;

            int currentLoop = TimeLoop.TimeLoopManager.Instance?.CurrentLoopCount ?? 0;
            return currentLoop >= loopThresholdToRemember;
        }

        /// <summary>
        /// Get appropriate dialogue based on memory state
        /// </summary>
        public string GetMemoryDialogue()
        {
            if (RemembersPreviousLoops() && memoryDialogue.Count > 0)
            {
                return memoryDialogue[Random.Range(0, memoryDialogue.Count)];
            }
            else if (normalDialogue.Count > 0)
            {
                return normalDialogue[Random.Range(0, normalDialogue.Count)];
            }
            return "...";
        }

        /// <summary>
        /// Called when loop count changes
        /// </summary>
        private void OnLoopCountChanged(int newLoopCount)
        {
            if (RemembersPreviousLoops() && !hasRevealed)
            {
                RevealSecret();
            }
        }

        /// <summary>
        /// Reveal secret when NPC starts remembering
        /// </summary>
        private void RevealSecret()
        {
            if (!revealsSecretAfterMemory || hasRevealed) return;

            Debug.Log($"[NPCMemoryComponent] {gameObject.name} now remembers and reveals a secret!");

            if (secretObject != null)
            {
                secretObject.SetActive(true);
            }

            hasRevealed = true;

            // Could trigger special animation or visual effect here
            ShowMemoryEffect();
        }

        /// <summary>
        /// Visual effect when NPC remembers
        /// </summary>
        private void ShowMemoryEffect()
        {
            // TODO: Add particle effect or visual indicator
            Debug.Log($"[NPCMemoryComponent] {gameObject.name} has a memory flash!");
        }

        /// <summary>
        /// Trigger special action when player talks to remembering NPC
        /// </summary>
        public void TriggerMemoryAction()
        {
            if (RemembersPreviousLoops())
            {
                Debug.Log($"[NPCMemoryComponent] {gameObject.name} shares important information!");
                // Could unlock quest, give item, reveal location, etc.
            }
        }
    }
}
