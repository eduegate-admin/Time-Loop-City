using UnityEngine;
using TimeLoopCity.UI;

namespace TimeLoopCity.Player
{
    /// <summary>
    /// Specialized interactable for reading items (Newspapers, Notes).
    /// </summary>
    public class ClueInteractable : InteractableObject
    {
        [Header("Readable Content")]
        [SerializeField] private string title;
        [TextArea(5, 10)]
        [SerializeField] private string content;

        protected override void OnInteract(PlayerController player)
        {
            base.OnInteract(player);

            // Open Readable UI
            ReadableUI.Instance?.ShowDocument(title, content);
        }
    }
}
