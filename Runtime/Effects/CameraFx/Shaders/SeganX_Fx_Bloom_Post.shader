Shader "SeganX/FX/Bloom/Post"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _RGBStrength("RGB Strength", Float) = 1
        [HideInInspector]_MainTex("Texture", 2D) = "white" {}

        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 10
    }

        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always
            Blend[_BlendSrc][_BlendDest]

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                sampler2D _MainTex;
                fixed4 _Color;
                fixed _RGBStrength;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.rgb *= _RGBStrength;
                    col.a = pow(col.a, 0.75f);
                    return col * _Color;
                }
                ENDCG
            }
        }
}
