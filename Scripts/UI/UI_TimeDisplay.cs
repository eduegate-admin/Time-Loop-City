using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// Displays current time and loop count on screen.
    /// </summary>
    public class UI_TimeDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI loopCountText;
        [SerializeField] private Image timeProgressBar;

        [Header("Settings")]
        [SerializeField] private bool show24HourFormat = false;

        private void Update()
        {
            UpdateTimeDisplay();
        }

        /// <summary>
        /// Update time display UI
        /// </summary>
        private void UpdateTimeDisplay()
        {
            if (TimeLoop.TimeLoopManager.Instance == null) return;

            // Update loop count
            if (loopCountText != null)
            {
                int loopCount = TimeLoop.TimeLoopManager.Instance.CurrentLoopCount;
                loopCountText.text = $"Loop: {loopCount}";
            }

            // Update time progress bar
            if (timeProgressBar != null)
            {
                float progress = TimeLoop.TimeLoopManager.Instance.LoopProgress;
                timeProgressBar.fillAmount = progress;
            }

            // Update time of day text
            if (timeText != null)
            {
                World.TimeOfDaySystem timeSystem = FindObjectOfType<World.TimeOfDaySystem>();
                if (timeSystem != null)
                {
                    float hour = timeSystem.GetCurrentHour();
                    int displayHour = Mathf.FloorToInt(hour);
                    int displayMinute = Mathf.FloorToInt((hour - displayHour) * 60f);

                    if (show24HourFormat)
                    {
                        timeText.text = $"{displayHour:00}:{displayMinute:00}";
                    }
                    else
                    {
                        string period = displayHour >= 12 ? "PM" : "AM";
                        int hour12 = displayHour > 12 ? displayHour - 12 : (displayHour == 0 ? 12 : displayHour);
                        timeText.text = $"{hour12}:{displayMinute:00} {period}";
                    }
                }
            }
        }
    }
}
