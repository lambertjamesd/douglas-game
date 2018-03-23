// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PlayerDeath"
{
	Properties
	{
        _Color ("Main Color", COLOR) = (1,1,1,1)
		_CircleOriginRadius ("Circle Origin And Radius", Vector) = (0.5, 0.5, 0.25, 1.77777)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 _Color;
			float4 _CircleOriginRadius;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 offset = i.uv - _CircleOriginRadius.xy;
				offset.x = offset.x * _CircleOriginRadius.w;
				float distance = dot(offset, offset);
				return float4(_Color.rgb, (distance > _CircleOriginRadius.z * _CircleOriginRadius.z) * _Color.a);
			}
			ENDCG
		}
	}
}
