Shader "SeganX/Particle"
{
    Properties
    {
        [HDR]_Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Base (RGB)", 2D) = "white" {}

        [Toggle] Feature_CoreGlow("Core Glow", Float) = 0
        [HDR]_CoreColor("Core Color", Color) = (1.0,1.0,1.0,1.0)
        _CoreStart("Core Start", Range(-1.0, 1.0)) = 0.5
        _CoreEnd("Core End", Range(-1.0, 1.0)) = 0.5

        [Enum(ON,1,OFF,0)]	_ZWrite("Z Write", Int) = 0
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDest("DstFactor", Int) = 1
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100
            Cull Off

            ZWrite[_ZWrite]
            Blend[_BlendSrc][_BlendDest]

            Pass
            {
                Name "FORWARD"
                Tags { "LightMode" = "ForwardBase" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile __ FEATURE_COREGLOW_ON

                #include "UnityCG.cginc"

                struct vertex_input
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR0;
                    float2 texcoord : TEXCOORD0;
                };

                struct vertex_output
                {
                    float4 position : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    fixed4 color : COLOR0;
                };

                float4 _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _CoreColor;
                float _CoreStart;
                float _CoreEnd;

                vertex_output vert(vertex_input v)
                {
                    vertex_output o;
                    o.position = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(vertex_output i) : SV_Target
                {
                    fixed4 c = tex2D(_MainTex, i.texcoord);
                    clip(c.a < 0.001f);

#if FEATURE_COREGLOW_ON
                    float t = smoothstep(_CoreStart, _CoreEnd, 1 - c.a);
                    return c * i.color * lerp(_CoreColor, _Color, t);
#else
                    return c * i.color * _Color;
#endif
                }

                ENDCG
            }
        }

}
