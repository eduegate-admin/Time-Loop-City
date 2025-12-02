using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Managers
{
    /// <summary>
    /// Manages game-wide state, pausing, and scene transitions.
    /// Acts as the central hub for non-loop specific game logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private bool isPaused = false;
        [SerializeField] private bool isGameActive = true;

        [Header("References")]
        [SerializeField] private GameObject pauseMenuUI;

        public bool IsPaused => isPaused;
        public bool IsGameActive => isGameActive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (!isGameActive) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(isPaused);
            }

            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isPaused;

            Debug.Log($"[GameManager] Game {(isPaused ? "Paused" : "Resumed")}");
        }

        public void QuitGame()
        {
            Debug.Log("[GameManager] Quitting Game...");
            Application.Quit();
        }

        public void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;
            
            // Optional: Show/Hide Pause Menu UI if it existed
        }
    }
}
