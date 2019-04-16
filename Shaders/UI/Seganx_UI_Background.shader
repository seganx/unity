// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/UI/Background" {
	Properties {
		_MainTex ("Sprite", 2D) = "white" {}
		_Queue ("Queue", Int) = 3000
	}

	SubShader {

		Tags 
		{
			"Queue"="Geometry"
			"IgnoreProjector"="True"
			"RenderType"="Background"
			"PreviewType"="Plane"
		}
				
		Cull OFF
		ZWrite OFF
		
		Lighting OFF 
		ZTest OFF

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct vs_in {
				float4 pos : POSITION;
				fixed4 col : COLOR;
				float2 uv0 : TEXCOORD0;
			};

			struct vs_out {
				float4 pos : SV_POSITION;
				fixed4 col : COLOR;
				float2 uv0 : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			vs_out vert (vs_in v)
			{
				vs_out o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.col = v.col;
				o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);

#ifdef UNITY_HALF_TEXEL_OFFSET
				o.pos.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif

				return o;
			}

			fixed4 frag (vs_out i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv0) * i.col;
				clip (c.a - 0.01);
				return c;
			}
			ENDCG 
		}
	}
}
