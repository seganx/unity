Shader "SeganX/FX/Bloom/Blur"
{
	Properties
	{
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _Offset ("Offset", Vector) = (0,0,0,0)
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            uniform float4 _Offset;

			fixed4 frag (v2f i) : SV_Target
			{
                fixed4 c1 = tex2D(_MainTex, i.uv);
                fixed4 c2 = tex2D(_MainTex, i.uv + _Offset.xy);
                fixed4 c3 = tex2D(_MainTex, i.uv - _Offset.xy);
                fixed4 c4 = tex2D(_MainTex, i.uv + _Offset.zw);
                fixed4 c5 = tex2D(_MainTex, i.uv - _Offset.zw);
                //return max(c1, max(c2, max(c3, max(c4, c5))));
                //return (c1 * c1.a + c2 * c2.a + c3 * c3.a + c4 * c4.a + c5 * c5.a) / (c1.a + c2.a + c3.a + c4.a + c5.a + 0.00005f);
                return (c1 + c2 + c3 + c4 + c5) / 3;
                //return c1;
			}
			ENDCG
		}
	}
}
