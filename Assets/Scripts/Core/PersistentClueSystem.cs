using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Core
{
    /// <summary>
    /// Manages clues and objects that persist across time loops.
    /// Players can interact with clues to unlock new information.
    /// </summary>
    public class PersistentClueSystem : MonoBehaviour
    {
        public static PersistentClueSystem Instance { get; private set; }

        [Header("Clue Settings")]
        [SerializeField] private ClueDatabase clueDatabase;

        // Runtime discovered clues
        private HashSet<string> discoveredClues = new HashSet<string>();

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
            // Load clues from save
            LoadCluesFromSave();
        }

        /// <summary>
        /// Discover a new clue
        /// </summary>
        public void DiscoverClue(string clueId)
        {
            if (discoveredClues.Add(clueId))
            {
                Debug.Log($"[PersistentClueSystem] New clue discovered: {clueId}");

                // Notify TimeLoopManager
                if (TimeLoop.TimeLoopManager.Instance != null)
                {
                    TimeLoop.TimeLoopManager.Instance.AddClue(clueId);
                }

                // Show UI notification
                ShowClueNotification(clueId);
            }
        }

        /// <summary>
        /// Check if player has discovered a clue
        /// </summary>
        public bool HasClue(string clueId)
        {
            return discoveredClues.Contains(clueId);
        }

        /// <summary>
        /// Get all discovered clues
        /// </summary>
        public HashSet<string> GetAllClues()
        {
            return new HashSet<string>(discoveredClues);
        }

        /// <summary>
        /// Load clues from save system
        /// </summary>
        private void LoadCluesFromSave()
        {
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                discoveredClues = TimeLoop.TimeLoopManager.Instance.GetAllClues();
                Debug.Log($"[PersistentClueSystem] Loaded {discoveredClues.Count} clues from save");
            }
        }

        /// <summary>
        /// Show UI notification for new clue
        /// </summary>
        private void ShowClueNotification(string clueId)
        {
            // TODO: Implement UI notification
            Debug.Log($"[UI] New Clue Unlocked: {clueId}");
        }

        /// <summary>
        /// Get clue data from database
        /// </summary>
        public ClueData GetClueData(string clueId)
        {
            if (clueDatabase != null)
            {
                return clueDatabase.GetClue(clueId);
            }
            return null;
        }
    }

    /// <summary>
    /// ScriptableObject database for clue definitions
    /// </summary>
    [CreateAssetMenu(fileName = "ClueDatabase", menuName = "TimeLoopCity/Clue Database")]
    public class ClueDatabase : ScriptableObject
    {
        [SerializeField] private List<ClueData> clues = new List<ClueData>();

        public ClueData GetClue(string clueId)
        {
            return clues.Find(c => c.clueId == clueId);
        }
    }

    /// <summary>
    /// Data for a single clue
    /// </summary>
    [System.Serializable]
    public class ClueData
    {
        public string clueId;
        public string clueName;
        [TextArea] public string description;
        public Sprite icon;
        public bool isStoryClue;
    }
}
