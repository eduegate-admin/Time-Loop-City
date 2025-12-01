using UnityEngine;
using TimeLoopCity.Core;
using TimeLoopCity.Missions;

namespace TimeLoopCity.Player
{
    /// <summary>
    /// Base class for objects the player can interact with.
    /// Used for doors, clues, items, NPCs, etc.
    /// </summary>
    public class InteractableObject : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private string interactionPrompt = "Press E to interact";
        [SerializeField] private bool canInteract = true;
        [SerializeField] private bool isClue = false;
        [SerializeField] private string clueId;

        public string InteractionPrompt => interactionPrompt;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject highlightEffect;

        /// <summary>
        /// Called when player interacts with this object
        /// </summary>
        public virtual void Interact(PlayerController player)
        {
            if (!canInteract) return;

            Debug.Log($"[InteractableObject] Interacted with {gameObject.name}");

            // If this is a clue, register it
            if (isClue && !string.IsNullOrEmpty(clueId))
            {
            if (isClue && !string.IsNullOrEmpty(clueId))
            {
                PersistentClueSystem.Instance?.DiscoverClue(clueId);
                
                // Also report as mission objective
                MissionManager.Instance?.OnObjectiveEvent(clueId, MissionObjectiveType.FindClue);
            }

            // Report generic interaction objective
            MissionManager.Instance?.OnObjectiveEvent(gameObject.name, MissionObjectiveType.VisitLocation);
            
            // Override this method in derived classes for custom behavior
            OnInteract(player);
        }

        /// <summary>
        /// Override this for custom interaction behavior
        /// </summary>
        protected virtual void OnInteract(PlayerController player)
        {
            // Default behavior: print message
            Debug.Log($"Interacted with {gameObject.name}");
        }

        /// <summary>
        /// Show interaction UI when player is nearby
        /// </summary>
        public void ShowInteractionUI(bool show)
        {
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(show);
            }
        }

        /// <summary>
        /// Enable or disable interaction
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            canInteract = interactable;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw interaction sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}
