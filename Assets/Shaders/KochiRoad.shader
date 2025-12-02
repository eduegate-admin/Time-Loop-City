Shader "Kochi/Road"
{
    Properties
    {
        _Color ("Asphalt Color", Color) = (0.2, 0.2, 0.2, 1)
        _MarkingColor ("Marking Color", Color) = (1, 1, 1, 1)
        _LaneWidth ("Lane Width", Float) = 0.1
        _DashLength ("Dash Length", Float) = 2.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        fixed4 _Color;
        fixed4 _MarkingColor;
        float _LaneWidth;
        float _DashLength;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base Asphalt
            fixed4 c = _Color;
            
            // Procedural Lane Markings (Center Line)
            // Assuming UV.x is 0-1 across width, UV.y is length
            float centerDist = abs(IN.uv_MainTex.x - 0.5);
            
            if (centerDist < _LaneWidth * 0.5)
            {
                // Dash pattern
                float dash = sin(IN.uv_MainTex.y * _DashLength * 3.14159);
                if (dash > 0)
                {
                    c = _MarkingColor;
                }
            }

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
