Shader "SeganX/Legacy/Albedo/GrayScale" 
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		[Enum(ON,1,OFF,0)]	_ZWrite ("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		ZWrite [_ZWrite]
			
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
					fixed4 col : COLOR0;
				};

				float4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
			
				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.pos );
					o.uv0 = TRANSFORM_TEX( v.uv0, _MainTex );
					o.col = _Color;
					return o;
				}
			
				fixed4 frag (VertexOutput i) : SV_Target
				{
					fixed4 c = tex2D( _MainTex, i.uv0 );
					fixed g = (c.r + c.g + c.b) / 3;
					fixed4 r = g * (1.0f - i.col.a) + c * i.col.a;
					r.rgb *= i.col.rgb;
					r.a = 1;
					return r;
				}
				
			ENDCG
		}
	}

}
