Shader "SeganX/VertexLit/Transparent"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}

		[Toggle] Feature_Vertex_Color("Vertex Color", Float) = 1
		[HDR] _Ambient("Ambient", Color) = (1,1,1,1)

		[Space(20)]
		[Toggle] Feature_Specular("Specular", Float) = 0
		[HDR] _SpecularColor("Color", Color) = (0.9,0.9,0.9,1)
		[PowerSlider(3.0)] _Glossiness("Glossiness", Range(1, 1000)) = 32

		[Space(20)]
		[Toggle] Feature_Emission("Emission", Float) = 0
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0,0)

		[Header(Render Options)]
		[Enum(UnityEngine.Rendering.CullMode)]		_ShadowCull("Shadow Caster Cull", Int) = 2
		[Enum(UnityEngine.Rendering.CullMode)]		_Cull("Cull", Int) = 2
		[Enum(OFF,0,Less,4,Greater,7)]				_ZTest("Depth Test", Int) = 4
		[Enum(OFF,0,ON,1)]							_ZWrite("Depth Write", Int) = 1
		[Enum(OFF,0,ON,15)]							_ColorMask("Color Write", Int) = 15
		[Space(20)]
		[Enum(UnityEngine.Rendering.BlendMode)]		_BlendSrc("Source Factor", Int) = 5
		[Enum(UnityEngine.Rendering.BlendOp)]		_BlendOp("Blend Operation", Int) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]		_BlendDest("Destination Factor", Int) = 10
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		LOD 200

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
			BlendOp[_BlendOp]
			Blend[_BlendSrc][_BlendDest]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing
			#pragma multi_compile __ FEATURE_VERTEX_COLOR_ON
			#pragma multi_compile __ FEATURE_SPECULAR_ON
			#pragma multi_compile __ FEATURE_EMISSION_ON

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct vertex_input
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vertex_output
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 shade : TEXCOORD1;
#if FEATURE_SPECULAR_ON
				float4 specular : TEXCOORD2;
#endif
				SHADOW_COORDS(3)
				UNITY_FOG_COORDS(4)
			};

			fixed4 _Color;
			fixed4 _Ambient;
#if FEATURE_EMISSION_ON
			float4 _EmissionColor;
#endif
#if FEATURE_SPECULAR_ON
			float4 _SpecularColor;
			float _Glossiness;
#endif
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
				o.color = v.color * _Color;
#else
				o.color = _Color;
#endif

				float3 world_normal = UnityObjectToWorldNormal(v.normal);
				float dot_light = dot(_WorldSpaceLightPos0, world_normal);
				float3 ambient = ShadeSH9(float4(world_normal, 1.0f)) * _Ambient;
				o.shade = float4(ambient, dot_light);

#if FEATURE_SPECULAR_ON
				float3 view_dir = normalize(WorldSpaceViewDir(v.vertex));
				float3 half_view = normalize(_WorldSpaceLightPos0 + view_dir);
				float NdotH = dot(world_normal, half_view);
				float spec = pow( smoothstep(0, 1, NdotH), _Glossiness );

				o.specular = spec * _SpecularColor;
#endif

				UNITY_TRANSFER_FOG(o, o.position);
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(vertex_output v) : SV_Target
			{
				float shadow = SHADOW_ATTENUATION(v);
				float light = smoothstep(0, 1, v.shade.a * shadow);
				float3 attenu = _LightColor0.rgb * light;

				fixed4 c = tex2D(_MainTex, v.texcoord) * v.color;
				c.rgb *= v.shade.rgb + attenu;

#if FEATURE_SPECULAR_ON
				float3 specular = attenu * v.specular;
				c.rgb += specular;
#endif

#if FEATURE_EMISSION_ON
				c += _EmissionColor;
#endif

				UNITY_APPLY_FOG(v.fogCoord, c);
				return c;
			}

			ENDCG
		}
		// Pass

		Pass
		{
			Name "ShadowCaster"
			Tags {"LightMode" = "ShadowCaster"}

			ZWrite On
			Cull[_ShadowCull]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}
	}
		// SubShader

	FallBack "Mobile/VertexLit"
}
