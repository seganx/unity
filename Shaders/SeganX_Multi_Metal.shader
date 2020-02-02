// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SeganX/Multi/Metal"
{
    Properties
    {
        _MainTex("Diffuse Texture", 2D) = "white" {}
        _VinylTex("Vinyl Texture", 2D) = "black" {}
        _MetalTex("Metal Texture", 2D) = "black" {}

        [Enum(Off,1,Transparent,0)]	    _FillMode("Fill Mode", Float) = 1
        [Enum(ON,1,OFF,0)]	            _ZWrite("Z Write", Int) = 0
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 2

        [Space]
        [Header(Material id 1)]
        _DiffColor1("1: Diffuse Color (alpha x vertex.color)", Color) = (1,1,1,1)
        _VinylColor("1: Vinyl Color", Color) = (1,1,1,1)
        _GlossColor("Gloss Color", Color) = (1,1,1,1)
        _Reflection1("1: Reflection", Range(0, 1)) = 0.5
        [Space]
        _SpecularAtten1("1: Specular", Range(0, 2)) = 0.75
        _SpecularPower1("1: Specular Power", Range(5, 200)) = 20
        _MetalPower1("1: MetalPower", Range(0, 2)) = 1

        [Space]
        [Header(Material id 2)]
        _DiffColor2("2: Diffuse Color (alpha x vertex.color)", Color) = (1,1,1,1)
        _SpecularAtten2("2: Specular", Range(0, 2)) = 0.5
        _SpecularPower2("2: Specular Power", Range(5, 200)) = 50
        _MetalPower2("2: MetalPower", Range(0, 2)) = 1.5

        [Space]
        [Header(Material id 3)]
        _DiffColor3("3: Diffuse Color (alpha x vertex.color)", Color) = (1,1,1,1)
        _Reflection3("3: Reflection", Range(0, 1)) = 0.5
        [Space]
        _SpecularAtten3("3: Specular", Range(0, 2)) = 0.5
        _SpecularPower3("3: Specular Power", Range(5, 200)) = 50
        _MetalPower3("3: MetalPower", Range(0, 2)) = 1.5
        
            
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "LightMode" = "ForwardBase"
            }

            Cull[_Cull]
            ZWrite[_ZWrite]

            Pass
            {
                CGPROGRAM
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile __ SX_OPP SX_SIMPLE

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                struct appdata
                {
                    float4 vrtx : POSITION;
                    float3 norm : NORMAL;
                    float4 colr : COLOR;
                    float2 uv0 : TEXCOORD0;
                    float2 uv1 : TEXCOORD1;
                };

                struct v2f
                {
                    float4 vrtx : SV_POSITION;
                    float3 norm : NORMAL;
                    float4 colr : COLOR;
                    float2 uv0 : TEXCOORD0;
                    float2 uv1 : TEXCOORD1;
                    float2 uv2 : TEXCOORD2;
                    float3 wrl : TEXCOORD3;
                    float2 zdp : TEXCOORD4;
                    UNITY_FOG_COORDS(5)
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _VinylTex;
                float4 _VinylTex_ST;
                sampler2D _MetalTex;
                float4 _MetalTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vrtx = UnityObjectToClipPos(v.vrtx);
                    o.norm = UnityObjectToWorldNormal(v.norm);
                    o.colr = v.colr;
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.uv1 = TRANSFORM_TEX(v.uv1, _VinylTex);
                    o.uv2 = TRANSFORM_TEX(v.uv1, _MetalTex);
                    o.wrl = mul(unity_ObjectToWorld, v.vrtx).xyz;

                    o.zdp.x = 0.5f * abs(o.vrtx.x / o.vrtx.w);
                    const float depz = -1.0f / (15.0f - 0.0f);
                    const float depw = 15.0f / (15.0f - 0.0f);
                    o.zdp.y = 0.5f * (UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vrtx.z) * depz + depw);

                    UNITY_TRANSFER_FOG(o, o.vrtx);
                    return o;
                }


                fixed4 _VinylColor;
                fixed4 _GlossColor;
                fixed4 _DiffColor1;
                fixed4 _DiffColor3;
                fixed4 _DiffColor2;
                float _Reflection1;
                float _Reflection3;
                float _SpecularPower1;
                float _SpecularAtten1;
                float _MetalPower1;
                float _SpecularPower3;
                float _SpecularAtten3;
                float _MetalPower3;
                float _SpecularPower2;
                float _SpecularAtten2;
                float _MetalPower2;
                uniform float bloomSpecular;

                float matlerp(uint matId, float v1, float v2, float v3)
                {
                    fixed4 r1 = lerp(v1, v2, clamp(matId, 0, 1));
                    fixed4 r2 = lerp(v2, v3, clamp(matId - 1, 0, 1));
                    return lerp(r1, r2, clamp(matId * 0.5f, 0, 1));
                }

                fixed4 matlerp(uint matId, fixed4 v1, fixed4 v2, fixed4 v3)
                {
                    fixed4 r1 = lerp(v1, v2, clamp(matId, 0, 1));
                    fixed4 r2 = lerp(v2, v3, clamp(matId - 1, 0, 1));
                    return lerp(r1, r2, clamp(matId * 0.5f, 0, 1));
                }

                float4 frag(v2f i) : SV_Target
                {
#if SX_OPP
                    clip(i.zdp.x + frac(i.vrtx.x * 0.5f) - i.zdp.y);
                    clip(i.zdp.x + frac(i.vrtx.y * 0.5f) - i.zdp.y);
#endif

                    //  extract material id from vertex color
                    uint matId = (round(i.colr.r * 255) / 10) - 1;  

                    // compute parametres based on material id
                    fixed4 diffcolor = matlerp(matId, _DiffColor1, _DiffColor2, _DiffColor3);
                    float reflection = matlerp(matId, _Reflection1, 0, _Reflection3);
#if !SX_OPP
                    float specpower = matlerp(matId, _SpecularPower1, _SpecularPower2, _SpecularPower3);
                    float specvalue = matlerp(matId, _SpecularAtten1, _SpecularAtten2, _SpecularAtten3);
#if !SX_SIMPLE
                    float metallic = matlerp(matId, _MetalPower1, _MetalPower2, _MetalPower3);
#endif
#endif

                    // compute lighting params
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                    half3 viewDir = normalize(UnityWorldSpaceViewDir(i.wrl));
#if !SX_OPP
                    float refresnel = 0.27f + 1 - dot(viewDir, i.norm);
#endif

                    // construct basic color
                    float4 res = float4( tex2D( _MainTex, i.uv0 ).rgb * diffcolor.rgb, bloomSpecular );
                    {
                        //  apply vinyl texture and color
                        half4 v = tex2D(_VinylTex, i.uv1) * _VinylColor;
                        res.rgb = lerp(lerp(res.rgb, v.rgb, v.a), res.rgb, clamp(matId, 0, 1));
                    }
                    res.rgb *= clamp(i.colr.a + diffcolor.a, 0, 1);

                    // apply basic lighting
                    {
                        half dl = max(0, dot(i.norm, lightDir));
                        fixed3 ambient = (i.norm.y > 0) ? lerp(unity_AmbientEquator.rgb, unity_AmbientSky.rgb, i.norm.y) : lerp(unity_AmbientEquator.rgb, unity_AmbientGround.rgb, -i.norm.y);
                        res.rgb *= lerp(ambient, _LightColor0.rgb, dl);
                    }
                    
                    // apply reflection
                    {
                        float cube = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(-viewDir, i.norm)).r;
                        cube *= cube;
#if SX_OPP || SX_SIMPLE
                        res.rgb += cube * reflection;
#else
                        res.a = max(res.a, cube * refresnel * reflection);
                        res.rgb += refresnel * cube * reflection;
#endif
                    }

                    // apply specular
#if !SX_OPP
                    {
                        float spec = max( 0, dot( i.norm, normalize( lightDir + viewDir ) ) );

                        //  layer 1 - gloss color
#if SX_SIMPLE
                        res.rgb += _GlossColor.rgb * pow(spec, 10) * specvalue;
#else
                        res.rgb += _GlossColor.rgb * pow(spec, 10) * specvalue * metallic;
#endif

                        //  layer 2 - specular color
                        res.rgb = lerp( res.rgb, 1, clamp( pow(spec, specpower * 2) * specvalue, 0, 1 ) );
#if !SX_SIMPLE
                        res.a = max(res.a, pow(spec, specpower * 80) * specvalue * 8);

                        // layer 3 - metallic 
                        res.rgb += tex2D(_MetalTex, i.uv2).a * metallic * pow(spec, 15);
#endif
                    }
#endif

                    res.rgb *= _LightColor0.a;

                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, res);

                    return res;
                }
                ENDCG
            }
        }
            Fallback "Mobile/Diffuse"
}
