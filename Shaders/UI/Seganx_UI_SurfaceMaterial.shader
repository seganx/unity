// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/UI/SurfaceMaterial"
{
    Properties
    {
        _MainColor("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Sprite (RGB)", 2D) = "white" {}
        _SurfColor("Color", Color) = (1.0,1.0,1.0,1.0)
        _SurfTex("Surface (RGB)", 2D) = "white" {}
        _SurfSpeed("Surf Speed", Vector) = (0, 0, 0, 0)

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

        SubShader
        {
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
                Name "FORWARD"
                Tags { "LightMode" = "ForwardBase" }

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
                    float2 uv2 : TEXCOORD2;
                };

                sampler2D _MainTex;
                sampler2D _SurfTex;
                float4 _MainTex_ST;
                float4 _SurfTex_ST;
                float4 _SurfSpeed;

                vs_out vert(vs_in v)
                {
                    vs_out o;
                    o.pos = UnityObjectToClipPos(v.pos);
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.uv1 = TRANSFORM_TEX(v.uv0, _SurfTex) + _Time.x * _SurfSpeed.xy;
                    o.uv2 = TRANSFORM_TEX(v.uv0, _SurfTex) + _Time.x * _SurfSpeed.zw;
                    o.col = v.col;
                    return o;
                }

                fixed4 _SurfColor;
                fixed4 _MainColor;


                fixed4 frag(vs_out i) : SV_Target
                {
                    fixed4 c = tex2D(_MainTex, i.uv0) * i.col;

                    fixed4 gray = 1 - (c.r + c.g + c.b) / 2;
                    gray *= _MainColor;
                    gray.a *= c.a;

                    fixed4 s = tex2D(_SurfTex, i.uv1);
                    if (any(_SurfSpeed))
                    {
                        s += tex2D(_SurfTex, -i.uv2.yx);
                        s *= 0.5f;
                    }
                    s *= _SurfColor;
                    s.a *= c.a;

                    fixed4 r;
                    r.rgb = s.rgb * (1 - gray.a) + gray.rgb * gray.a;
                    r.a = c.a;
                    //s.rgb *= 1 - gray.rgb;
                    return r;
                    //return s;
                }

                ENDCG
            }
        }

}
