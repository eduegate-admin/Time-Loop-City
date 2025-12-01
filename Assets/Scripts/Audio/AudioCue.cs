using UnityEngine;

namespace TimeLoopCity.Audio
{
    /// <summary>
    /// Data container for sound effects.
    /// Allows randomization of pitch and volume for variety.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAudioCue", menuName = "TimeLoopCity/Audio/Audio Cue")]
    public class AudioCue : ScriptableObject
    {
        [Header("Sound Configuration")]
        public AudioClip[] clips;
        
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;

        [Header("Randomization")]
        public bool randomizePitch = true;
        [Range(0f, 0.5f)] public float pitchVariation = 0.1f;
        
        public bool randomizeVolume = true;
        [Range(0f, 0.2f)] public float volumeVariation = 0.05f;

        public AudioClip GetClip()
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }

        public float GetPitch()
        {
            if (!randomizePitch) return pitch;
            return pitch + Random.Range(-pitchVariation, pitchVariation);
        }

        public float GetVolume()
        {
            if (!randomizeVolume) return volume;
            return Mathf.Clamp01(volume + Random.Range(-volumeVariation, volumeVariation));
        }
    }
}
