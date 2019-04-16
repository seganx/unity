// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seganx/Particle"
{
    Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _CoreColor("Core Color", Color) = (1.0,1.0,1.0,1.0)
        _CoreThreshold("Core Color Threshold", Range(0, 1)) = 0.5
        _ColorStrength("Color Strength", Float) = 1

        [Enum(ON,1,OFF,0)]	_ZWrite("Z Write", Int) = 0
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 0
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 1
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

                    #include "UnityCG.cginc"

                    struct VertexInput
                    {
                        float4 pos : POSITION;
                        float2 uv0 : TEXCOORD0;
                        fixed4 col : COLOR0;
                    };

                    struct VertexOutput
                    {
                        float4 pos : SV_POSITION;
                        float2 uv0 : TEXCOORD0;
                        fixed4 col : COLOR0;
                    };

                    float4 _Color;
                    float _ColorStrength;
                    sampler2D _MainTex;
                    float4 _MainTex_ST;

                    VertexOutput vert(VertexInput v)
                    {
                        VertexOutput o;
                        o.pos = UnityObjectToClipPos(v.pos);
                        o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                        o.col = v.col * _ColorStrength;
                        return o;
                    }

                    float4 _CoreColor;
                    float _CoreThreshold;
                    fixed4 frag(VertexOutput i) : SV_Target
                    {
                        fixed4 c = tex2D(_MainTex, i.uv0);
                        clip(c.a < 0.001f);
#if 0
                        if (_CoreThreshold > 0.001f)
                            return c * i.col * lerp(_Color, _CoreColor, clamp(pow(c.a, 100) * _CoreThreshold * 100, 0, 1));
                        else 
#endif
                            return c * i.col * (c.a < _CoreThreshold ? _Color : _CoreColor);
                    }

                ENDCG
            }
        }

}
