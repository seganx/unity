// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/Albedo/Alpha/TileSpeed" 
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _TileSpeedX("Tile Speed X", Float) = 0
        _TileSpeedY("Tile Speed Y", Float) = 0
		_ColorStrength ("Color Strength", Float) = 1
		
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
					float4 col : COLOR0;
				};
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					fixed4 col : COLOR0;
				};

				float4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
                float _TileSpeedX;
                float _TileSpeedY;
			
				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.pos );
					o.uv0 = TRANSFORM_TEX( v.uv0, _MainTex );
                    o.uv0.x += _TileSpeedX * _Time.y;
                    o.uv0.y += _TileSpeedY * _Time.y;
					o.col = v.col * _Color;
					return o;
				}
			
				float _ColorStrength;				
				fixed4 frag (VertexOutput i) : SV_Target
				{
					fixed4 c = tex2D( _MainTex, i.uv0 ) * i.col;
					clip(c.a - 0.01f);
					return c * _ColorStrength;
				}
				
			ENDCG
		}
	}

}
