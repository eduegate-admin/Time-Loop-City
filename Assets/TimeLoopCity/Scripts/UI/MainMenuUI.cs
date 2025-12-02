using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TimeLoopCity.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button optionsButton; // Placeholder

        [Header("Scene Names")]
        [SerializeField] private string gameSceneName = "MainCity";

        private void Start()
        {
            if (playButton != null)
                playButton.onClick.AddListener(PlayGame);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        private void PlayGame()
        {
            Debug.Log("[MainMenu] Starting Game...");
            SceneManager.LoadScene(gameSceneName);
        }

        private void QuitGame()
        {
            Debug.Log("[MainMenu] Quitting...");
            Application.Quit();
        }
    }
}
