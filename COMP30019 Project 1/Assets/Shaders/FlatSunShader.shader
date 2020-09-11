//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/FlatSunShader"
{
    Properties 
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1)
    }
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

            float4 _Color;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 color: COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color: COLOR;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = _Color;
				return o;
			}
			
			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				return v.color;
			}
			ENDCG
		}
	}
}
