Shader "SeganX/Unlit/Outline"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[KeywordEnum(World, Screen)] _SimulationSpace("Space", Float) = 0
		_Thickness("Thickness", Range(0.001, 0.2)) = 0.05

		[Header(Render Options)]
		_Bias("Bias", Range(-100, 100)) = 1
		[Enum(ON,1,OFF,0)]				_ZWrite("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 1

	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 80

		Pass
		{
			Name "FORWARD"
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			Cull [_Cull]
			ZWrite [_ZWrite]
			Offset [_Bias],1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing
			#pragma multi_compile _SIMULATIONSPACE_WORLD _SIMULATIONSPACE_SCREEN


			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct vertex_input
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vertex_output
			{
				float4 position : SV_POSITION;
				UNITY_FOG_COORDS(0)
			};

			float _Thickness;
			float _Bias;
			float4 _Color;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			vertex_output vert(vertex_input v)
			{
				vertex_output o;
				UNITY_SETUP_INSTANCE_ID(v);

#if _SIMULATIONSPACE_SCREEN
				float distance = length(ObjSpaceViewDir(v.vertex));
				v.vertex.xyz += normalize(v.normal) * _Thickness * distance;
#else
				v.vertex.xyz += normalize(v.normal) * _Thickness;
#endif

				o.position = UnityObjectToClipPos(v.vertex);

				UNITY_TRANSFER_FOG(o, o.position);
				return o;
			}

			fixed4 frag(vertex_output v) : SV_Target
			{
				fixed4 c = _Color;
				UNITY_APPLY_FOG(v.fogCoord, c);
				return c;
			}

			ENDCG
		}
		// Pass

	}
	// SubShader

	FallBack "Mobile/VertexLit"
}
