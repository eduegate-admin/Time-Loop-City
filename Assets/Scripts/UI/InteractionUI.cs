using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TimeLoopCity.UI
{
    public class InteractionUI : MonoBehaviour
    {
        public static InteractionUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image crosshair;
        [SerializeField] private Color defaultCrosshairColor = Color.white;
        [SerializeField] private Color activeCrosshairColor = Color.cyan;

        [Header("Polish")]
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private float popAnimationDuration = 0.2f;
        private Vector3 originalScale;

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
            if (promptPanel != null) originalScale = promptPanel.transform.localScale;
            HidePrompt();
            if (crosshair != null) crosshair.color = defaultCrosshairColor;
        }

        public void ShowPrompt(string message)
        {
            if (promptPanel != null)
            {
                if (!promptPanel.activeSelf)
                {
                    // Play sound on first show
                    if (Audio.AudioManager.Instance != null && hoverSound != null)
                        Audio.AudioManager.Instance.PlaySFX(hoverSound, 0.5f);
                    
                    // Pop animation
                    StopAllCoroutines();
                    StartCoroutine(PopIn());
                }
                promptPanel.SetActive(true);
            }
            
            if (promptText != null) promptText.text = message;
            if (crosshair != null) crosshair.color = activeCrosshairColor;
        }

        private System.Collections.IEnumerator PopIn()
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / popAnimationDuration;
                float scale = Mathf.Lerp(0.5f, 1.1f, t); // Overshoot slightly
                if (promptPanel != null) promptPanel.transform.localScale = originalScale * scale;
                yield return null;
            }
            // Settle
            if (promptPanel != null) promptPanel.transform.localScale = originalScale;
        }

        public void HidePrompt()
        {
            if (promptPanel != null) promptPanel.SetActive(false);
            if (crosshair != null) crosshair.color = defaultCrosshairColor;
        }
    }
}
