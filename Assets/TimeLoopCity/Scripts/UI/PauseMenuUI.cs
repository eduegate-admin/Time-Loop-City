using UnityEngine;
using UnityEngine.UI;
using TimeLoopCity.Managers;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// Main pause menu controller.
    /// Shows when ESC is pressed during gameplay.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Instance { get; private set; }

        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button settingsBackButton;

        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Dropdown resolutionDropdown;

        private bool isPaused = false;

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
            // Setup buttons
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(ShowSettings);
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
            if (settingsBackButton != null)
                settingsBackButton.onClick.AddListener(HideSettings);

            // Setup volume sliders
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

            // Setup graphics toggles
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

            HidePauseMenu();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        public void Pause()
        {
            isPaused = true;
            ShowPauseMenu();
            GameManager.Instance?.SetPaused(true);
        }

        public void Resume()
        {
            isPaused = false;
            HidePauseMenu();
            GameManager.Instance?.SetPaused(false);
        }

        private void ShowPauseMenu()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void HidePauseMenu()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void ShowSettings()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        private void HideSettings()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void GoToMainMenu()
        {
            GameManager.Instance?.SetPaused(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void SetMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        private void SetMusicVolume(float value)
        {
            // Would control music source volume specifically
            Debug.Log($"Music Volume: {value}");
        }

        private void SetSFXVolume(float value)
        {
            // Would control SFX source volume specifically
            Debug.Log($"SFX Volume: {value}");
        }

        private void SetFullscreen(bool value)
        {
            Screen.fullScreen = value;
        }
    }
}
