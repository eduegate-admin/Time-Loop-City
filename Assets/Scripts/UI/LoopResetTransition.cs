using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// Handles visual transition when loop resets (fade to black/white).
    /// </summary>
    public class LoopResetTransition : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Transition Settings")]
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private Color fadeColor = Color.black;

        private void Start()
        {
            // Subscribe to loop reset event
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopReset.AddListener(PlayResetTransition);
            }

            // Initialize fade image
            if (fadeImage != null)
            {
                fadeImage.color = fadeColor;
            }

            // Start transparent
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Play reset transition effect
        /// </summary>
        private void PlayResetTransition()
        {
            StartCoroutine(FadeTransition());
        }

        /// <summary>
        /// Fade in and out coroutine
        /// </summary>
        private IEnumerator FadeTransition()
        {
            if (canvasGroup == null) yield break;

            // Fade to black
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;

            // Hold for a moment
            yield return new WaitForSeconds(0.5f);

            // Fade back to transparent
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// Manually trigger transition (for testing or special cases)
        /// </summary>
        public void TriggerTransition()
        {
            StartCoroutine(FadeTransition());
        }
    }
}
