Shader "Hidden/Universal Render Pipeline/LensFlareScreenSpace"
{
    SubShader
    {
        Tags{ "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Name "LensFlareScreenSpac Prefilter"
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
            LOD 100

            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment FragmentPrefilter

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/PostProcessing/Common.hlsl"

            #define URP_LENS_FLARE_SCREEN_SPACE

            #include "Packages/com.unity.render-pipelines.core/Runtime/PostProcessing/Shaders/LensFlareScreenSpaceCommon.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "LensFlareScreenSpace Downsample"
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
            LOD 100

            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment FragmentDownsample

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/PostProcessing/Common.hlsl"

            #define URP_LENS_FLARE_SCREEN_SPACE

            #include "Packages/com.unity.render-pipelines.core/Runtime/PostProcessing/Shaders/LensFlareScreenSpaceCommon.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "LensFlareScreenSpace Upsample"
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
            LOD 100

            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment FragmentUpsample

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/PostProcessing/Common.hlsl"

            #define URP_LENS_FLARE_SCREEN_SPACE

            #include "Packages/com.unity.render-pipelines.core/Runtime/PostProcessing/Shaders/LensFlareScreenSpaceCommon.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "LensFlareScreenSpace Composition"
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
            LOD 100

            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment FragmentComposition

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/PostProcessing/Common.hlsl"

            #define URP_LENS_FLARE_SCREEN_SPACE

            #include "Packages/com.unity.render-pipelines.core/Runtime/PostProcessing/Shaders/LensFlareScreenSpaceCommon.hlsl"

            ENDHLSL
        }
        
        Pass
        {
            Name "LensFlareScreenSpace Write to BloomTexture"
            Tags{ "LightMode" = "Forward"  "RenderQueue" = "Transparent" }
            
            Blend One One
            BlendOp Add
            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment FragmentWrite

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/PostProcessing/Common.hlsl"

            #define URP_LENS_FLARE_SCREEN_SPACE

            #include "Packages/com.unity.render-pipelines.core/Runtime/PostProcessing/Shaders/LensFlareScreenSpaceCommon.hlsl"

            ENDHLSL
        }

    }
}
