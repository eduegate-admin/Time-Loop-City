using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace TimeLoopCity.Managers
{
    /// <summary>
    /// Handles saving and loading persistent data (clues and meta info only).
    /// Does NOT save world state - that resets each loop.
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance { get; private set; }

        private const string SAVE_FILE_NAME = "timeloop_save.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Ensure this GameObject is a root object before DontDestroyOnLoad
            if (transform.parent != null)
            {
                Debug.LogWarning("[SaveLoadManager] DontDestroyOnLoad only works on root GameObjects. " +
                    "Please move SaveLoadManager to the scene root (not as a child of another GameObject).");
                transform.SetParent(null);
            }
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadGame();
        }

        /// <summary>
        /// Save clues to disk
        /// </summary>
        public void SaveClues(HashSet<string> clues)
        {
            SaveData data = new SaveData
            {
                discoveredClues = new List<string>(clues),
                saveTimestamp = System.DateTime.Now.ToString()
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveLoadManager] Game saved to {SavePath}");
        }

        /// <summary>
        /// Load game data
        /// </summary>
        public void LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[SaveLoadManager] No save file found, starting fresh");
                return;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // Load clues into TimeLoopManager
                HashSet<string> clues = new HashSet<string>(data.discoveredClues);
                if (TimeLoop.TimeLoopManager.Instance != null)
                {
                    TimeLoop.TimeLoopManager.Instance.LoadClues(clues);
                }

                Debug.Log($"[SaveLoadManager] Game loaded: {data.discoveredClues.Count} clues restored");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoadManager] Failed to load save: {e.Message}");
            }
        }

        /// <summary>
        /// Delete save file
        /// </summary>
        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("[SaveLoadManager] Save file deleted");
            }
        }
    }

    /// <summary>
    /// Serializable save data structure
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public List<string> discoveredClues = new List<string>();
        public string saveTimestamp;
    }
}
