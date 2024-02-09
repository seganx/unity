Shader "SeganX/Unlit/Opaque" 
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		[Toggle] Feature_Vertex_Color("Vertex Color", Float) = 1
		_ColorStrength("_ColorStrength", Range(0, 10)) = 1

		[Header(Render Options)]
		[Enum(UnityEngine.Rendering.CullMode)]	_ShadowCull("Shadow Caster Cull", Int) = 2
		[Enum(UnityEngine.Rendering.CullMode)]	_Cull("Cull", Int) = 2
		[Enum(OFF,0,Less,4,Greater,7)]			_ZTest("Depth Test", Int) = 4
		[Enum(OFF,0,ON,1)]						_ZWrite("Depth Write", Int) = 1
		[Enum(OFF,0,ON,15)]						_ColorMask("Color Write", Int) = 15
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

			Cull[_Cull]
			ZTest[_ZTest]
			ZWrite[_ZWrite]
			ColorMask[_ColorMask]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing
			#pragma multi_compile __ FEATURE_VERTEX_COLOR_ON


			#include "UnityCG.cginc"

			struct vertex_input
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vertex_output
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;

				UNITY_FOG_COORDS(1)
			};

			float4 _Color;
			float _ColorStrength;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			vertex_output vert(vertex_input v)
			{
				vertex_output o;
				UNITY_SETUP_INSTANCE_ID(v);

				o.position = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

#if FEATURE_VERTEX_COLOR_ON
				o.color = v.color * _Color * _ColorStrength;
#else
				o.color = _Color * _ColorStrength;
#endif

				UNITY_TRANSFER_FOG(o, o.position);
				return o;
			}

			fixed4 frag(vertex_output v) : SV_Target
			{
				fixed4 c = tex2D( _MainTex, v.texcoord ) * v.color;

				UNITY_APPLY_FOG(v.fogCoord, c);
				return c;
			}
			
			ENDCG
		}
	}

}
