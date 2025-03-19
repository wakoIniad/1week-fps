Shader "Custom/WorldBaseSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _MainTex_ST;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(half, _Glossiness)
            UNITY_DEFINE_INSTANCED_PROP(half, _Metallic)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        float4 texTriPlanar(sampler2D tex, float3 p, float3 n, float4 st)
        {
            float3 blending = abs(n);
            blending = normalize(max(blending, 0.00001));

            // normalized total value to 1.0
            float b = (blending.x + blending.y + blending.z);
            blending /= b;

            float4 xaxis = tex2D(tex, p.yz * st.xy + st.zw);
            float4 yaxis = tex2D(tex, p.xz * st.xy + st.zw);
            float4 zaxis = tex2D(tex, p.xy * st.xy + st.zw);

            // blend the results of the 3 planar projections.
            return (xaxis * blending.x + yaxis * blending.y + zaxis * blending.z);
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = texTriPlanar(_MainTex, IN.worldPos, o.Normal, _MainTex_ST) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            o.Albedo = c.rgb;
            o.Metallic = UNITY_ACCESS_INSTANCED_PROP(Props, _Metallic);
            o.Smoothness = UNITY_ACCESS_INSTANCED_PROP(Props, _Glossiness);
            o.Alpha = c.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
