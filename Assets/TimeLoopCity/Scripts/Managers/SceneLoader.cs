using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace TimeLoopCity.Managers
{
    /// <summary>
    /// Handles scene transitions with a loading screen or fade effect.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private UnityEngine.UI.Slider progressBar;

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

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadAsync(sceneName));
        }

        private IEnumerator LoadAsync(string sceneName)
        {
            if (loadingScreen != null) loadingScreen.SetActive(true);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

            while (!op.isDone)
            {
                if (progressBar != null)
                {
                    progressBar.value = Mathf.Clamp01(op.progress / 0.9f);
                }
                yield return null;
            }

            if (loadingScreen != null) loadingScreen.SetActive(false);
        }
    }
}
