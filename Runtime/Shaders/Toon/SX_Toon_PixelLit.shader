Shader "SeganX/Toon/PixelLit"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}

		[Toggle] Feature_Vertex_Color("Vertex Color", Float) = 1
		[HDR] _Ambient("Ambient", Color) = (1,1,1,1)

		[Space(20)]
		[Toggle] Feature_Rim("Rim", Float) = 1
		[HDR] _RimColor("Color", Color) = (1,1,1,1)
		_RimAmount("Amount", Range(0, 1)) = 0.716
		_RimThreshold("Threshold", Range(0, 1)) = 0.1

		[Space(20)]
		[Toggle] Feature_Specular("Specular", Float) = 0
		[HDR] _SpecularColor("Color", Color) = (0.9,0.9,0.9,1)
		[PowerSlider(3.0)] _Glossiness("Glossiness", Range(1, 1000)) = 32

		[Space(20)]
		[Toggle] Feature_Emission("Emission", Float) = 0
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0,0)

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
			#pragma multi_compile __ FEATURE_RIM_ON
			#pragma multi_compile __ FEATURE_SPECULAR_ON
			#pragma multi_compile __ FEATURE_EMISSION_ON

			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct vertex_input
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vertex_output
			{
				float4 position : SV_POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;

#if FEATURE_SPECULAR_ON || FEATURE_RIM_ON
				float3 view_dir : TEXCOORD1;	
#endif
				SHADOW_COORDS(2)
				UNITY_FOG_COORDS(3)
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
#if FEATURE_RIM_ON
			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;
#endif
			sampler2D _MainTex;
			float4 _MainTex_ST;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			vertex_output vert (vertex_input v)
			{
				vertex_output o;
				UNITY_SETUP_INSTANCE_ID(v);

				o.position = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);		
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

#if FEATURE_VERTEX_COLOR_ON
				o.color = v.color * _Color;
#else
				o.color = _Color;
#endif

#if FEATURE_SPECULAR_ON || FEATURE_RIM_ON
				o.view_dir = WorldSpaceViewDir(v.vertex);
#endif

				UNITY_TRANSFER_FOG(o, o.position);
				TRANSFER_SHADOW(o);
				return o;
			}
			

			float4 frag (vertex_output i) : SV_Target
			{
				float3 normal = normalize(i.normal);

#if FEATURE_SPECULAR_ON || FEATURE_RIM_ON
				float3 view_dir = normalize(i.view_dir);
#endif

				float n_dot_l = dot(_WorldSpaceLightPos0, normal);
				float shadow = SHADOW_ATTENUATION(i);

				float light_intensity = smoothstep(0, 0.025f, n_dot_l * shadow);
				float4 light = light_intensity * _LightColor0;
				float4 ambient = float4(ShadeSH9(float4(normal, 1.0f)) * _Ambient, 1);
				light = light + ambient;

#if FEATURE_SPECULAR_ON
				float3 half_view = normalize(_WorldSpaceLightPos0 + view_dir);
				float NdotH = dot(normal, half_view);
				float specular_intensity = pow(NdotH * light_intensity, _Glossiness);
				float specular_intensity_smooth = smoothstep(0.005f, 0.01f, specular_intensity);
				float4 specular = specular_intensity_smooth * _SpecularColor;
#endif

#if FEATURE_RIM_ON
				float rim_dot = 1 - dot(view_dir, normal);
				float rim_intensity = rim_dot * pow(n_dot_l, _RimThreshold);
				rim_intensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rim_intensity);
#endif

				float4 c = tex2D(_MainTex, i.texcoord) * i.color * light;

#if FEATURE_SPECULAR_ON
				c += specular;
#endif

#if FEATURE_RIM_ON
				c = lerp(c, _RimColor, rim_intensity);
#endif

#if FEATURE_EMISSION_ON
				c += _EmissionColor;
#endif

				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
		//	pass


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
}