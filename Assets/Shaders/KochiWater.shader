Shader "Kochi/Water"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.4, 0.6, 0.8)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveScale ("Wave Scale", Float) = 10.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.9
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        fixed4 _Color;
        float _WaveSpeed;
        float _WaveScale;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Simple procedural waves
            float wave = sin(IN.worldPos.x * _WaveScale + _Time.y * _WaveSpeed) * 
                         cos(IN.worldPos.z * _WaveScale + _Time.y * _WaveSpeed * 0.5);
            
            // Perturb normal based on wave
            o.Normal = normalize(float3(wave * 0.1, 1, wave * 0.1));
            
            o.Albedo = _Color.rgb + (wave * 0.05);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
