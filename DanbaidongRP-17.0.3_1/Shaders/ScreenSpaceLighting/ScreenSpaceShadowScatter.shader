Shader "Hidden/ScreenSpaceShadowScatter"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }


        // 0: Shadow estimation
        Pass
        {
            Name "ShadowScatter"

            // -------------------------------------
            // Render State Commands
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma editor_sync_compilation
            #pragma target 4.5
            #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

            // -------------------------------------
            // Shader Stages
            #pragma vertex Vert
            #pragma fragment FragShadowScatter

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Lighting.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/ScreenSpaceLighting/ScreenSpaceLighting.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/ShadowsPCSS.hlsl"


            #define DOWNSAMPLE  (0.5)

            // Scatter use big width and small count
            #define PREFILTER_SAMPLE_COUNT          (8)
            #define BLOCKER_SAMPLE_COUNT            (8)
            #define PCSS_SAMPLE_COUNT               (16)
            // World space filter size.
            #define FILTER_SIZE_PREFILTER           (0.4)
            #define FILTER_SIZE_BLOCKER             (0.2)
            #define DIR_LIGHT_PENUMBRA_WIDTH        _DirLightShadowPenumbraParams.y * 3

            int _CamHistoryFrameCount;


            float4 FragShadowScatter(Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 screenUV = i.texcoord.xy;

                float depth = SampleSceneDepth(screenUV);
                if (depth == UNITY_RAW_FAR_CLIP_VALUE)
                    return 1.0;

                PositionInputs posInput = GetPositionInput(screenUV * _ScreenSize.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V, uint2(0, 0));

                float3 positionWS = posInput.positionWS;
                float cascadeIndex = ComputeCascadeIndex(positionWS);
                float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));

                float radial2ShadowmapDepth = _PerCascadePCSSData[cascadeIndex].x;
                float texelSizeWS           = _PerCascadePCSSData[cascadeIndex].y;
                float farToNear             = _PerCascadePCSSData[cascadeIndex].z;
                float blockerInvTangent     = _PerCascadePCSSData[cascadeIndex].w;

                // Sample Noise: Use Jitter instead.
                float2 noiseJitter = 0;
                noiseJitter.xy = InterleavedGradientNoise(screenUV * _ScreenSize.xy * DOWNSAMPLE, _CamHistoryFrameCount);
                noiseJitter *= TWO_PI;
                noiseJitter.x = sin(noiseJitter.x);
                noiseJitter.y = cos(noiseJitter.y);

                // PreFilter Search
                float preFilterSize = FILTER_SIZE_PREFILTER / texelSizeWS; // texel count
                preFilterSize = max(preFilterSize, 1.0);
                float preFilterRet = PreFilterSearch(PREFILTER_SAMPLE_COUNT, 2.0 * preFilterSize, shadowCoord.xyz, cascadeIndex, noiseJitter);

                UNITY_BRANCH
                if (preFilterRet > 0 && preFilterRet < PREFILTER_SAMPLE_COUNT)
                {
                    // Blocker Search
                    float filterSize = FILTER_SIZE_BLOCKER / texelSizeWS; // texel count
                    filterSize = max(filterSize, 1.0);
                    float2 avgDepthAndCount = BlockerSearch(BLOCKER_SAMPLE_COUNT, filterSize, shadowCoord.xyz, noiseJitter, cascadeIndex);
                    if (avgDepthAndCount.y == 0) // No Blocker
                    {
                        return 1.0;
                    }

                    // Penumbra Estimation
                    float blockerDistance = abs(avgDepthAndCount.x - shadowCoord.z);
                    blockerDistance *= farToNear;
                    blockerDistance = min(blockerDistance, 10.0);

                    float maxPCSSoffset = blockerDistance / farToNear * 0.25;

                    float pcssFilterSize = DIR_LIGHT_PENUMBRA_WIDTH * blockerDistance * 0.01 / texelSizeWS;
                    pcssFilterSize = max(pcssFilterSize, 0.01);


                    // PCSS Filter
                    float pcssResult = PCSSFilter(PCSS_SAMPLE_COUNT, pcssFilterSize, shadowCoord.xyz, noiseJitter, cascadeIndex, maxPCSSoffset);

                    return pcssResult;
                }
                else
                {
                    return preFilterRet > 0 ? 0.0 : 1.0;
                }

            }
            ENDHLSL
        }
    }
}
