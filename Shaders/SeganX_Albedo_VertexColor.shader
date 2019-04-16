// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/Albedo/VertexColor" 
{
	Properties 
	{
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
			
				#include "UnityCG.cginc"

				struct VertexInput
				{
					float4 pos : POSITION;
                    fixed4 col : COLOR;
				};
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
                    float4 col : COLOR;
				};

			
				float4 _MainTex_ST;
				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.pos );
                    o.col = v.col;
					return o;
				}
			

                fixed4 frag(VertexOutput i) : SV_Target
                {
                    fixed4 c = 1;
                    c.r = i.col.r;
                    c.g = i.col.a;
					return c;
				}
				
			ENDCG
		}
	}

}
