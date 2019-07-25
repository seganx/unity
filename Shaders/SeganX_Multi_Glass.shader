Shader "SeganX/Multi/Glass"
{
    Properties
    {
        [Enum(ON,1,OFF,0)]	            _ZWrite("Z Write", Int) = 0
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 2
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendSrc("SrcFactor", Int) = 5
        [Enum(Zero,0,One,1,DstColor,2,SrcColor,3,SrcAlpha,5,DstAlpha,7,OneMinusSrcAlpha,10)] _BlendDest("DstFactor", Int) = 10

        [Space]
        [Header(Material id 1)]
        _DiffColor1("1: Diffuse Color", Color) = (1,1,1,1)
        _Reflection1("1: Reflection", Range(0, 1)) = 0.5
        [Space]
        _SpecularColor1("1: Specular Color (+ alpha x light.color)", Color) = (1,1,1,1)
        _SpecularAtten1("1: Specular", Range(0, 2)) = 0.5
        _SpecularPower1("1: Specular Power", Range(5, 200)) = 50

        [Space]
        [Header(Material id 2)]
        _MainTex("Diffuse Texture", 2D) = "white" {}
        _DiffColor2("2: Diffuse Color", Color) = (1,1,1,1)
        _Reflection2("2: Reflection", Range(0, 1)) = 0.5
        [Space]
        _SpecularColor2("2: Specular Color (+ alpha x light.color)", Color) = (1,1,1,1)
        _SpecularAtten2("2: Specular", Range(0, 2)) = 0.5
        _SpecularPower2("2: Specular Power", Range(5, 200)) = 50
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "LightMode" = "ForwardBase"
            }

            Cull[_Cull]
            ZWrite[_ZWrite]
            Blend[_BlendSrc][_BlendDest]

            Pass
            {
                CGPROGRAM
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                struct appdata
                {
                    float4 vrtx : POSITION;
                    float3 norm : NORMAL;
                    float4 colr : COLOR;
                    float2 uv0 : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vrtx : SV_POSITION;
                    float3 norm : NORMAL;
                    float4 colr : COLOR;
                    float2 uv0 : TEXCOORD0;
                    float3 wrl : TEXCOORD1;
                    UNITY_FOG_COORDS(2)
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vrtx = UnityObjectToClipPos(v.vrtx);
                    o.norm = UnityObjectToWorldNormal(v.norm);
                    o.colr = v.colr;
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.wrl = mul(unity_ObjectToWorld, v.vrtx).xyz;
                    UNITY_TRANSFER_FOG(o, o.vrtx);
                    return o;
                }


                sampler2D _MetalTex;
                fixed4 _DiffColor1;
                fixed4 _DiffColor2;
                fixed4 _SpecularColor1;
                fixed4 _SpecularColor2;
                float _Reflection1;
                float _Reflection2;
                float _SpecularPower1;
                float _SpecularAtten1;
                float _SpecularPower2;
                float _SpecularAtten2;
                float _MetalPower2;
                uniform float bloomSpecular;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 res = bloomSpecular;

                    uint matId = round(i.colr.r * 255) / 10;
                    if (matId == 1)
                    {
                        res = _DiffColor1;
                    }
                    else 
                    {
                        res = tex2D(_MainTex, i.uv0);
                        res *= _DiffColor2;
                    }

                    half3 viewDir = normalize(UnityWorldSpaceViewDir(i.wrl));
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                    fixed3 diffuse = _LightColor0.rgb * max(0, dot(i.norm, lightDir));
                    fixed3 ambient = (i.norm.y > 0) ? lerp(unity_AmbientEquator.rgb, unity_AmbientSky.rgb, i.norm.y) : lerp(unity_AmbientEquator.rgb, unity_AmbientGround.rgb, -i.norm.y);
                    res.rgb *= (diffuse + ambient);

                    if (matId == 1)
                    {
                        if (_Reflection1 > 0.01f)
                        {
                            half cube = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(-viewDir, i.norm)).r;
                            res.rgb += cube * _Reflection1;
                            res.a = max(res.a, cube * _Reflection1);
                        }

                        if (_SpecularAtten1 > 0.01f)
                        {
                            float spec = pow(max(0, dot(i.norm, normalize(lightDir + viewDir))), _SpecularPower1) * _SpecularAtten1;
                            res.rgb += (_SpecularColor1.rgb + _LightColor0.rgb * _SpecularColor1.a) * spec;
                            res.a = max(res.a, pow(spec, 8));
                        }
                    }
                    else// if (matId == 2)
                    {
                        res.rgb *= 0.7f;
                        if (_Reflection2 > 0.01f)
                        {
                            half cube = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(-viewDir, i.norm)).r;
                            res.rgb += cube * _Reflection2;
                            res.a = max(res.a, cube * _Reflection2);
                        }

                        if (_SpecularAtten2 > 0.01f)
                        {
                            float spec = pow(max(0, dot(i.norm, normalize(lightDir + viewDir))), _SpecularPower2) * _SpecularAtten2;
                            res.rgb += (_SpecularColor2.rgb + _LightColor0.rgb * _SpecularColor2.a) * spec;
                            res.a += spec;
                        }
                    }

                    UNITY_APPLY_FOG(i.fogCoord, res);
                    return res;
                }
                ENDCG
            }
        }
        Fallback "Mobile/Diffuse"
}
