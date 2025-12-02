using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Audio
{
    /// <summary>
    /// Manages all game audio: Music, SFX, and Ambience.
    /// Supports cross-fading and loop-based dynamic audio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource uiSource; // Dedicated UI source

        [Header("Mixer Groups (Optional)")]
        [SerializeField] private UnityEngine.Audio.AudioMixerGroup musicGroup;
        [SerializeField] private UnityEngine.Audio.AudioMixerGroup sfxGroup;
        [SerializeField] private UnityEngine.Audio.AudioMixerGroup uiGroup;
        [SerializeField] private UnityEngine.Audio.AudioMixerGroup ambienceGroup;

        [Header("Clips")]
        [SerializeField] private AudioClip mainTheme;
        [SerializeField] private AudioClip loopResetSFX;
        [SerializeField] private AudioClip missionCompleteSFX;
        [SerializeField] private AudioClip clueFoundSFX;

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

        private void Start()
        {
            InitializeSources();
            PlayMusic(mainTheme);
            
            // Subscribe to events
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopReset.AddListener(() => PlaySFX(loopResetSFX));
            }
        }

        private void InitializeSources()
        {
            if (musicSource) musicSource.outputAudioMixerGroup = musicGroup;
            if (sfxSource) sfxSource.outputAudioMixerGroup = sfxGroup;
            if (ambienceSource) ambienceSource.outputAudioMixerGroup = ambienceGroup;
            if (uiSource == null && sfxSource != null) uiSource = sfxSource; // Fallback
            if (uiSource) uiSource.outputAudioMixerGroup = uiGroup;
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
        {
            if (clip == null) return;
            if (musicSource.clip == clip) return;

            // Simple switch for now, could add coroutine for fading
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void SetAmbience(AudioClip clip)
        {
            if (clip == null) return;
            if (ambienceSource.clip == clip) return;

            ambienceSource.clip = clip;
            ambienceSource.Play();
        }
        public void PlayCue(AudioCue cue, Vector3 position = default)
        {
            if (cue == null) return;

            AudioClip clip = cue.GetClip();
            if (clip == null) return;

            // If position is default (zero), play 2D, otherwise play at location
            if (position == Vector3.zero)
            {
                sfxSource.pitch = cue.GetPitch();
                sfxSource.PlayOneShot(clip, cue.GetVolume());
                sfxSource.pitch = 1f; // Reset
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, position, cue.GetVolume());
            }
        }

        public void PlayUICue(AudioCue cue)
        {
            if (cue == null || uiSource == null) return;
            
            uiSource.pitch = cue.GetPitch();
            uiSource.PlayOneShot(cue.GetClip(), cue.GetVolume());
            uiSource.pitch = 1f;
        }

        public void PlayUISound(AudioClip clip)
        {
            if (clip == null) return;
            if (uiSource != null) uiSource.PlayOneShot(clip, 0.8f);
            else sfxSource.PlayOneShot(clip, 0.8f);
        }
    }
}
