Shader "SeganX/FX/Screen/Lut"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _LutTex("LUT Texture", 3D) = "white" {}
        _LutSize("Lut Width", Float) = 16
        _LutBlend("Lut Blend", Range(-0.5, 1.5)) = 1
        _ColorStrength("Color Strength", Float) = 1
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
                    fixed4 col : COLOR;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv0 : TEXCOORD0;
                    fixed4 col : COLOR;
                    float2 uv1 : TEXCOORD1;
                };


                fixed4 _Color;
                fixed _ColorStrength;
                fixed _LutSize;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    
                    o.col.rgb = _ColorStrength;
                    o.col.a = 1;
                    o.col *= _Color;

                    o.uv0 = v.uv;
                    
                    o.uv1.x = (_LutSize - 1) / (1.0f * _LutSize);
                    o.uv1.y = 1.0f / (4.0f * _LutSize);
                    return o;
                }

                sampler2D _MainTex;
                sampler3D _LutTex;
                fixed _LutBlend;

                fixed4 frag(v2f i) : SV_Target
                {
                    float4 c = tex2D(_MainTex, i.uv0);
    #if SHADER_API_GLES3 || SHADER_API_D3D11 || SHADER_API_D3D11_9X
                    float3 lut = tex3D(_LutTex, c.rgb * i.uv1.x + float3(i.uv1.y, i.uv1.y, i.uv1.y)).rgb;
                    c.rgb = c.rgb * (1 - _LutBlend) + lut.rgb * _LutBlend;
    #endif
                    return c * i.col;
                }
                ENDCG
            }
        }
}
