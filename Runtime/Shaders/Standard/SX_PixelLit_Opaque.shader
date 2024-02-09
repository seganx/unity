Shader "SeganX/PixelLit/Opaque"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Ambient("Ambient", Color) = (0,0,0,0)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [Header(Render Options)]
        [Enum(UnityEngine.Rendering.CullMode)]	_Cull("Cull", Int) = 2
        [Enum(OFF,0,Less,4,Greater,7)]			_ZTest("Depth Test", Int) = 4
        [Enum(OFF,0,ON,1)]						_ZWrite("Depth Write", Int) = 1
        [Enum(OFF,0,ON,15)]						_ColorMask("Color Write", Int) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Cull[_Cull]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        ColorMask[_ColorMask]

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Ambient;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Emission = _Ambient;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
