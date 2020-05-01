Shader "Universal Render Pipeline/Screen Space Decal"
{
    Properties
    {
        [Header(Basic)]
        _MainTex("Texture", 2D) = "white" {}
        [HDR]_Color("Albedo", color) = (1,1,1,1)

        [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Source Blend", Float) = 5 // 5 = SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Destination Blend", Float) = 10 // 10 = OneMinusSrcAlpha

        [Header(ZTest)]
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest Compare function", Float) = 0 // 0 = Disabled

        [Header(Culling)]
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull mode", Float) = 1 // 1 = Front
    }

    SubShader
    {
        Tags { "RenderType" = "Overlay" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent-499" }

        Pass
        {
            Cull[_Cull]
            ZTest[_ZTest]

            ZWrite off
            Blend[_SrcBlend][_DstBlend]

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                half4 vertex : POSITION;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half4 screenUV : TEXCOORD0;
                half4 viewRayOS : TEXCOORD1;
                half3 cameraPosOS : TEXCOORD2;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                half4 _Color;
            CBUFFER_END

            sampler2D _CameraDepthTexture;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                o.screenUV = ComputeScreenPos(o.vertex);

                half3 viewRay = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));

                o.viewRayOS.w = viewRay.z;

                viewRay *= -1;

                half4x4 ViewToObjectMatrix = mul(unity_WorldToObject, UNITY_MATRIX_I_V);

                o.viewRayOS.xyz = mul((half3x3)ViewToObjectMatrix, viewRay);
                o.cameraPosOS = mul(ViewToObjectMatrix, half4(0,0,0,1)).xyz;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                i.viewRayOS /= i.viewRayOS.w;

                half sceneCameraSpaceDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.screenUV).r, _ZBufferParams);

                half3 decalSpaceScenePos = i.cameraPosOS + i.viewRayOS.xyz * sceneCameraSpaceDepth;

                half2 decalSpaceUV = decalSpaceScenePos.xy + 0.5;
        
                half mask = (abs(decalSpaceScenePos.x) < 0.5 ? 1.0 : 0.0) * (abs(decalSpaceScenePos.y) < 0.5 ? 1.0 : 0.0) * (abs(decalSpaceScenePos.z) < 0.5 ? 1.0 : 0.0);

                clip(mask - 0.5);

                half2 uv = decalSpaceUV.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                half4 col = tex2D(_MainTex, uv);

                col *= _Color;

                return col;
            }

            ENDHLSL
        }
    }
}
