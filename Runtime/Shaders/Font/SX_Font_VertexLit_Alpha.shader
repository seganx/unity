Shader "SeganX/Font/VertexLit/Transparent" 
{
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorStrength("Color Strength", Float) = 1
		_AmbientStrength("Ambient Strength", Float) = 1

		
		[Header(Render Options)]
		[Enum(ON,1,OFF,0)]				_ZWrite ("Z Write", Int) = 0
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
		[Enum(BACK,2,FRONT,1,OFF,0)]	_ShadowCull ("Shadow Caster Cull", Int) = 2
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Source Factor", Int) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDest("Destination Factor", Int) = 10
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
			ZWrite[_ZWrite]
			Blend[_BlendSrc][_BlendDest]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing

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
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
				float4 shade : TEXCOORD1;

				SHADOW_COORDS(2)
				UNITY_FOG_COORDS(3)
			};

			fixed4 _Color;
			float _ColorStrength;
			float _AmbientStrength;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			vertex_output vert (vertex_input v)
			{
				v.normal = float3(0, 0, -1);

				vertex_output o;
				UNITY_SETUP_INSTANCE_ID( v );

				o.position = UnityObjectToClipPos( v.vertex );
				o.texcoord = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.color = v.color * _Color * _ColorStrength;

				float3 world_normal = UnityObjectToWorldNormal( v.normal );
				float3 ambient = ShadeSH9( float4( world_normal, 1.0f ) ) * _AmbientStrength;
				float dot_light = max( 0, dot( world_normal, _WorldSpaceLightPos0.xyz ) );
				o.shade = float4( ambient, dot_light );

				UNITY_TRANSFER_FOG( o, o.position);
				TRANSFER_SHADOW( o );
				return o;
			}
			
			fixed4 frag (vertex_output v) : SV_Target
			{
				fixed4 c = tex2D( _MainTex, v.texcoord );

				float shadow = SHADOW_ATTENUATION(v);
				float3 attenu = _LightColor0.rgb * v.shade.a * shadow;

				c.rgb = v.color.rgb * (v.shade.rgb + attenu);
				c.a *= v.color.a;


				UNITY_APPLY_FOG( v.fogCoord, c );
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
			Cull [_ShadowCull]
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

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
				V2F_SHADOW_CASTER;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			fixed4 _Color;
			float _ColorStrength;
			sampler2D _MainTex;
			float4 _MainTex_ST;
		
			vertex_output vert( vertex_input v )
			{
				vertex_output o;
				UNITY_SETUP_INSTANCE_ID( v );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o );

				o.texcoord = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.color = v.color * _Color * _ColorStrength;

				return o;
			}

			float4 frag( vertex_output i ) : SV_Target
			{
				fixed4 c = tex2D( _MainTex, i.texcoord );
				c.a *= i.color.a;
				clip( c.a - 0.01f );

				SHADOW_CASTER_FRAGMENT( i )
			}
			ENDCG
		}
	}
	// SubShader

FallBack "Mobile/VertexLit"
}
