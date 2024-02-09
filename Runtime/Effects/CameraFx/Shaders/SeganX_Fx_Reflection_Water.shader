Shader "SeganX/FX/Reflection/Water"
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
        _WaterTex("Water bump (RGB)", 2D) = "white" {}
        _WaterMaskTex("Water mask (RGB)", 2D) = "white" {}
        _WaterPower("Water Power", Float) = 0
        _WaterSpeedX("Water Speed X", Float) = 0
        _WaterSpeedY("Water Speed Y", Float) = 0
		_ColorStrength ("Color Strength", Float) = 1
        //_RflctTex("Reflection (RGB)", 2D) = "white" {}
		
		[Enum(ON,1,OFF,0)]	_ZWrite ("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
		[Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc ("SrcFactor", Int) = 5
		[Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest ("DstFactor", Int) = 10
	}

	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		
		LOD 100
		
		Cull [_Cull]
		ZWrite [_ZWrite]
		Blend [_BlendSrc] [_BlendDest] 
			
		Pass 
		{ 
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }
		
			CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"

				struct VertexInput
				{
					float4 pos : POSITION;
					float2 uv0 : TEXCOORD0;
				};
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
                    float2 uv0 : TEXCOORD0;
                    float2 uv1 : TEXCOORD1;
                    float4 uv2 : TEXCOORD2;
				};

                sampler2D _WaterTex;
                float4 _WaterTex_ST;
                float _WaterSpeedX;
                float _WaterSpeedY;

				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.pos );
                    o.uv0 = v.uv0;
                    o.uv1 = TRANSFORM_TEX( v.uv0, _WaterTex );
                    o.uv1.x += _WaterSpeedX * _Time.y;
                    o.uv1.y += _WaterSpeedY * _Time.y;
                    o.uv2 = o.pos;
					return o;
				}
			
                uniform sampler2D _RflctTex;
                sampler2D _WaterMaskTex;
                float _WaterPower;
                float _ColorStrength;
				float4 _Color;

				fixed4 frag (VertexOutput i) : SV_Target
				{
                    fixed a = tex2D( _WaterMaskTex, i.uv0 ).a;
					clip(a - 0.01f);
                    fixed2 w = tex2D( _WaterTex, i.uv1).rg;

                    i.uv2.x = 0.5f - (0.5f * i.uv2.x / i.uv2.w);

#if SHADER_API_GLES3 || SHADER_API_GLES || SHADER_API_GLCORE
                    i.uv2.y = (0.5f * i.uv2.y / i.uv2.w) + 0.5f; 
#else
                    i.uv2.y = 0.5f - (0.5f * i.uv2.y / i.uv2.w);
#endif
					fixed4 c;
                    c.rgb = tex2D(_RflctTex, i.uv2 + w * _WaterPower).rgb * _Color.rgb * _ColorStrength;
                    c.a = a * _Color.a;
					return c;
				}
				
			ENDCG
		}
	}

}
