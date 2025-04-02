Shader "Custom/SpriteShadowReceiver"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags {"Queue"="Geometry" "RenderType"="Opaque"}
        CGPROGRAM
        #pragma surface surf Lambert addshadow

        sampler2D _MainTex;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            
            // Discard fully transparent pixels to prevent them from receiving shadows
            clip(c.a - _Cutoff);
            
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
