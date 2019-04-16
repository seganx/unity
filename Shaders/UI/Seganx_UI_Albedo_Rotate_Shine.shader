// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/UI/Albedo/Rotate_Shine" {
	Properties {
		_MainTex ("Sprite", 2D) = "white" {}
		_ColorStrength ("Color Strength", Float) = 1
		_Queue ("Queue", Int) = 3002
        _Color1("Shine Color Front:", Color) = (0.5,0.5,0.5,0.5)
		_Speed1 ("Shine Speed Front", Float) = 2.0
		_Radius1 ("Shine Radius Front", Float) = 0.5
        _Color2("Shine Color Back:", Color) = (0.5,0.5,0.5,0.5)
		_Speed2 ("Shine Speed Back", Float) = 2.0
		_Radius2 ("Shine Radius Back", Float) = 0.5
		

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
				float2 uv1 : TEXCOORD1;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Speed1;
			uniform float _Radius1;
			uniform float _Speed2;
			uniform float _Radius2;
			
			
			vs_out vert (vs_in v)
			{
				vs_out o;

				o.pos = UnityObjectToClipPos(v.pos);
				o.col = v.col;

				half2 uv = TRANSFORM_TEX(v.uv0, _MainTex) - 0.5f;

				half sint, cost;
				sincos(_Time.x * _Speed1, sint, cost);
				o.uv0.x = 0.5f + _Radius1 * (+ uv.x * cost - uv.y * sint);
				o.uv0.y = 0.5f + _Radius1 * (- uv.x * sint - uv.y * cost);

				sincos(_Time.x * _Speed2, sint, cost);
				o.uv1.x = 0.5f - _Radius2 * (+ uv.x * cost - uv.y * sint);
				o.uv1.y = 0.5f + _Radius2 * (- uv.x * sint - uv.y * cost);

#ifdef UNITY_HALF_TEXEL_OFFSET
				o.pos.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif

				return o;
			}


			float _ColorStrength;
            float4 _Color1;
            float4 _Color2;

			fixed4 frag (vs_out i) : SV_Target
			{
				fixed4 c1 = tex2D(_MainTex, i.uv0) * _Color1;
				fixed4 c2 = tex2D(_MainTex, i.uv1) * _Color2;
				
				fixed4 c = (c1 + c2) * i.col;

				clip (c.a - 0.01);
				return c * _ColorStrength;
			}
			ENDCG 
		}
	}
}
