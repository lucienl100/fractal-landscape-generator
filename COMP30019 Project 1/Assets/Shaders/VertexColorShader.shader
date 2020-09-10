//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/TerrainShader"
{        
	Properties 
    { 
        _avgheight ("avgheight", Range(-400,400.0)) = 0
		_maxheight ("maxheight", Range(-400,400.0)) = 0
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
		Pass
		{
			// Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			float _avgheight;
			float _maxheight;
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
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
                float4 worldVertex : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				// Pass through world vertex and normals
				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));
                o.worldNormal = worldNormal;
                o.worldVertex = worldVertex;

				o.color = v.color;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				float snowheight;
				float heightweight = 0.7;
				snowheight = _avgheight+heightweight*(_maxheight - _avgheight);

				float x = 1.0;
				float y = 1.0;
				float3 green = float3(0.3f, 0.8f, 0.1f);
				float3 brown = float3(0.7f, 0.5f, 0.3f);
				float3 white = float3(1.0f, 1.0f, 1.0f);
				float3 sand = float3(0.8f, 0.8f, 0.4f);

				float3 brown2greendiff = green - brown;
				float3 green2sanddiff = sand - green;
				
                if (v.worldVertex.y > sin(v.worldVertex.x*v.worldVertex.z/500)+snowheight) {
					// White down to snow height
					v.color.rgb = white;
				} else if (v.worldVertex.y  > _avgheight+5) {
					// Gradient brown to green until reached avg + 5
					y = clamp(((snowheight - v.worldVertex.y)*(snowheight - v.worldVertex.y))/300, 0, 1);
					v.color.rgb = brown + y*brown2greendiff;
				} else {
					// Green to sand
					x = clamp(((_avgheight+5 - v.worldVertex.y))/5, 0, 1);
					v.color.rgb = green + x*green2sanddiff;
				} 

				v.color.a = 1.0f;
				
				float3 interpolatedNormal = normalize(v.worldNormal);
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
