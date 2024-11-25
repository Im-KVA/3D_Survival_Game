Shader "Custom/SkyboxTransition"
{
    Properties
    {
        _TransitionFactor("Transition Factor", Range(0, 1)) = 0.0
        _AtmosphereTex("Atmosphere CubeMap", Cube) = "" {}
        _SpaceTex("Space CubeMap", Cube) = "" {}
    }
 
    SubShader
    {
        Tags { "Queue"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
            };
 
            struct v2f
            {
                float3 pos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            float _TransitionFactor;
            samplerCUBE _AtmosphereTex;
            samplerCUBE _SpaceTex;
 
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                // Use the transition factor to blend between the two skybox cube maps
                half4 atmosphereColor = texCUBE(_AtmosphereTex, i.pos);
                half4 spaceColor = texCUBE(_SpaceTex, i.pos);
                half4 finalColor = lerp(atmosphereColor, spaceColor, _TransitionFactor);
 
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Skybox/Cubemap"
}