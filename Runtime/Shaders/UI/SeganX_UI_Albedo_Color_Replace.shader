// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SeganX/UI/Albedo/Replace Color" {
	Properties {
		_MainTex ("Sprite", 2D) = "white" {}
		_Queue ("Queue", Int) = 3002
		
		[Enum(ON,1,OFF,0)]	_ZWrite ("Z Write", Int) = 0
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
		[Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc ("SrcFactor", Int) = 5
		[Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest ("DstFactor", Int) = 10
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader {

		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Cull [_Cull]
		ZWrite [_ZWrite]
		Blend [_BlendSrc] [_BlendDest] 
		
		Lighting Off 
		ZTest [unity_GUIZTestMode]
		ColorMask [_ColorMask]

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
			
			const float EPSILON = 1e-10;

			fixed3 hue_to_rgb(in float hue)
			{
				// Hue [0..1] to RGB [0..1]
				// See http://www.chilliant.com/rgb2hsv.html
				fixed3 rgb = abs(hue * 6. - fixed3(3, 2, 4)) * fixed3(1, -1, -1) + fixed3(-1, 2, 2);
				return clamp(rgb, 0., 1.);
			}

			fixed3 rgb_to_hcv(in fixed3 rgb)
			{
				// RGB [0..1] to Hue-Chroma-Value [0..1]
				// Based on work by Sam Hocevar and Emil Persson
				fixed4 p = (rgb.g < rgb.b) ? fixed4(rgb.bg, -1., 2. / 3.) : fixed4(rgb.gb, 0., -1. / 3.);
				fixed4 q = (rgb.r < p.x) ? fixed4(p.xyw, rgb.r) : fixed4(rgb.r, p.yzx);
				float c = q.x - min(q.w, q.y);
				float h = abs((q.w - q.y) / (6. * c + EPSILON) + q.z);
				return fixed3(h, c, q.x);
			}

			fixed3 hsv_to_rgb(in fixed3 hsv)
			{
				// Hue-Saturation-Value [0..1] to RGB [0..1]
				fixed3 rgb = hue_to_rgb(hsv.x);
				return ((rgb - 1.) * hsv.y + 1.) * hsv.z;
			}

			fixed3 rgb_to_hsv(in fixed3 rgb)
			{
				// RGB [0..1] to Hue-Saturation-Value [0..1]
				fixed3 hcv = rgb_to_hcv(rgb);
				float s = hcv.y / (hcv.z + EPSILON);
				return fixed3(hcv.x, s, hcv.z);
			}

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
				fixed4 c = tex2D(_MainTex, i.uv0);
				
				fixed3 tex = rgb_to_hsv(c.rgb);
				fixed3 col = rgb_to_hsv(i.col.rgb);
				fixed3 hsv;
				hsv.x = col.x;
				hsv.y = tex.y * col.y;
				hsv.z = tex.z * col.z;
				c.rgb = hsv_to_rgb(hsv);				
				c.a *= i.col.a;
				clip (c.a - 0.01);
				return c;
			}
			ENDCG 
		}
	}
}
