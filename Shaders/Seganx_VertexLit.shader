// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/VertexLit" 
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_AmbientColor("Ambient", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorStrength("Color Strength", Float) = 1

		
		[Enum(ON,1,OFF,0)]	_ZWrite ("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 80
		
		ZWrite [_ZWrite]
		
		Pass 
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				fixed4 _Color;
				fixed4 _AmbientColor;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				struct VertexInput
				{
					float4 pos : POSITION;
                    fixed3 nrm : NORMAL;
                    fixed3 col : COLOR;
					float2 uv0 : TEXCOORD0;
				};
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
                    fixed3 col : COLOR;
                    float2 uv0 : TEXCOORD0;
					fixed3 shd : TEXCOORD2;
				};
				

				inline float3 LightingLambertVS (float3 normal, float3 lightDir)
				{
					fixed diff = max (0, dot (normal, lightDir));			
					return _LightColor0.rgb * diff;
				}
				
				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos (v.pos);
					o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.col = v.col;
					float3 worldN = UnityObjectToWorldNormal(v.nrm);
					
					o.shd = ShadeSH9 (float4(worldN,1.0));
					o.shd += LightingLambertVS (worldN, _WorldSpaceLightPos0.xyz);
					return o;
				}
				
				float _ColorStrength;
				fixed4 frag (VertexOutput v) : SV_Target
				{
					fixed4 c = tex2D( _MainTex, v.uv0 ) * _Color * _ColorStrength;
					c.rgb *= v.shd * v.col;
					return c + _AmbientColor;
				}

			ENDCG
		}
		// Pass
	}
	// SubShader

FallBack "Mobile/VertexLit"
}
