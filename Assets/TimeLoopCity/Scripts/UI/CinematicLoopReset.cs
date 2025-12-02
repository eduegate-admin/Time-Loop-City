using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// Handles the cinematic transition when the loop resets.
    /// Coordinates screen fade, audio, and player teleportation.
    /// </summary>
    public class CinematicLoopReset : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private Image fadeImage;

        [Header("Timing")]
        [SerializeField] private float fadeOutDuration = 1.0f;
        [SerializeField] private float blackScreenDuration = 2.0f;
        [SerializeField] private float fadeInDuration = 1.5f;

        [Header("Audio")]
        [SerializeField] private AudioClip resetSound;
        [SerializeField] private AudioClip wakeUpSound;

        private void Start()
        {
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }

            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopReset.AddListener(PlayResetSequence);
            }
        }

        private void PlayResetSequence()
        {
            StartCoroutine(ResetRoutine());
        }

        private IEnumerator ResetRoutine()
        {
            Debug.Log("[CinematicLoopReset] Starting reset sequence...");

            // 1. Play Reset Sound
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(resetSound);
            }

            // 2. Fade Out (to black)
            yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

            // 3. Hold Black Screen (Player is teleported during this time by TimeLoopManager)
            // TimeLoopManager teleports player immediately on reset, so we are covered.
            yield return new WaitForSeconds(blackScreenDuration);

            // 4. Play Wake Up Sound
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(wakeUpSound);
            }

            // 5. Fade In (from black)
            yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

            Debug.Log("[CinematicLoopReset] Reset sequence complete.");
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            if (fadeCanvasGroup == null) yield break;

            fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                yield return null;
            }

            fadeCanvasGroup.alpha = endAlpha;
            fadeCanvasGroup.blocksRaycasts = (endAlpha > 0f);
        }
    }
}
