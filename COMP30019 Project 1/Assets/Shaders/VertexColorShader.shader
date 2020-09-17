//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/VertexColorShader"
{        
	Properties 
    { 
        _avgheight ("avgheight", Range(-400,400.0)) = 0
		_maxheight ("maxheight", Range(-400,400.0)) = 0
		_fAtt ("fAtt", Range(0,5)) = 1
        _Ka ("Ambient relfection constant",Range(0,5)) = 1.5
        _Kd ("Diffuse reflection constant",Range(0,5)) = 1
        _Ks ("Ks", Range(0,5)) = 0.1
        _specN ("SpecularN", Range(1,20)) = 1
        _PointLightColor ("Point Light Color", Color) = (1, 1, 1)
        _PointLightPosition ("Point Light Position", Vector) = (0.0, 0.0, 0.0)
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

				// Pass through colours and vertex in screenspace
				o.color = v.color;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				float actualMaxHeight = _maxheight - _avgheight;

				// Adjust weights of sealevels and snow
				float SNOWWEIGHT = 0.5;
				float SNOWTOBROWNBLUR = 4; // higher = more blurry snow edge
				float HEIGHTABOVESEALEVEL = 5.5; // this value increases sealevel by addition
				float NOISEWEIGHT = 0.25; // higher = more colour noise
				float NOISESIZE = 10; // higher = noise waves bigger

				// Variables that adjust when darkness comes based on height of sun. 
				float DISTANCEBELOWZERO = 80; // how low under 0 to be dark mode
				float AMBLEVELWHENUNDER = 0.5; // amount of amb light

				// Colors
				float3 green = float3(0.3f, 0.6f, 0.1f);
				float3 brown = float3(0.7f, 0.5f, 0.3f);
				float3 white = float3(1.0f, 1.0f, 1.0f);
				float3 sand = float3(0.8f, 0.8f, 0.4f);
				float3 brown2whitediff = white - brown;
				float3 white2browndiff = brown - white;
				float3 brown2greendiff = green - brown;
				float3 green2sanddiff = sand - green;

				float snowheight = actualMaxHeight-SNOWWEIGHT*(actualMaxHeight);
				float sealevel = HEIGHTABOVESEALEVEL;
				float factor;

                if (v.worldVertex.y > sin(v.worldVertex.x*v.worldVertex.z/500)+snowheight) {
					// White down to snow height
					factor = clamp(2/(v.worldVertex.y-(sin(v.worldVertex.x*v.worldVertex.z/500)+snowheight)), 0, 1);
					v.color.rgb = white + factor*white2browndiff;
				} else if (v.worldVertex.y  > (sealevel)) {
					// Gradient brown to green until reached avg + 5
					factor = clamp(((snowheight - v.worldVertex.y))/(snowheight-sealevel), 0, 1);
					v.color.rgb = brown + factor*(brown2greendiff);
				} else {
					// Green to sand
					factor = clamp(((sealevel - v.worldVertex.y))/SNOWTOBROWNBLUR, 0, 1);
					v.color.rgb = green + factor*green2sanddiff;
				} 

				// Add subtle noise to entire area above sea level.
				if (v.worldVertex.y  > (sealevel)) {	
					factor = clamp(((snowheight - v.worldVertex.y))/(snowheight-sealevel), 0, 1);
					float noise = clamp(cos(sin((v.worldVertex.x - v.worldVertex.z)/NOISESIZE) - (v.worldVertex.x + v.worldVertex.z)/NOISESIZE), 0, 1);
					v.color.rgb += NOISEWEIGHT*(1-factor)*(noise)*brown2whitediff;
				}

				float3 interpolatedNormal = normalize(v.worldNormal);

                // Calculate ambient RGB intensities
                float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * _Ka;

                // Calculate diffuse RBG reflections, we save the results of L.N because we will use it again for specular
                float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
                float LdotN = dot(L, interpolatedNormal);
                float3 dif = _fAtt * _PointLightColor.rgb * _Kd * v.color.rgb * saturate(LdotN);

                float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
                float3 H = normalize(V + L);

				// Calculate specular
                float3 spe = _fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(interpolatedNormal, H)), _specN);

                // Combine Phong illumination model components
                float4 color;
				if (_PointLightPosition.y > 0) {
					// If sun above y = 0
                	color.rgb = amb.rgb + dif.rgb + spe.rgb;
				} else {
					// If sun below y = 0
					factor = clamp(((_PointLightPosition.y+DISTANCEBELOWZERO)/DISTANCEBELOWZERO), 0, 1);
					color.rgb = clamp((factor),AMBLEVELWHENUNDER,1) * (amb.rgb) + factor*(dif.rgb + spe.rgb);
				}

                color.a = 1.0f;
                return color;
			}
			ENDCG
		}
	}
}
