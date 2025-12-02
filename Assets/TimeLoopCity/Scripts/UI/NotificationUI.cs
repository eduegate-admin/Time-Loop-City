using UnityEngine;
using TMPro;
using System.Collections;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// Handles visual notifications for mission updates and clues.
    /// </summary>
    public class NotificationUI : MonoBehaviour
    {
        public static NotificationUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private AudioClip notificationSound;

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
            if (notificationPanel != null) notificationPanel.SetActive(false);
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }

        public void ShowNotification(string title, string message)
        {
            StopAllCoroutines();
            StartCoroutine(ShowRoutine(title, message));
        }

        private IEnumerator ShowRoutine(string title, string message)
        {
            // Setup
            if (notificationPanel != null) notificationPanel.SetActive(true);
            if (titleText != null) titleText.text = title;
            if (messageText != null) messageText.text = message;

            // Sound
            if (Audio.AudioManager.Instance != null && notificationSound != null)
            {
                Audio.AudioManager.Instance.PlaySFX(notificationSound);
            }

            // Fade In
            yield return StartCoroutine(Fade(0f, 1f));

            // Wait
            yield return new WaitForSeconds(displayDuration);

            // Fade Out
            yield return StartCoroutine(Fade(1f, 0f));

            if (notificationPanel != null) notificationPanel.SetActive(false);
        }

        private IEnumerator Fade(float start, float end)
        {
            if (canvasGroup == null) yield break;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(start, end, t);
                yield return null;
            }
            canvasGroup.alpha = end;
        }
    }
}
