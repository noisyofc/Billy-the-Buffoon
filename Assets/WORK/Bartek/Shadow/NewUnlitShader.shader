Shader "Unlit/BlobShadow"
{
    Properties
    {
        _Color ("Shadow Color", Color) = (0,0,0,0.5)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Color [_Color]
        }
    }
}
