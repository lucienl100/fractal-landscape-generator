Shader "Unlit/WaterShader"
{
    Properties
    {
        _Color("Color", Color) = (0.4, 0.5, 0.8, 1)
        _Strength("Strength", Range(0,5)) = 1
        _Speed("Speed", Range(-100, 100)) = 25
        _Spread("Spread", Range(0, 1)) = 0.5
        _fAtt ("fAtt", Range(0,5)) = 1
        _Ka ("Ambient relfection constant",Range(0,5)) = 1
        _Kd ("Diffuse reflection constant",Range(0,5)) = 1
        _Ks ("Ks", Range(0,5)) = 0.5
        _specN ("SpecularN", Range(1,20)) = 5
        _PointLightColor ("Point Light Color", Color) = (1, 1, 1)
        _PointLightPosition ("Point Light Position", Vector) = (0.0, 100.0, 0.0)
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
            float _fAtt;
			float _Ka;
			float _Kd;
			float _Ks;
			float _specN;
			float3 _PointLightColor;
			float3 _PointLightPosition;

            struct vertIn
            {
                float4 vertex : POSITION;
            };

            struct vertOut
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 worldVertex : TEXCOORD0;
				// float3 worldNormal : TEXCOORD1;
            };

            vertOut vert(vertIn IN)
            {
                vertOut o;
				float4 worldVertex = mul(unity_ObjectToWorld, IN.vertex);
                // float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));
                float noise = cos((0.5*sin((worldVertex.x - worldVertex.z)* _Spread) - (worldVertex.x + worldVertex.z) * _Spread) + (_Speed * _Time));
                //float noise = sin(worldVertex.x/10 + _Time*100);
                worldVertex.y = worldVertex.y + noise * _Strength;
                o.worldVertex = worldVertex;
                o.vertex = mul(UNITY_MATRIX_VP, worldVertex);
                o.color = _Color;
                return o;
            }

            float4 frag(vertOut v) : COLOR
            {   
                float3 ddxPos = ddx(v.worldVertex);
                float3 ddyPos = ddy(v.worldVertex) * _ProjectionParams.x;
                float3 interpolatedNormal = normalize(cross(ddxPos, ddyPos));
                //float3 interpolatedNormal = normalize(v.worldNormal);
                
				
                // Calculate ambient RGB intensities
                float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * _Ka;

                // Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
                // (when calculating the reflected ray in our specular component)
                float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
                float LdotN = dot(L, interpolatedNormal);
                float3 dif = _fAtt * _PointLightColor.rgb * _Kd * v.color.rgb * saturate(LdotN);

                float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
                float3 H = normalize(V + L);

                float3 spe = _fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(interpolatedNormal, H)), _specN);

                // Combine Phong illumination model components
                float4 color;
                color.rgb = amb.rgb + dif.rgb + spe.rgb;
                color.a = v.color.a;

                return color;
            }
            ENDCG
        }
    }
}
