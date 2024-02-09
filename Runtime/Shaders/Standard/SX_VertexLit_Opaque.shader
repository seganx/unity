Shader "SeganX/VertexLit/Opaque"
{
	Properties
	{
		[Header(_______________ Basic Colors __________________________________________________)]
		_Color("Color", Color) = (1,1,1,1)
		[HDR] _Ambient("Ambient", Color) = (1,1,1,1)
		[Toggle] Feature_Vertex_Color("Vertex Color", Float) = 1

		[Header(_______________ Main Texture and UV Options ___________________________________)]
		_MainTex("Base (RGB)", 2D) = "white" {}
		[KeywordEnum(None, Up, Cube)] _OverrideUV("Override UV", Float) = 0
		[Toggle] Feature_UVMovement("UV Movement", Float) = 0
		_MainTileU("Speed U", Float) = 0
		_MainTileV("Speed V", Float) = 0

		[Header(_______________ Additional Color and Texture and UV Options ___________________)]
		[KeywordEnum(None, Add, Multiply)] _AdditionalTile("Additional Tile", Float) = 0
		_AddColor("Color", Color) = (1,1,1,1)
		_AddTex("Texture", 2D) = "white" {}
		_AddTileU("Speed U", Float) = 0
		_AddTileV("Speed V", Float) = 0

		[Header(_______________ Specular Options ______________________________________________)]
		[Toggle] Feature_Specular("Specular", Float) = 0
		[HDR] _SpecularColor("Color", Color) = (0.9,0.9,0.9,1)
		[PowerSlider(3.0)] _Glossiness("Glossiness", Range(1, 1000)) = 32

		[Header(_______________ Emission Options ______________________________________________)]
		[Toggle] Feature_Emission("Emission", Float) = 0
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0,0)

		[Header(_______________ Final Render Options __________________________________________)]
		[Enum(UnityEngine.Rendering.CullMode)]	_Cull("Cull", Int) = 2
		[Enum(OFF,0,Less,4,Greater,7)]			_ZTest("Depth Test", Int) = 4
		[Enum(OFF,0,ON,1)]						_ZWrite("Depth Write", Int) = 1
		[Enum(OFF,0,ON,15)]						_ColorMask("Color Write", Int) = 15
		[Enum(UnityEngine.Rendering.CullMode)]	_ShadowCull("Shadow Caster Cull", Int) = 2
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
			#pragma multi_compile __ FEATURE_UVMOVEMENT_ON
			#pragma multi_compile _ADDITIONALTILE_NONE _ADDITIONALTILE_ADD _ADDITIONALTILE_MULTIPLY
			#pragma multi_compile _OVERRIDEUV_NONE _OVERRIDEUV_UP _OVERRIDEUV_CUBE
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
				float2 texcoord0 : TEXCOORD0;

#if _ADDITIONALTILE_ADD || _ADDITIONALTILE_MULTIPLY
				float2 texcoord1 : TEXCOORD1;
#endif

				float4 shade : TEXCOORD2;

#if FEATURE_SPECULAR_ON
				float4 specular : TEXCOORD3;
#endif
				SHADOW_COORDS(4)
				UNITY_FOG_COORDS(5)
			};

			fixed4 _Color;
			fixed4 _Ambient;
			sampler2D _MainTex;
			float4 _MainTex_ST;

#if FEATURE_UVMOVEMENT_ON
			float _MainTileU;
			float _MainTileV;
#endif

#if _ADDITIONALTILE_ADD || _ADDITIONALTILE_MULTIPLY
			float4 _AddColor;
			sampler2D _AddTex;
			float4 _AddTex_ST;
			float _AddTileU;
			float _AddTileV;
#endif

#if FEATURE_EMISSION_ON
			float4 _EmissionColor;
#endif
#if FEATURE_SPECULAR_ON
			float4 _SpecularColor;
			float _Glossiness;
#endif

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			vertex_output vert(vertex_input v)
			{
				vertex_output o;
				UNITY_SETUP_INSTANCE_ID(v);

				o.position = UnityObjectToClipPos(v.vertex);

#if FEATURE_VERTEX_COLOR_ON
				o.color = v.color;
#else
				o.color = 1;
#endif

#if _OVERRIDEUV_UP
				o.texcoord0 = mul(unity_ObjectToWorld, v.vertex).xz;
				o.texcoord0 = TRANSFORM_TEX(o.texcoord0, _MainTex);
#elif _OVERRIDEUV_CUBE
				float3 blend = abs( normalize(v.normal) );
				blend /= dot(blend, 1.0f);
				float3 word_position = mul(unity_ObjectToWorld, v.vertex);
				o.texcoord0 = blend.x * word_position.yz + blend.y * word_position.xz + blend.z * word_position.xy;
				o.texcoord0 = TRANSFORM_TEX(o.texcoord0, _MainTex);
#else
				o.texcoord0 = TRANSFORM_TEX(v.texcoord, _MainTex);
#endif

#if FEATURE_UVMOVEMENT_ON
				o.texcoord0.x += _Time.x * _MainTileU;
				o.texcoord0.y += _Time.x * _MainTileV;
#endif

#if _ADDITIONALTILE_ADD || _ADDITIONALTILE_MULTIPLY
				float2 texcoord1 = TRANSFORM_TEX(v.texcoord, _AddTex);
				o.texcoord1.x = texcoord1.x + _Time.x * _AddTileU;
				o.texcoord1.y = texcoord1.y + _Time.x * _AddTileV;
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
				fixed4 c = v.color;

				fixed4 t0 = tex2D(_MainTex, v.texcoord0) * _Color;

#if _ADDITIONALTILE_ADD || _ADDITIONALTILE_MULTIPLY
				fixed4 t1 = tex2D(_AddTex, v.texcoord1) * _AddColor;
				c.a = min(1, t0.a + t1.a);

#if _ADDITIONALTILE_ADD
				c.rgb *= t0.rgb * _Color * t0.a + t1.rgb * _AddColor * t1.a;
#else
				c.rgb *= lerp(t0, lerp(t1, t0 * t1, t0.a * t1.a), t1.a);
#endif

#else  // _ADDITIONALTILE_ADD || _ADDITIONALTILE_MULTIPLY
				c *= t0;
#endif // _ADDITIONALTILE_ADD || _ADDITIONALTILE_MULTIP
				
				
				float shadow = SHADOW_ATTENUATION(v);
				float light = smoothstep(0, 1, v.shade.a * shadow);
				float3 attenu = _LightColor0.rgb * light;
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
