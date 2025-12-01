using UnityEngine;
using TimeLoopCity.Player;
using TimeLoopCity.UI;

namespace TimeLoopCity.Managers
{
    /// <summary>
    /// Centralizes interaction logic. Handles raycasting and UI updates.
    /// </summary>
    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float interactionRange = 3.0f;
        [SerializeField] private LayerMask interactableLayer;

        private Camera mainCamera;
        private InteractableObject currentInteractable;

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
            mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleRaycast();
            HandleInput();
        }

        private void HandleRaycast()
        {
            if (mainCamera == null) return;

            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                
                if (interactable != null && interactable != currentInteractable)
                {
                    // New interactable found
                    if (currentInteractable != null) currentInteractable.ShowInteractionUI(false);
                    
                    currentInteractable = interactable;
                    currentInteractable.ShowInteractionUI(true);
                    
                    // Update UI
                    InteractionUI.Instance?.ShowPrompt(currentInteractable.InteractionPrompt);
                }
            }
            else
            {
                // Nothing found
                if (currentInteractable != null)
                {
                    currentInteractable.ShowInteractionUI(false);
                    currentInteractable = null;
                    InteractionUI.Instance?.HidePrompt();
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            {
                // We need the player controller reference. 
                // For now, finding it is okay, or we can pass null if Interact doesn't strictly need it immediately.
                // Better approach: PlayerController calls InteractionManager, or we cache PlayerController.
                PlayerController player = FindObjectOfType<PlayerController>(); 
                currentInteractable.Interact(player);
            }
        }
    }
}
