Shader "Unlit/WaterShader"
{
    Properties
    {
        _Color("Color", Color) = (0.2, 0.5, 1, 0.5)
        _Strength("Strength", Range(0,1)) = 0.25
        _Speed("Speed", Range(-100, 100)) = 25
        _Spread("Spread", Range(0, 1)) = 0.5
        _MainTex("Albedo", 2D) = "white"{}
        _BumpMap("Bump Map", 2D) = "bump"{}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Cull Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _Color;
            float _Strength;
            float _Speed;
            float _Spread;
            float4 worldLightPosition;
            sampler2D _MainTex;
            sampler2D _BumpMap;
            sampler2D DiffuseSampler;

            struct vertIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct vertOut
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            vertOut vert(vertIn IN)
            {
                vertOut o;
                o.uv = IN.uv;
                float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);
                o.lightDir = worldPos.xyz - _WorldSpaceLightPos0;
                float noise = cos((0.5*sin((worldPos.x - worldPos.z)* _Spread) - (worldPos.x + worldPos.z) * _Spread) + (_Speed * _Time));
                worldPos.y = worldPos.y + noise * _Strength;
                o.worldPos = worldPos;
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            float4 frag(vertOut IN) : SV_TARGET
            {   
                float3 diffuse = float3(0.9, 0.9, 0.9) * saturate(dot(IN.normal, -IN.lightDir));
                float3 ambient = float3(0.1, 0.1, 0.3);

                return float4((diffuse + ambient) * _Color, 0.5);
            }
            ENDCG
        }
    }
}
