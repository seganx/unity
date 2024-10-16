Shader "SeganX/Legacy/Albedo/Shadow" 
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorStrength("Color Strength", Float) = 1

		[Enum(ON,1,OFF,0)]				_ZWrite ("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Cull [_Cull]
		ZWrite [_ZWrite]
			
		Pass 
		{ 
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }
		
			CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
		
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					
					LIGHTING_COORDS(1, 2) // 1, 2 are texcoord indices
				};

			
				float4 _MainTex_ST;
				VertexOutput vert (appdata_base v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.vertex );
					o.uv0 = TRANSFORM_TEX( v.texcoord, _MainTex );

					// append shaodow coords
					TRANSFER_VERTEX_TO_FRAGMENT(o);

					return o;
				}
			

				sampler2D _MainTex;
				float4 _Color;
				float _ColorStrength;
				fixed4 frag (VertexOutput i) : SV_Target
				{
					fixed4 c = tex2D( _MainTex, i.uv0 );
					
					c.rgb *= LIGHT_ATTENUATION(i);

					return c * _Color * _ColorStrength;
				}
				
			ENDCG
		}
	}

	Fallback "SeganX/Albedo/Simple"
}
