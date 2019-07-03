Shader "SeganX/FX/Screen/Post"
{
	Properties
	{
        _Screen("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Contrast("Contrast", Range(0, 4)) = 1
        _Brightness("Brightness", Range(0, 4)) = 1
        _Saturation("Saturation", Range(0, 2)) = 1


        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 10
	}

	SubShader
	{
		// No culling or depth
		Cull Off 
        ZWrite Off 
        Blend [_BlendSrc] [_BlendDest]

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
			
            sampler2D _Screen;
            sampler2D _MainTex;
            fixed4 _Color;
            float _Contrast;
            float _Brightness;
            float _Saturation;

			fixed4 frag (v2f i) : SV_Target
			{
                fixed4 c;
                c .rgb = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                //c.rgb = pow(c.rgb, _Brightness) * _Contrast;
                //c.rgb = lerp((c.r + c.g + c.b) / 3, c.rgb, _Saturation);
                fixed4 s = tex2D(_Screen, i.uv);
                c.rgb *= s.rgb;
                c.a = s.a * _Color.a;
                return c;
			}
			ENDCG
		}
	}
}
