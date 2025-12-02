using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace TimeLoopCity.Kochi.Water
{
    [ExecuteAlways]
    public class WaterSystem : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private float waveAmplitude = 0.5f;
        [SerializeField] private float waveFrequency = 1f;
        [SerializeField] private float waveSpeed = 2f;
        [SerializeField] private Vector2 waveDirection = Vector2.right;

        [Header("Water Appearance")]
        [SerializeField] private Color shallowColor = new Color(0.1f, 0.6f, 0.4f, 0.7f);
        [SerializeField] private Color deepColor = new Color(0.05f, 0.2f, 0.3f, 0.9f);
        [SerializeField] private float depthFadeDistance = 50f;

        [Header("Foam Settings")]
        [SerializeField] private float foamStrength = 0.3f;

        [Header("Reflection/Refraction")]
        [SerializeField] private float reflectionStrength = 0.5f;
        [SerializeField] private float refractionStrength = 0.3f;

        private Material waterMaterial;
        private List<Rigidbody> buoyancyObjects = new List<Rigidbody>();

        private static WaterSystem instance;
        public static WaterSystem Instance => instance;

        private void OnEnable()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                DestroyImmediate(gameObject);

            InitializeWaterSystem();
        }

        private void OnDisable()
        {
            if (instance == this)
                instance = null;
        }

        private void InitializeWaterSystem()
        {
            if (GetComponent<MeshRenderer>() == null)
                gameObject.AddComponent<MeshRenderer>();

            waterMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            waterMaterial.name = "KochiWater";
            waterMaterial.SetColor("_BaseColor", shallowColor);
            waterMaterial.SetFloat("_Smoothness", 0.9f);
            waterMaterial.SetFloat("_Metallic", 0.1f);

            GetComponent<MeshRenderer>().material = waterMaterial;
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;

            UpdateMaterialProperties();
            UpdateBuoyancyObjects();
        }

        private void UpdateMaterialProperties()
        {
            if (waterMaterial == null)
                return;

            waterMaterial.SetFloat("_Time", Time.time);
            waterMaterial.SetFloat("_WaveAmplitude", waveAmplitude);
            waterMaterial.SetFloat("_WaveFrequency", waveFrequency);
            waterMaterial.SetVector("_WaveDirection", waveDirection.normalized);
            waterMaterial.SetColor("_ShallowColor", shallowColor);
            waterMaterial.SetColor("_DeepColor", deepColor);
            waterMaterial.SetFloat("_DepthFadeDistance", depthFadeDistance);
            waterMaterial.SetFloat("_FoamStrength", foamStrength);
            waterMaterial.SetFloat("_ReflectionStrength", reflectionStrength);
            waterMaterial.SetFloat("_RefractionStrength", refractionStrength);
        }

        private void UpdateBuoyancyObjects()
        {
            for (int i = buoyancyObjects.Count - 1; i >= 0; i--)
            {
                if (buoyancyObjects[i] == null)
                    buoyancyObjects.RemoveAt(i);
                else
                    ApplyBuoyancyForce(buoyancyObjects[i]);
            }
        }

        private void ApplyBuoyancyForce(Rigidbody rb)
        {
            if (rb == null) return;

            float waveHeight = GetWaveHeightAtPosition(rb.position);
            if (rb.position.y < waveHeight)
            {
                float depth = waveHeight - rb.position.y;
                Vector3 buoyancyForce = Vector3.up * depth * rb.mass * 9.81f;
                rb.AddForce(buoyancyForce, ForceMode.Force);
            }
        }

        public void RegisterBuoyancyObject(Rigidbody rigidbody)
        {
            if (!buoyancyObjects.Contains(rigidbody))
                buoyancyObjects.Add(rigidbody);
        }

        public void UnregisterBuoyancyObject(Rigidbody rigidbody)
        {
            buoyancyObjects.Remove(rigidbody);
        }

        public float GetWaveHeightAtPosition(Vector3 worldPosition)
        {
            Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint(worldPosition);
            
            float phase = (localPos.x * waveDirection.x + localPos.z * waveDirection.y) * waveFrequency - Time.time * waveSpeed;
            float height = Mathf.Sin(phase) * waveAmplitude;
            
            Vector2 perpDir = new Vector2(-waveDirection.y, waveDirection.x);
            float phase2 = (localPos.x * perpDir.x + localPos.z * perpDir.y) * waveFrequency * 0.5f - Time.time * waveSpeed * 0.7f;
            height += Mathf.Sin(phase2) * waveAmplitude * 0.5f;

            return transform.TransformPoint(new Vector3(localPos.x, height, localPos.z)).y;
        }

        public Vector3 GetWaterNormalAtPosition(Vector3 worldPosition)
        {
            Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint(worldPosition);
            
            float phase = (localPos.x * waveDirection.x + localPos.z * waveDirection.y) * waveFrequency - Time.time * waveSpeed;
            float phase2 = (localPos.x * (-waveDirection.y) + localPos.z * waveDirection.x) * waveFrequency * 0.5f - Time.time * waveSpeed * 0.7f;

            Vector3 normal = new Vector3(
                -Mathf.Cos(phase) * waveFrequency * waveAmplitude - Mathf.Cos(phase2) * waveFrequency * 0.5f * waveAmplitude * 0.5f,
                1f,
                -Mathf.Cos(phase) * waveFrequency * waveAmplitude - Mathf.Cos(phase2) * waveFrequency * 0.5f * waveAmplitude * 0.5f
            ).normalized;

            return transform.TransformDirection(normal);
        }

        public bool IsPositionInWater(Vector3 worldPosition)
        {
            float waveHeight = GetWaveHeightAtPosition(worldPosition);
            return worldPosition.y <= waveHeight;
        }
    }
}
