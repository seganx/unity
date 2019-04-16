Shader "Seganx/UI/Effect" {
    Properties{
        _MainTex("Sprite", 2D) = "white" {}
        _LutTex("LUT Texture", 3D) = "white" {}
        _LutSize("Lut Width", Float) = 16
        _LutBlend("Lut Blend", Range(-0.5, 1.5)) = 1
        _ColorStrength("Color Strength", Float) = 1

        [Enum(ON,1,OFF,0)]	_ZWrite("Z Write", Int) = 0
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 2
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 10

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15
    }

        SubShader{

            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull[_Cull]
            ZWrite[_ZWrite]
            Blend[_BlendSrc][_BlendDest]

            Lighting Off
            ZTest[unity_GUIZTestMode]
            ColorMask[_ColorMask]

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct vs_in {
                    float4 pos : POSITION;
                    fixed4 col : COLOR;
                    float2 uv0 : TEXCOORD0;
                };

                struct vs_out {
                    float4 pos : SV_POSITION;
                    fixed4 col : COLOR;
                    float2 uv0 : TEXCOORD0;
                    float2 uv1 : TEXCOORD1;
                    float2 sof : TEXCOORD2;
                };

                sampler2D _MainTex;
                sampler3D _LutTex;
                sampler2D _DistTex;
                uniform float4 _MainTex_ST;
                uniform float4 _DistTex_ST;
                float4 _DistSpeed;
                float4 _DistColor;
                float _LutSize;

                vs_out vert(vs_in v)
                {
                    vs_out o;
                    o.pos = UnityObjectToClipPos(v.pos);
                    o.col = v.col;
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.uv1 = TRANSFORM_TEX(v.uv0, _DistTex) + _Time.x * _DistSpeed.xy;
                    o.sof.x = (_LutSize - 1) / (1.0f * _LutSize);
                    o.sof.y = 1.0f / (4.0f * _LutSize);

    #ifdef UNITY_HALF_TEXEL_OFFSET
                    o.pos.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
    #endif
                    return o;
                }


                fixed _ColorStrength;
                fixed _LutBlend;

                fixed4 frag(vs_out i) : SV_Target
                {
                    float4 c = tex2D(_MainTex, i.uv0);
                    float3 lut = tex3D(_LutTex, c.rgb * i.sof.x + float3(i.sof.y, i.sof.y, i.sof.y)).rgb;
#ifdef OFF
                    float2 luv;
                    luv.x = c.r / _LutSize;
                    luv.x += ceil(c.b * _LutSize) / _LutSize;
                    luv.x = clamp(luv.x, 0, 1);
                    luv.y = 1.0f - c.g;
                    float3 lut = tex2D(_LutTex, luv * i.sof.x + i.sof.y).rgb;
                    _LutBlend *= 0.25f;
#endif
                    c.rgb = c.rgb * (1 - _LutBlend) + lut.rgb * _LutBlend;
                    c.rgb *= _ColorStrength;
                    return c * i.col;
                }
                ENDCG
            }
        }
}
