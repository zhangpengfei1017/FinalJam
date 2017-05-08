// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Indicator" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Threshold ("Threshold", Range(0, 1)) = 0.5
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			//Blend DstColor One
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				return o;
			}
			
			fixed4 _Color;
			fixed _Threshold;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 uv = UNITY_PROJ_COORD(i.uvShadow);
				fixed2 pos = uv.xy / uv.w - fixed2(0.5, 0.5);

				fixed r = sqrt(pos.x * pos.x + pos.y * pos.y);
				fixed val = step(0.5, 1 - r) * smoothstep((1 - _Threshold) * 0.5, 0.5, r);

				if (step(0.1, val) == 0) discard;

				fixed4 col = _Color * val;
				//col.a = _Color.a;

				return col;
			}
			ENDCG
		}
	}
}
