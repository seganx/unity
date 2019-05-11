Shader "SeganX/FX/Bloom/DownScale"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _FilterRange("Filter Range", Range(0, 100)) = 7
        
        [HideInInspector] _Offset("Offset", Vector) = (0,0,0,0)
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
            float _FilterRange;
            float4 _Offset;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 c1 = tex2D(_MainTex, i.uv).rgb;
                fixed3 c2 = tex2D(_MainTex, i.uv + _Offset.xy).rgb;
                fixed3 c3 = tex2D(_MainTex, i.uv - _Offset.xy).rgb;
                fixed3 c4 = tex2D(_MainTex, i.uv + _Offset.zw).rgb;
                fixed3 c5 = tex2D(_MainTex, i.uv - _Offset.zw).rgb;
                fixed4 col = 1;
                col.rgb = (c1 + c2 + c3 + c4 + c5) / 5;

                col.rgb = col.rgb * col.rgb * pow((col.r + col.g + col.b) / 3, _FilterRange) * _FilterRange * 0.5f;
                return col;
			}
			ENDCG
		}
	}
}
