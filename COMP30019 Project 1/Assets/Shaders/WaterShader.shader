Shader "Unlit/WaterShader"
{
    Properties
    {
        _Color("Color", Color) = (0.30, 0.850, 0.95, 0.7)
        _Strength("Strength", Range(0,5)) = 0.7
        _Strength2("Strength2", Range(0,5)) = 1.8
        _Speed("Speed", Range(-100, 100)) = 15
        _Spread("Spread", Range(0, 1)) = 0.1
        _fAtt ("fAtt", Range(0,5)) = 1
        _Ka ("Ambient relfection constant",Range(0,5)) = 1
        _Kd ("Diffuse reflection constant",Range(0,5)) = 1
        _Ks ("Ks", Range(0,10)) = 2.5
        _specN ("SpecularN", Range(1,500)) = 250
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
            float _Strength2;
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
                float3 worldNormal : TEXCOORD1;
            };

            vertOut vert(vertIn IN)
            {
                vertOut o;
                // Pass world vertexes 
				float4 worldVertex = mul(unity_ObjectToWorld, IN.vertex);
                o.worldVertex = worldVertex;

                /* Alternative water noise formula */
                // float noise = cos((0.5*sin((worldVertex.x - worldVertex.z)* _Spread) - (worldVertex.x + worldVertex.z) * _Spread) + (_Speed * _Time));
                // float s = (_Spread-_Spread*cos(_Spread*(worldVertex.x+worldVertex.z))/2);
                // float dfdx = s*sin(sin(_Spread*(worldVertex.x+worldVertex.z))/2-(_Spread*(worldVertex.x+worldVertex.z))+(_Speed*_Time));
                // float dfdz = s*sin(sin(_Spread*(worldVertex.x+worldVertex.z))/2-(_Spread*(worldVertex.x+worldVertex.z))+(_Speed*_Time));

                // Calculate water waves/noise and tangents
                float noise = _Strength*(sin((worldVertex.x+worldVertex.z)*_Spread+_Time*_Speed) + _Strength2*cos(0.25*((worldVertex.z-worldVertex.x)*(_Spread)+_Time*_Speed)));
                worldVertex.y = worldVertex.y + noise * _Strength;
                //Calculate the partial derivatives dx and dz of noise
                float dnoisedx = _Strength*(cos((worldVertex.x+worldVertex.z)*_Spread+_Time*_Speed)*_Spread) + (_Spread)*0.25*_Strength2*sin(0.25*((worldVertex.z-worldVertex.x)*(_Spread)+_Time*_Speed));
                float dnoisedz = _Strength*(cos((worldVertex.x+worldVertex.z)*_Spread+_Time*_Speed)*_Spread) - (_Spread)*0.25*_Strength2*sin(0.25*((worldVertex.z-worldVertex.x)*(_Spread)+_Time*_Speed));
                //Get the tangent vectors
                float3 tx = float3(1, dnoisedx, 0);
                float3 tz = float3(0, dnoisedz, 1);

                // Cross product tangents to form normals and pass through vertex & colour data
                o.worldNormal = normalize(cross(tz, tx));
                o.vertex = mul(UNITY_MATRIX_VP, worldVertex);
                o.color = _Color;
                return o;
            }

            float4 frag(vertOut v) : COLOR
            {   
                // Variables that adjust when darkness comes based on height of sun. 
				float DISTANCEBELOWZERO = 80; // how low under 0 to be dark mode
				float AMBLEVELWHENUNDER = 0.5; // amount of amb light
                
                // Incorrect no interpolation in fragment tangent calculator
                // float3 ddxPos = ddx(v.worldVertex);
                // float3 ddyPos = ddy(v.worldVertex) * _ProjectionParams.x;
                // float3 interpolatedNormal = normalize(cross(ddxPos, ddyPos));

                // Interpolate normals
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
                    float factor = clamp((_PointLightPosition.y+DISTANCEBELOWZERO)/DISTANCEBELOWZERO, 0, 1);
					color.rgb = clamp((factor),AMBLEVELWHENUNDER,1) * (amb.rgb) + factor*(dif.rgb + spe.rgb);
				}

                color.a = v.color.a;
                return color;
            }
            ENDCG
        }
    }
}
