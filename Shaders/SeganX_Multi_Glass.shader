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


                fixed4 _DiffColor1;
                fixed4 _DiffColor2;
                float _Reflection1;
                float _Reflection2;
                float _SpecularPower1;
                float _SpecularAtten1;
                float _SpecularPower2;
                float _SpecularAtten2;
                uniform float bloomSpecular;


                fixed4 frag(v2f i) : SV_Target
                {
                    //  extract material id from vertex color
                    uint umatId = (round(i.colr.r * 255) / 10) - 1;
                    float matId = clamp(umatId, 0, 1);
                    
                    // compute parametres based on material id
                    fixed4 diffcolor = lerp(_DiffColor1, _DiffColor2, matId);
                    float reflection = lerp(_Reflection1, _Reflection2, matId);
                    float specpower = lerp(_SpecularPower1, _SpecularPower2, matId);
                    float specvalue = lerp(_SpecularAtten1, _SpecularAtten2, matId);

                    // compute lighting params
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                    half3 viewDir = normalize(UnityWorldSpaceViewDir(i.wrl));

                    // construct basic color
                    fixed4 res = lerp(1, tex2D(_MainTex, i.uv0), matId) * diffcolor;

                    // apply basic lighting
                    {
                        fixed3 diffuse = _LightColor0.rgb * max(0, dot(i.norm, lightDir));
                        fixed3 ambient = (i.norm.y > 0) ? lerp(unity_AmbientEquator.rgb, unity_AmbientSky.rgb, i.norm.y) : lerp(unity_AmbientEquator.rgb, unity_AmbientGround.rgb, -i.norm.y);
                       // res.rgb *= (diffuse + ambient);
                    }

                    // apply reflection
                    {
                        half cube = UNITY_SAMPLE_TEXCUBE( unity_SpecCube0, reflect(-viewDir, i.norm) ).r;
                        res.rgb += cube * reflection;
                        res.a = max(res.a, cube * reflection);
                    }

                    // apply specular
                    {
                        float spec = pow( max( 0, dot(i.norm, normalize(lightDir + viewDir))), specpower) * specvalue;
                        res.rgb += _LightColor0.rgb * spec;
                        res.a = max(res.a, pow(spec, 8));
                    }                    

                    res.rgb *= _LightColor0.a;
                    UNITY_APPLY_FOG(i.fogCoord, res);
                    return res;
                }
                ENDCG
            }
        }
        Fallback "Mobile/Diffuse"
}
