Shader "Unlit/WaterShader"
{
    Properties
    {
        _Color("Color", Color) = (0.4, 0.5, 0.8, 0.7)
        _Strength("Strength", Range(0,1)) = 0.25
        _Speed("Speed", Range(-100, 100)) = 25
        _Spread("Spread", Range(0, 1)) = 0.5
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

            struct vertIn
            {
                float4 vertex : POSITION;
            };

            struct vertOut
            {
                float4 vertex : SV_POSITION;
                float height : TEXCOORD0;
            };

            vertOut vert(vertIn IN)
            {
                vertOut o;
                float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);

                float noise = cos((0.5*sin((worldPos.x - worldPos.z)* _Spread) - (worldPos.x + worldPos.z) * _Spread) + (_Speed * _Time));
                worldPos.y = worldPos.y + noise * _Strength;
                o.height = worldPos.y;
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            float4 frag(vertOut IN) : COLOR
            {   
                return _Color;
            }
            ENDCG
        }
    }
}
