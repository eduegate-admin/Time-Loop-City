Shader "Kochi/Building"
{
    Properties
    {
        _WallColor ("Wall Color", Color) = (0.8, 0.8, 0.8, 1)
        _WindowColor ("Window Color", Color) = (0.1, 0.2, 0.3, 1)
        _WindowEmission ("Window Emission", Color) = (0.5, 0.5, 0.2, 1)
        _FloorHeight ("Floor Height", Float) = 3.0
        _WindowWidth ("Window Width Ratio", Range(0,1)) = 0.5
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
            float3 worldPos;
            float3 worldNormal;
        };

        fixed4 _WallColor;
        fixed4 _WindowColor;
        fixed4 _WindowEmission;
        float _FloorHeight;
        float _WindowWidth;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Triplanar-ish logic for windows
            // Use world position to determine if we are in a "window" zone
            
            float y = IN.worldPos.y;
            float x = IN.worldPos.x;
            float z = IN.worldPos.z;
            
            // Determine which axis we are facing
            float2 pos = (abs(IN.worldNormal.y) > 0.5) ? float2(x, z) : 
                         (abs(IN.worldNormal.x) > 0.5) ? float2(z, y) : float2(x, y);

            // Grid pattern
            float floorIndex = floor(y / _FloorHeight);
            float floorPos = frac(y / _FloorHeight);
            
            // Horizontal repetition
            float width = 3.0; // Assume 3m width per room
            float roomPos = frac((abs(IN.worldNormal.y) > 0.5 ? x : (abs(IN.worldNormal.x) > 0.5 ? z : x)) / width);

            bool isWindow = false;
            
            // Simple window logic: Middle of the room horizontally, middle of floor vertically
            if (floorPos > 0.3 && floorPos < 0.8) // Vertical window range
            {
                if (roomPos > (1 - _WindowWidth)/2 && roomPos < 1 - (1 - _WindowWidth)/2)
                {
                    isWindow = true;
                }
            }
            
            // Don't put windows on roof/ground (Y normal)
            if (abs(IN.worldNormal.y) > 0.9) isWindow = false;

            fixed4 c = isWindow ? _WindowColor : _WallColor;
            
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = isWindow ? 0.9 : _Glossiness;
            o.Alpha = c.a;
            
            // Randomly light up windows at night (simulated by emission)
            if (isWindow)
            {
                // Random based on position
                float random = frac(sin(dot(floor(IN.worldPos / 3.0), float2(12.9898,78.233))) * 43758.5453);
                if (random > 0.7)
                {
                    o.Emission = _WindowEmission;
                }
            }
        }
        ENDCG
    }
    FallBack "Standard"
}
