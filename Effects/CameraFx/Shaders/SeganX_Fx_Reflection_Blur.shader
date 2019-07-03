Shader "SeganX/FX/Reflection/Blur"
{
	Properties 
	{
        _MaskTex("Water mask (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_ColorStrength ("Color Strength", Float) = 1

		[Enum(ON,1,OFF,0)]	_ZWrite ("Z Write", Int) = 1
		[Enum(BACK,2,FRONT,1,OFF,0)]	_Cull ("Cull", Int) = 2
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 10
	}

	SubShader 
	{
        //Tags { "RenderType" = "Opaque" }
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "LightMode" = "ForwardBase"
        }

		LOD 100
		
		Cull [_Cull]
		ZWrite [_ZWrite]
        Blend[_BlendSrc][_BlendDest]

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
					float2 uv0 : TEXCOORD0;
				};
				
				struct VertexOutput 
				{
					float4 pos : SV_POSITION;
                    float2 uv0 : TEXCOORD0;
                    float4 uv1 : TEXCOORD1;
				};

				VertexOutput vert (VertexInput v)
				{
					VertexOutput o;
					o.pos = UnityObjectToClipPos( v.pos );
                    o.uv0 = v.uv0;
                    o.uv1 = o.pos;
					return o;
				}
			
                uniform sampler2D _RflctBlurTex;
                sampler2D _MaskTex;
                float _ColorStrength;
				float4 _Color;

				fixed4 frag (VertexOutput i) : SV_Target
				{
                    i.uv1.x = 0.5f - (0.5f * i.uv1.x / i.uv1.w);
#if SHADER_API_GLES3 || SHADER_API_GLES || SHADER_API_GLCORE
                    i.uv1.y = (0.5f * i.uv1.y / i.uv1.w) + 0.5f;
#else
                    i.uv1.y = 0.5f - (0.5f * i.uv1.y / i.uv1.w);
#endif
                    fixed4 c = tex2D(_RflctBlurTex, i.uv1);
                    c.rgb *= _Color.rgb * _ColorStrength;
                    c.a = tex2D(_MaskTex, i.uv0).a * _Color.a;
					return c;
				}
				
			ENDCG
		}
	}

}
