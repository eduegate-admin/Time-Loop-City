using UnityEngine;
using System.Collections;

namespace TimeLoopCity.Visuals
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        private Vector3 originalPos;
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.7f;
        private float dampingSpeed = 1.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            originalPos = transform.localPosition;
        }

        private void Update()
        {
            if (shakeDuration > 0)
            {
                transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                shakeDuration = 0f;
                transform.localPosition = originalPos;
            }
        }

        public void Shake(float duration, float magnitude)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }
    }
}
